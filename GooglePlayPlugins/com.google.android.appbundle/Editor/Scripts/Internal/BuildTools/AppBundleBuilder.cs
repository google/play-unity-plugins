// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Google.Android.AppBundle.Editor.Internal.AndroidManifest;
using Google.Android.AppBundle.Editor.Internal.AssetPacks;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Helper to build an Android App Bundle file on Unity.
    /// </summary>
    public class AppBundleBuilder : IBuildTool
    {
        public delegate void PostBuildCallback(string finishedAabFilePath);

        /// <summary>
        /// Status for coordinating between the background thread and main thread on async builds.
        /// </summary>
        private enum BuildStatus
        {
            Running,
            Failing,
            Succeeding,
            Halted
        }

        private const string AndroidManifestFileName = "AndroidManifest.xml";
        private const string AssetsDirectoryName = "assets";
        private const string BundleMetadataDirectoryName = "BUNDLE-METADATA";
        private const string DexFileExtensionOrDirectoryName = "dex";
        private const string ManifestDirectoryName = "manifest";
        private const string ResourceTableFileName = "resources.pb";
        private const string RootDirectoryName = "root";
        private const string CreatingBaseModuleMessage = "Creating base module";
        private const float ProgressCreateBaseModule = 0.3f;
        private const float ProgressProcessModules = 0.5f;
        private const float ProgressRunBundletool = 0.7f;
        private const int ProgressBarWaitHandleTimeoutMs = 100;

        /// <summary>
        /// The folder where to store the asset pack, inside the "assets" folder of an Android App Bundle module.
        /// This intermediate folder name can be suffixed with a texture compression format targeting (e.g: #tcf_astc),
        /// which will be stripped out by bundletool.
        /// </summary>
        private const string AssetPackFolder = "assetpack";

        private readonly AndroidAssetPackagingTool _androidAssetPackagingTool;
        private readonly AndroidBuilder _androidBuilder;
        private readonly BundletoolHelper _bundletool;
        private readonly JarSigner _jarSigner;
        private readonly string _workingDirectoryPath;
        private readonly ZipUtils _zipUtils;

        private volatile BuildStatus _buildStatus = BuildStatus.Running;
        private volatile string _progressBarMessage = "";
        private volatile float _progressBarProgress;
        private volatile string _buildErrorMessage;
        private volatile string _finishedAabFilePath;
        private Thread _backgroundThread;
        private EventWaitHandle _progressBarWaitHandle;
        private bool _isGradleBuild;
        private AndroidSdkVersions _minSdkVersion;
        private string _packageName;
        private PostBuildCallback _createBundleAsyncOnSuccess = delegate { };
        private IEnumerable<IAssetPackManifestTransformer> _assetPackManifestTransformers;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AppBundleBuilder(
            AndroidAssetPackagingTool androidAssetPackagingTool,
            AndroidBuilder androidBuilder,
            BundletoolHelper bundletool,
            JarSigner jarSigner,
            string workingDirectoryPath,
            ZipUtils zipUtils)
        {
            _androidAssetPackagingTool = androidAssetPackagingTool;
            _androidBuilder = androidBuilder;
            _bundletool = bundletool;
            _jarSigner = jarSigner;
            _workingDirectoryPath = workingDirectoryPath;
            _zipUtils = zipUtils;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            _isGradleBuild = EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.Gradle;
            // Cache minSdkVersion because PlayerSettings.Android is only accessible from the main thread.
            _minSdkVersion = PlayerSettings.Android.minSdkVersion;
            _packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);

            _assetPackManifestTransformers = AssetPackManifestTransformerRegistry.Registry.ConstructInstances();
            var initializedManifestTransformers = true;
            foreach (var transformer in _assetPackManifestTransformers)
            {
                initializedManifestTransformers &= transformer.Initialize(buildToolLogger);
            }

            return initializedManifestTransformers
                   && _androidAssetPackagingTool.Initialize(buildToolLogger)
                   && _androidBuilder.Initialize(buildToolLogger)
                   && _jarSigner.Initialize(buildToolLogger)
                   && _bundletool.Initialize(buildToolLogger)
                   && _zipUtils.Initialize(buildToolLogger);
        }

        public string WorkingDirectoryPath
        {
            get { return _workingDirectoryPath; }
        }

        /// <summary>
        /// Builds an Android Player with the specified options.
        /// Note: the specified <see cref="BuildPlayerOptions.locationPathName"/> field is ignored and the
        /// Android Player is written to a temporary file whose path is returned as a string.
        /// </summary>
        /// <returns>The path to the file if the build succeeded, or null if it failed or was cancelled.</returns>
        public virtual string BuildAndroidPlayer(BuildPlayerOptions buildPlayerOptions)
        {
            var workingDirectory = new DirectoryInfo(_workingDirectoryPath);
            if (workingDirectory.Exists)
            {
                workingDirectory.Delete(true);
            }

            workingDirectory.Create();

            if (UseNativeAppBundleSupport)
            {
                AndroidAppBundle.EnableNativeBuild();
            }

            var androidPlayerFilePath = Path.Combine(workingDirectory.FullName, "tmp.aab");
            Debug.LogFormat("Building Android Player: {0}", androidPlayerFilePath);
            // This Android Player is an intermediate build artifact, so use a temporary path for the output file path.
            var updatedBuildPlayerOptions = new BuildPlayerOptions
            {
                assetBundleManifestPath = buildPlayerOptions.assetBundleManifestPath,
                locationPathName = androidPlayerFilePath,
                options = buildPlayerOptions.options,
                scenes = buildPlayerOptions.scenes,
                target = buildPlayerOptions.target,
                targetGroup = buildPlayerOptions.targetGroup
            };

            if (EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.Gradle)
            {
                EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            }

            // Do not use BuildAndSign since this signature won't be used.
            return _androidBuilder.Build(updatedBuildPlayerOptions) ? androidPlayerFilePath : null;
        }

        /// <summary>
        /// Synchronously builds an Android App Bundle at the specified path using the specified Android Player.
        /// </summary>
        /// <returns>True if the build succeeded, false if it failed or was cancelled.</returns>
        public bool CreateBundle(string aabFilePath, string androidPlayerFilePath, AssetPackConfig assetPackConfig)
        {
            if (_buildStatus != BuildStatus.Running)
            {
                throw new Exception("Unexpected call to CreateBundle() with status: " + _buildStatus);
            }

            var moduleDirectoryList = new List<DirectoryInfo>();
            var workingDirectory = new DirectoryInfo(_workingDirectoryPath);
            var configParams = new BundletoolHelper.BuildBundleConfigParams
            {
                defaultTcfSuffix = TextureTargetingTools.GetBundleToolTextureCompressionFormatName(
                    assetPackConfig.DefaultTextureCompressionFormat),
                minSdkVersion = _minSdkVersion
            };

            // Create asset pack module directories.
            var index = 0;
            var assetPacks = assetPackConfig.DeliveredAssetPacks;
            foreach (var entry in assetPacks)
            {
                DisplayProgress(
                    string.Format("Processing asset pack {0} of {1}", index + 1, assetPacks.Count),
                    Mathf.Lerp(0.1f, ProgressCreateBaseModule, (float) index / assetPacks.Count));
                index++;

                var assetPackName = entry.Key;
                var assetPack = entry.Value;
                configParams.enableTcfTargeting |= assetPack.CompressionFormatToAssetBundleFilePath != null;
                configParams.enableTcfTargeting |= assetPack.CompressionFormatToAssetPackDirectoryPath != null;
                configParams.containsInstallTimeAssetPack |=
                    assetPack.DeliveryMode == AssetPackDeliveryMode.InstallTime;

                var assetPackDirectoryInfo = workingDirectory.CreateSubdirectory(assetPackName);
                if (!CreateAssetPackModule(assetPackName, assetPack, assetPackDirectoryInfo))
                {
                    return false;
                }

                moduleDirectoryList.Add(assetPackDirectoryInfo);
            }

            // Create base module directory.
            var baseDirectory = workingDirectory.CreateSubdirectory(AndroidAppBundle.BaseModuleName);
            IList<string> bundleMetadata;
            if (!CreateBaseModule(baseDirectory, androidPlayerFilePath, out bundleMetadata))
            {
                return false;
            }

            moduleDirectoryList.Add(baseDirectory);

            // Create a ZIP file for each module directory.
            var moduleFiles = new List<string>();
            var numModules = moduleDirectoryList.Count;
            for (var i = 0; i < numModules; i++)
            {
                if (numModules == 1)
                {
                    DisplayProgress("Processing base module", ProgressProcessModules);
                }
                else
                {
                    DisplayProgress(
                        string.Format("Processing module {0} of {1}", i + 1, numModules),
                        Mathf.Lerp(ProgressProcessModules, ProgressRunBundletool, (float) i / numModules));
                }

                var moduleDirectoryInfo = moduleDirectoryList[i];
                var destinationDirectoryInfo = GetDestinationSubdirectory(moduleDirectoryInfo);

                // Create ZIP file path, for example /path/to/files/base becomes /path/to/files/base/base.zip
                var zipFilePath = Path.Combine(moduleDirectoryInfo.FullName, moduleDirectoryInfo.Name + ".zip");
                var zipErrorMessage = _zipUtils.CreateZipFile(zipFilePath, destinationDirectoryInfo.FullName, ".");
                if (zipErrorMessage != null)
                {
                    DisplayBuildError("Zip creation", zipErrorMessage);
                    return false;
                }

                moduleFiles.Add(zipFilePath);
            }

            DisplayProgress("Running bundletool", ProgressRunBundletool);
            var buildBundleErrorMessage =
                _bundletool.BuildBundle(aabFilePath, moduleFiles, bundleMetadata, configParams);
            if (buildBundleErrorMessage != null)
            {
                DisplayBuildError("bundletool", buildBundleErrorMessage);
                return false;
            }

            // Only sign the .aab if a custom keystore is configured.
            if (_jarSigner.UseCustomKeystore)
            {
                DisplayProgress("Signing bundle", 0.9f);
                var signingErrorMessage = _jarSigner.Sign(aabFilePath);
                if (signingErrorMessage != null)
                {
                    DisplayBuildError("Signing", signingErrorMessage);
                    return false;
                }
            }
            else
            {
                Debug.LogFormat("Skipped signing since a Custom Keystore isn't configured in Android Player Settings");
            }

            Debug.LogFormat("Finished building app bundle: {0}", aabFilePath);
            _finishedAabFilePath = aabFilePath;
            _buildStatus = BuildStatus.Succeeding;
            return true;
        }

        /// <summary>
        /// Asynchronously builds an Android App Bundle at the specified path using the specified Android Player.
        /// </summary>
        /// <param name="aabFilePath">Path to the AAB file that should be built.</param>
        /// <param name="androidPlayerFilePath">Path to an existing Android Player ZIP file.</param>
        /// <param name="assetPackConfig">Indicates asset packs to include in the AAB.</param>
        /// <param name="onSuccess">
        /// Callback that fires with the final aab file location, when the bundle creation succeeds.
        /// </param>
        public void CreateBundleAsync(string aabFilePath, string androidPlayerFilePath,
            AssetPackConfig assetPackConfig, PostBuildCallback onSuccess)
        {
            _progressBarWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            EditorApplication.update += HandleUpdate;
            _createBundleAsyncOnSuccess = onSuccess;
            _backgroundThread = new Thread(() =>
            {
                try
                {
                    CreateBundle(aabFilePath, androidPlayerFilePath, assetPackConfig);
                }
                catch (Exception ex)
                {
                    // Catch and display exceptions since they may otherwise be undetected on a background thread.
                    DisplayBuildError("Exception", ex.ToString());
                    throw;
                }
            });
            _backgroundThread.Name = "AppBundle";
            _backgroundThread.Start();
        }

        // Handler for EditorApplication.update callbacks on the main thread.
        private void HandleUpdate()
        {
            switch (_buildStatus)
            {
                case BuildStatus.Running:
                    // Checking for task cancellation during EditorApplication.update is much more responsive
                    // than only calling DisplayCancelableProgressBar() when the message changes.
                    if (EditorUtility.DisplayCancelableProgressBar(
                        "Building App Bundle", _progressBarMessage, _progressBarProgress))
                    {
                        Debug.Log("Cancelling app bundle build...");
                        EditorUtility.ClearProgressBar();
                        _backgroundThread.Abort();
                        _buildStatus = BuildStatus.Halted;
                    }

                    // Signal the background thread that we've handled an update since DisplayProgress() was called.
                    _progressBarWaitHandle.Set();
                    break;
                case BuildStatus.Failing:
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog(
                        BuildToolLogger.BuildErrorTitle, _buildErrorMessage, WindowUtils.OkButtonText);
                    _buildStatus = BuildStatus.Halted;
                    break;
                case BuildStatus.Succeeding:
                    EditorUtility.ClearProgressBar();
                    _buildStatus = BuildStatus.Halted;
                    _createBundleAsyncOnSuccess(_finishedAabFilePath);
                    break;
                case BuildStatus.Halted:
                    _progressBarWaitHandle.Close();
                    EditorApplication.update -= HandleUpdate;
                    break;
                default:
                    throw new Exception("Unexpected BuildStatus: " + _buildStatus);
            }
        }

        private bool CreateAssetPackModule(
            string assetPackName, AssetPack assetPack, DirectoryInfo assetPackDirectoryInfo)
        {
            var androidManifestXmlPath = Path.Combine(assetPackDirectoryInfo.FullName, AndroidManifestFileName);
            var assetPackManifestXDocument =
                CreateAssetPackManifestXDocument(assetPackName, assetPack.DeliveryMode);
            assetPackManifestXDocument.Save(androidManifestXmlPath);

            // Use aapt2 link to make a bundletool compatible module
            var sourceDirectoryInfo = assetPackDirectoryInfo.CreateSubdirectory("source");
            var aaptErrorMessage =
                _androidAssetPackagingTool.Link(androidManifestXmlPath, sourceDirectoryInfo.FullName);
            if (aaptErrorMessage != null)
            {
                DisplayBuildError("aapt2 link " + assetPackName, aaptErrorMessage);
                return false;
            }

            var destinationDirectoryInfo = GetDestinationSubdirectory(assetPackDirectoryInfo);
            ArrangeFilesForAssetPack(sourceDirectoryInfo, destinationDirectoryInfo);

            // Copy all assets to an "assets" subdirectory in the destination folder.
            var destinationAssetsDirectory = destinationDirectoryInfo.CreateSubdirectory(AssetsDirectoryName);
            if (assetPack.AssetBundleFilePath != null)
            {
                // Copy AssetBundle files into the module's "assets" folder, inside an "assetpack" folder.
                var outputDirectory = destinationAssetsDirectory.CreateSubdirectory(AssetPackFolder).FullName;
                File.Copy(assetPack.AssetBundleFilePath, Path.Combine(outputDirectory, assetPackName));
            }
            else if (assetPack.CompressionFormatToAssetBundleFilePath != null)
            {
                // Copy the AssetBundle files into the module's "assets" folder, inside an "assetpack#tcf_xxx" folder.
                foreach (var compressionAndFilePath in assetPack.CompressionFormatToAssetBundleFilePath)
                {
                    var targetedAssetsFolderName =
                        AssetPackFolder + TextureTargetingTools.GetTargetingSuffix(compressionAndFilePath.Key);
                    var outputDirectory = destinationAssetsDirectory.CreateSubdirectory(targetedAssetsFolderName);
                    File.Copy(compressionAndFilePath.Value, Path.Combine(outputDirectory.FullName, assetPackName));
                }
            }
            else if (assetPack.AssetPackDirectoryPath != null)
            {
                var sourceAssetsDirectory = new DirectoryInfo(assetPack.AssetPackDirectoryPath);
                if (!sourceAssetsDirectory.Exists)
                {
                    // TODO: check this earlier.
                    DisplayBuildError("Missing directory for " + assetPackName, sourceAssetsDirectory.FullName);
                    return false;
                }

                // Copy asset pack files into the module's "assets" folder, inside an "assetpack" folder.
                var outputDirectory = destinationAssetsDirectory.CreateSubdirectory(AssetPackFolder);
                CopyFilesRecursively(sourceAssetsDirectory, outputDirectory);
            }
            else if (assetPack.CompressionFormatToAssetPackDirectoryPath != null)
            {
                // Copy asset pack files into the module's "assets" folder, inside an "assetpack#tcf_xxx" folder.
                foreach (var compressionAndDirectoryPath in assetPack.CompressionFormatToAssetPackDirectoryPath)
                {
                    var sourceAssetsDirectory = new DirectoryInfo(compressionAndDirectoryPath.Value);
                    if (!sourceAssetsDirectory.Exists)
                    {
                        // TODO: check this earlier.
                        DisplayBuildError("Missing directory for " + assetPackName, sourceAssetsDirectory.FullName);
                        return false;
                    }

                    var targetedAssetsFolderName =
                        AssetPackFolder + TextureTargetingTools.GetTargetingSuffix(compressionAndDirectoryPath.Key);
                    var outputDirectory = destinationAssetsDirectory.CreateSubdirectory(targetedAssetsFolderName);
                    CopyFilesRecursively(sourceAssetsDirectory, outputDirectory);
                }
            }
            else
            {
                throw new InvalidOperationException("Missing asset pack files: " + assetPackName);
            }

            return true;
        }

        private static void CopyFilesRecursively(DirectoryInfo sourceDirectory, DirectoryInfo destinationDirectory)
        {
            foreach (var sourceSubdirectory in sourceDirectory.GetDirectories())
            {
                var destinationSubdirectory = destinationDirectory.CreateSubdirectory(sourceSubdirectory.Name);
                CopyFilesRecursively(sourceSubdirectory, destinationSubdirectory);
            }

            foreach (var file in sourceDirectory.GetFiles())
            {
                file.CopyTo(Path.Combine(destinationDirectory.FullName, file.Name), false);
            }
        }

        private XDocument CreateAssetPackManifestXDocument(string packName, AssetPackDeliveryMode deliveryMode)
        {
            if (_assetPackManifestTransformers == null)
            {
                throw new BuildToolNotInitializedException(this);
            }

            var doc = AssetPackManifestHelper.CreateAssetPackManifestXDocument(
                _packageName,
                packName,
                deliveryMode);
            foreach (var transformer in _assetPackManifestTransformers)
            {
                var error = transformer.Transform(doc);
                if (!string.IsNullOrEmpty(error))
                {
                    DisplayBuildError("AndroidManifest configuration", error);
                    break;
                }
            }

            return doc;
        }

        private bool CreateBaseModule(
            DirectoryInfo baseWorkingDirectory, string androidPlayerFilePath, out IList<string> bundleMetadata)
        {
            string zipFilePath;
            var sourceDirectoryInfo = baseWorkingDirectory.CreateSubdirectory("source");
            if (UseNativeAppBundleSupport)
            {
                DisplayProgress(CreatingBaseModuleMessage, ProgressCreateBaseModule);
                zipFilePath = androidPlayerFilePath;
            }
            else
            {
                // If this version of Unity natively supports AABs, note that switching the Android Build System
                // to Gradle will generally speed up the build due to the slow "aapt2 convert" step.
                var messageSuffix =
                    SuggestNativeAppBundleSupport ? " (switch to Gradle for faster builds)" : " (this may be slow)";
                DisplayProgress(CreatingBaseModuleMessage + messageSuffix, ProgressCreateBaseModule);
                zipFilePath = Path.Combine(baseWorkingDirectory.FullName, "AndroidPlayer.zip");

                var aaptErrorMessage = _androidAssetPackagingTool.Convert(androidPlayerFilePath, zipFilePath);
                if (aaptErrorMessage != null)
                {
                    DisplayBuildError("aapt2 convert", aaptErrorMessage);
                    bundleMetadata = null;
                    return false;
                }

                DisplayProgress("Extracting base module", 0.45f);
            }

            var unzipErrorMessage = _zipUtils.UnzipFile(zipFilePath, sourceDirectoryInfo.FullName);
            if (unzipErrorMessage != null)
            {
                DisplayBuildError("Unzip", unzipErrorMessage);
                bundleMetadata = null;
                return false;
            }

            bundleMetadata = GetExistingBundleMetadata(sourceDirectoryInfo);

            var destinationDirectoryInfo = GetDestinationSubdirectory(baseWorkingDirectory);
            if (UseNativeAppBundleSupport)
            {
                var baseModuleDirectories = sourceDirectoryInfo.GetDirectories(AndroidAppBundle.BaseModuleName);
                if (baseModuleDirectories.Length != 1)
                {
                    DisplayBuildError("Find base directory", sourceDirectoryInfo.FullName);
                    return false;
                }

                ArrangeFilesForExistingModule(baseModuleDirectories[0], destinationDirectoryInfo);
            }
            else
            {
                ArrangeFilesForNewModule(sourceDirectoryInfo, destinationDirectoryInfo);
            }

            return true;
        }

        private bool UseNativeAppBundleSupport
        {
            get { return _isGradleBuild && AndroidAppBundle.HasNativeBuildSupport(); }
        }

        private bool SuggestNativeAppBundleSupport
        {
            get { return !_isGradleBuild && AndroidAppBundle.HasNativeBuildSupport(); }
        }

        private static void ArrangeFilesForAssetPack(DirectoryInfo source, DirectoryInfo destination)
        {
            // aapt2 link creates an empty resource table even though asset packs have no resources.
            // Bundletool fails if the asset pack has a resources.pb. Only retain the AndroidManifest.xml file.
            foreach (var sourceFileInfo in source.GetFiles())
            {
                if (sourceFileInfo.Name == AndroidManifestFileName)
                {
                    var destinationSubdirectory = destination.CreateSubdirectory(ManifestDirectoryName);
                    sourceFileInfo.MoveTo(Path.Combine(destinationSubdirectory.FullName, sourceFileInfo.Name));
                }
            }
        }

        private static void ArrangeFilesForExistingModule(DirectoryInfo source, DirectoryInfo destination)
        {
            // Only include the resources.pb file located directly in the source directory.
            foreach (var sourceFileInfo in source.GetFiles())
            {
                if (sourceFileInfo.Name == ResourceTableFileName)
                {
                    sourceFileInfo.MoveTo(Path.Combine(destination.FullName, ResourceTableFileName));
                }
            }

            // Include all directories located in the source directory.
            foreach (var sourceDirectoryInfo in source.GetDirectories())
            {
                sourceDirectoryInfo.MoveTo(Path.Combine(destination.FullName, sourceDirectoryInfo.Name));
            }
        }

        private static void ArrangeFilesForNewModule(DirectoryInfo source, DirectoryInfo destination)
        {
            // Arrange files according to https://developer.android.com/guide/app-bundle/#aab_format
            foreach (var sourceFileInfo in source.GetFiles())
            {
                DirectoryInfo destinationSubdirectory;
                var fileName = sourceFileInfo.Name;
                if (fileName == AndroidManifestFileName)
                {
                    destinationSubdirectory = destination.CreateSubdirectory(ManifestDirectoryName);
                }
                else if (fileName == ResourceTableFileName)
                {
                    destinationSubdirectory = destination;
                }
                else if (fileName.EndsWith(DexFileExtensionOrDirectoryName))
                {
                    destinationSubdirectory = destination.CreateSubdirectory(DexFileExtensionOrDirectoryName);
                }
                else
                {
                    destinationSubdirectory = destination.CreateSubdirectory(RootDirectoryName);
                }

                sourceFileInfo.MoveTo(Path.Combine(destinationSubdirectory.FullName, fileName));
            }

            foreach (var sourceDirectoryInfo in source.GetDirectories())
            {
                var directoryName = sourceDirectoryInfo.Name;
                switch (directoryName)
                {
                    case "META-INF":
                        // Skip files like MANIFEST.MF
                        break;
                    case AssetsDirectoryName:
                    case "lib":
                    case "res":
                        sourceDirectoryInfo.MoveTo(Path.Combine(destination.FullName, directoryName));
                        break;
                    default:
                        var subdirectory = destination.CreateSubdirectory(RootDirectoryName);
                        sourceDirectoryInfo.MoveTo(Path.Combine(subdirectory.FullName, directoryName));
                        break;
                }
            }
        }

        /// <summary>
        /// Given the root directory of an existing AAB, return a list of all bundle metadata files in the specific
        /// format expected by bundletool.
        /// </summary>
        private IList<string> GetExistingBundleMetadata(DirectoryInfo rootDirectoryInfo)
        {
            var bundleMetadata = new List<string>();
            var bundleMetadataDirectories = rootDirectoryInfo.GetDirectories(BundleMetadataDirectoryName);
            if (bundleMetadataDirectories.Length != 1)
            {
                return bundleMetadata;
            }

            // The metadata files are usually one directory deep, but use Directory.GetFiles() since this isn't a given.
            var bundleMetadataPath = bundleMetadataDirectories[0].FullName;
            var metadataFiles = Directory.GetFiles(bundleMetadataPath, "*", SearchOption.AllDirectories);
            foreach (var physicalFile in metadataFiles)
            {
                if (physicalFile.Substring(0, bundleMetadataPath.Length) != bundleMetadataPath)
                {
                    throw new InvalidOperationException(
                        "A bundle metadata file doesn't match the expected parent path: " + physicalFile);
                }

                var startIndex = bundleMetadataPath.Length + 1;
                var relativePath = physicalFile.Substring(startIndex, physicalFile.Length - startIndex);
                // Expected format is "<bundle-path>:<physical-file>". "<bundle-path>" uses "/" directory separators.
                var bundlePath = relativePath.Replace("\\", "/");
                bundleMetadata.Add(string.Format("{0}:{1}", bundlePath, physicalFile));
            }

            return bundleMetadata;
        }

        private static DirectoryInfo GetDestinationSubdirectory(DirectoryInfo directoryInfo)
        {
            return directoryInfo.CreateSubdirectory("destination");
        }

        private void DisplayProgress(string info, float progress)
        {
            Debug.LogFormat("{0}...", info);
            if (_backgroundThread == null)
            {
                // Running synchronously in batch mode.
                return;
            }

            _progressBarMessage = info;
            _progressBarProgress = progress;

            // Wait for the main thread to display the message in at least one callback of the update handler.
            // Give up after a short timeout since this is best effort, and we don't want to block indefinitely.
            _progressBarWaitHandle.WaitOne(ProgressBarWaitHandleTimeoutMs);
        }

        private void DisplayBuildError(string errorType, string errorMessage)
        {
            if (_buildStatus == BuildStatus.Halted)
            {
                // Ignore any errors after we've halted, e.g. if the thread abort causes an exception to be thrown.
                return;
            }

            _buildStatus = BuildStatus.Failing;
            _buildErrorMessage = string.Format("{0} failed: {1}", errorType, errorMessage);
            Debug.LogError(_buildErrorMessage);
        }
    }
}