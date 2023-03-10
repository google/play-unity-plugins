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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.Android.AppBundle.Editor.Internal.AssetPacks;
using Google.Android.AppBundle.Editor.Internal.PlayServices;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Provides methods for <a href="https://developer.android.com/studio/command-line/bundletool">bundletool</a>.
    /// </summary>
    public class BundletoolHelper : IBuildTool
    {
        /// <summary>
        /// Contains parameters needed to generate a config file for the build-bundle command.
        /// </summary>
        public class BuildBundleConfigParams
        {
            /// <summary>
            /// If true, enables targeting of module contents by texture formats.
            /// </summary>
            public bool enableTcfTargeting;

            /// <summary>
            /// When targeting by texture format, specifies the default format that will be used to generate
            /// standalone APKs for Android pre-Lollipop devices that don't support split APKs.
            /// </summary>
            public string defaultTcfSuffix;

            /// <summary>
            /// If true, enables targeting of module contents by device tiers.
            /// </summary>
            public bool enableDeviceTierTargeting;

            /// <summary>
            /// When targeting by device tier, specifies the default device tier that will be used to generate
            /// standalone APKs for Android pre-Lollipop devices that don't support split APKs.
            /// If not specified, it defaults to "0".
            /// </summary>
            public string defaultDeviceTier = "0";


            /// <summary>
            /// Whether or not this bundle contains an install-time asset pack.
            /// </summary>
            public bool containsInstallTimeAssetPack;

            /// <summary>
            /// Minimum Android SDK version, e.g. from PlayerSettings.Android.
            /// </summary>
            public AndroidSdkVersions minSdkVersion;

            /// <summary>
            /// Options for overriding the default file compression policies.
            /// </summary>
            public CompressionOptions compressionOptions;

            /// <summary>
            /// Options for configuring asset only app bundles.
            /// </summary>
            public AssetOnlyOptions assetOnlyOptions;
        }

        // Paths where the bundletool jar may potentially be found.
        private const string PackagePath = "com.google.android.appbundle/Editor/Tools/bundletool-all.jar";
        private const string PackagesPath = "Packages/" + PackagePath;
        private const string PluginPath = "Assets/GooglePlayPlugins/" + PackagePath;
        private const string OverridePath = "bundletool-all.jar";

        /// <summary>
        /// List of glob patterns specifying files that Unity requires be left uncompressed.
        /// Similar to PlaybackEngines/AndroidPlayer/Tools/GradleTemplates/mainTemplate.gradle
        /// </summary>
        private static readonly string[] UnityUncompressedGlob =
            { "assets/**/*.unity3d", "assets/**/*.ress", "assets/**/*.resource" };

        /// <summary>
        /// Make the Bundle Config exported as JSON cleaner by removing the suffix stripping fields
        /// that are not enabled.
        /// This also fix an issue with bundletool v0.11.0 which does not consider the config as valid if
        /// a suffix stripping field is defined (even if not enabled) for any dimension other than texture compression
        /// format.
        /// </summary>
        /// <param name="configJson">The configuration serialized to a JSON string.</param>
        /// <returns>The cleaned JSON string.</returns>
        public static string CleanDisabledSuffixStripping(string configJson)
        {
            return configJson.Replace(",\"suffixStripping\":{\"enabled\":false,\"defaultSuffix\":\"\"}", "");
        }

        /// <summary>
        /// BundleTool config optimized for Unity-based apps.
        /// </summary>
        public static BundletoolConfig.Config MakeConfig(
            BuildBundleConfigParams configParams, string streamingAssetsPath)
        {
            var config = new BundletoolConfig.Config();

            // APK download size is smaller when native libraries are uncompressed. uncompressNativeLibraries sets
            // android:extractNativeLibs="false" in the manifest, which also reduces on-disk for Android 6.0+ devices.
            config.optimizations.uncompressNativeLibraries.enabled = true;
            config.compression.uncompressedGlob.AddRange(UnityUncompressedGlob);

            var compressionOptions = configParams.compressionOptions;
            if (compressionOptions.UncompressedGlobs != null)
            {
                config.compression.uncompressedGlob.AddRange(compressionOptions.UncompressedGlobs);
            }

            if (!compressionOptions.CompressStreamingAssets)
            {
                config.compression.uncompressedGlob.AddRange(GetStreamingAssetsFileGlobs(streamingAssetsPath));
            }

            if (compressionOptions.CompressInstallTimeAssetPacks)
            {
                config.compression.installTimeAssetModuleDefaultCompression = BundletoolConfig.Compressed;
            }

            var dimensions = config.optimizations.splitsConfig.splitDimension;
            // Split on ABI so only one set of native libraries (armeabi-v7a, arm64-v8a, or x86) is sent to a device.
            dimensions.Add(new BundletoolConfig.SplitDimension { value = BundletoolConfig.Abi, negate = false });
            // Do not split on LANGUAGE since Unity games don't store localized strings in the typical Android manner.
            dimensions.Add(new BundletoolConfig.SplitDimension { value = BundletoolConfig.Language, negate = true });
            // Do not split on SCREEN_DENSITY since Unity games don't have per-density resources other than app icons.
            dimensions.Add(
                new BundletoolConfig.SplitDimension { value = BundletoolConfig.ScreenDensity, negate = true });
            if (configParams.enableTcfTargeting)
            {
                dimensions.Add(new BundletoolConfig.SplitDimension
                {
                    value = BundletoolConfig.TextureCompressionFormat,
                    negate = false,
                    suffixStripping =
                    {
                        enabled = true,
                        defaultSuffix = configParams.defaultTcfSuffix
                    }
                });
            }

            if (configParams.enableDeviceTierTargeting)
            {
                dimensions.Add(new BundletoolConfig.SplitDimension
                {
                    value = BundletoolConfig.DeviceTier,
                    negate = false,
                    suffixStripping =
                    {
                        enabled = true,
                        defaultSuffix = configParams.defaultDeviceTier
                    }
                });
            }


            if (configParams.assetOnlyOptions != null)
            {
                config.type = BundletoolConfig.AssetOnly;
                config.asset_modules_config = new BundletoolConfig.AssetModulesConfig
                {
                    app_version = new List<long>(configParams.assetOnlyOptions.AppVersions),
                    asset_version_tag = configParams.assetOnlyOptions.AssetVersionTag
                };
                return config;
            }

            // Bundletool requires the below standaloneConfig when supporting install-time asset packs for pre-Lollipop.
            if (configParams.containsInstallTimeAssetPack &&
                TextureTargetingTools.IsSdkVersionPreLollipop(configParams.minSdkVersion))
            {
                config.optimizations.standaloneConfig.splitDimension.Add(new BundletoolConfig.SplitDimension
                    { value = BundletoolConfig.Abi, negate = true });

                config.optimizations.standaloneConfig.splitDimension.Add(new BundletoolConfig.SplitDimension
                    { value = BundletoolConfig.Language, negate = true });

                config.optimizations.standaloneConfig.splitDimension.Add(new BundletoolConfig.SplitDimension
                    { value = BundletoolConfig.ScreenDensity, negate = true });

                config.optimizations.standaloneConfig.splitDimension.Add(new BundletoolConfig.SplitDimension
                    { value = BundletoolConfig.TextureCompressionFormat, negate = true });

                config.optimizations.standaloneConfig.strip64BitLibraries = true;
            }

            return config;
        }

        /// <summary>
        /// Searches the streaming assets path and returns a list of globs that includes the contained files relative to
        /// their final location within the APK.
        /// Note: Does not include .meta files.
        /// Visible for testing.
        /// </summary>
        public static IEnumerable<string> GetStreamingAssetsFileGlobs(string streamingAssetsPath)
        {
            if (!Directory.Exists(streamingAssetsPath))
            {
                return new List<string>();
            }

            var streamingAssets = new DirectoryInfo(streamingAssetsPath);

            // Create a glob for every subdirectory in the streaming assets path.
            // This is more efficient than returning every file from each of the subdirectories.
            var directoryGlobs = streamingAssets.GetDirectories("*", SearchOption.TopDirectoryOnly)
                .Select(directory => Path.Combine(directory.FullName, "**"));

            // Create a list of files that are located in the root of the streaming assets directory.
            var fileNames = streamingAssets.GetFiles("*", SearchOption.TopDirectoryOnly)
                .Where(file => !file.Name.EndsWith(".meta"))
                .Select(file => file.FullName);

            // Combine the directory glob list and file list and update the paths to be relative to the final file
            // locations within the APK.
            return directoryGlobs.Concat(fileNames)
                .Select(fullName => "assets/" + fullName.Remove(0, streamingAssetsPath.Length + 1))
                .Select(name => name.Replace("\\", "/")); // Support Windows' path separator.
        }


        private readonly JavaUtils _javaUtils;

        private string _streamingAssetsPath;
        private string _bundletoolJarPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        public BundletoolHelper(JavaUtils javaUtils)
        {
            _javaUtils = javaUtils;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            if (!_javaUtils.Initialize(buildToolLogger))
            {
                return false;
            }

            _streamingAssetsPath = Application.streamingAssetsPath;
            _bundletoolJarPath = BundletoolJarPath;
            if (_bundletoolJarPath == null)
            {
                buildToolLogger.DisplayErrorDialog("Failed to locate bundletool.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Builds an Android App Bundle at the specified location, overwriting an existing file if necessary. The
        /// bundle will contain the specified modules, with optional targeting done by texture compression format.
        /// </summary>
        /// <param name="outputFile">The output Android App Bundle (AAB) file.</param>
        /// <param name="moduleFiles">The modules to build inside the bundle.</param>
        /// <param name="metadataFiles">Metadata files to include in the bundle.</param>
        /// <param name="buildBundleConfigParams">Contains parameters needed to generate JSON for --config.</param>
        /// <returns>An error message if there was a problem running bundletool, or null if successful.</returns>
        public virtual string BuildBundle(string outputFile, IEnumerable<string> moduleFiles,
            IEnumerable<string> metadataFiles, BuildBundleConfigParams buildBundleConfigParams)
        {
            var bundleConfigJsonFile = Path.Combine(Path.GetTempPath(), "BundleConfig.json");
            var bundleConfig = MakeConfig(buildBundleConfigParams, _streamingAssetsPath);
            var bundleConfigJsonText = CleanDisabledSuffixStripping(JsonUtility.ToJson(bundleConfig));
            File.WriteAllText(bundleConfigJsonFile, bundleConfigJsonText);

            var metadataArgumentBuilder = new StringBuilder();
            foreach (var metadataFile in metadataFiles)
            {
                metadataArgumentBuilder.AppendFormat(" --metadata-file={0}", metadataFile);
            }

            // TODO: fix bundletool support for quoted paths around moduleFiles.
            return Run(
                "build-bundle --overwrite --config={0} --modules={1} --output={2}{3}",
                CommandLine.QuotePath(bundleConfigJsonFile),
                string.Join(",", moduleFiles.ToArray()),
                CommandLine.QuotePath(outputFile),
                metadataArgumentBuilder.ToString());
        }

        /// <summary>
        /// Builds an APK Set file from the specified Android App Bundle file.
        /// </summary>
        /// <param name="bundleFile">The output file from <see cref="BuildBundle"/>.</param>
        /// <param name="apkSetFile">A ZIP file containing APKs.</param>
        /// <param name="buildMode">The type of APKs to produce, such as "persistent" or "instant".</param>
        /// <param name="enableLocalTesting">
        /// Whether or not the --local-testing flag is enabled. This will change the behaviour of Play Asset Delivery so
        /// that fast-follow and on-demand packs are fetched from storage rather than downloaded.
        /// </param>
        /// <returns>An error message if there was a problem running bundletool, or null if successful.</returns>
        public virtual string BuildApkSet(
            string bundleFile, string apkSetFile, BundletoolBuildMode buildMode, bool enableLocalTesting)
        {
            return Run(
                "build-apks --bundle={0} --output={1} --mode={2}{3}",
                CommandLine.QuotePath(bundleFile),
                CommandLine.QuotePath(apkSetFile),
                buildMode.GetModeFlag(),
                enableLocalTesting ? " --local-testing" : "");
        }

        /// <summary>
        /// Installs the specified APK Set file to device.
        /// </summary>
        /// <param name="apkSetFile">A ZIP file containing APKs, produced from "bundletool build-apks".</param>
        /// <param name="adbPath">Path to the adb executable used to install apks.</param>
        /// <returns>An error message if there was a problem running bundletool, or null if successful.</returns>
        public virtual string InstallApkSet(string apkSetFile, string adbPath)
        {
            return Run("install-apks --adb={0} --apks={1}",
                CommandLine.QuotePath(adbPath),
                CommandLine.QuotePath(apkSetFile));
        }

        private string Run(string bundletoolCommand, params object[] args)
        {
            var bundletoolCommandWithArgs = string.Format(bundletoolCommand, args);
            var arguments =
                string.Format("-jar {0} {1}", CommandLine.QuotePath(_bundletoolJarPath), bundletoolCommandWithArgs);
            var result = CommandLine.Run(_javaUtils.JavaBinaryPath, arguments);
            return result.exitCode == 0 ? null : result.message;
        }

        /// <summary>
        /// Returns the absolute path to the bundletool jar packaged with the plugin.
        /// Returning the absolute path handles cases where Unity treats relative paths
        /// differently than the command line. For example Unity treats the Packages/
        /// folder as a symlink to Library/PackageCache while the command line does not.
        /// </summary>
        private static string BundletoolJarPath
        {
            get
            {
                // GUIDToAssetPath throws an exception if called on a non-main thread. To catch this case early, we
                // define this variable here, rather than below, where it is used.
                var guidPath = AssetDatabase.GUIDToAssetPath("c52291e63505c4121a167e6f0121c1b1");

                string relativePath = null;

                // Bundletool is normally included with the .unitypackage or UPM package, but the bundletool used for
                // builds can be overridden by placing a JAR in the project root, i.e. the parent folder of "Assets".
                if (File.Exists(OverridePath))
                {
                    relativePath = OverridePath;
                }
                else if (File.Exists(PackagesPath))
                {
                    relativePath = PackagesPath;
                }
                else if (File.Exists(PluginPath))
                {
                    relativePath = PluginPath;
                }
                else if (!string.IsNullOrEmpty(guidPath) && guidPath.EndsWith(".jar") && File.Exists(guidPath))
                {
                    relativePath = guidPath;
                }

                return relativePath == null ? null : Path.GetFullPath(relativePath);
            }
        }
    }
}