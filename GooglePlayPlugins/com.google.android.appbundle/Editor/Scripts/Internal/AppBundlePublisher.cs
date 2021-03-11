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
using System.IO;
using Google.Android.AppBundle.Editor.AssetPacks;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using Google.Android.AppBundle.Editor.Internal.Config;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Helper to build <a href="https://developer.android.com/platform/technology/app-bundle/">Android App Bundles</a>.
    /// </summary>
    public static class AppBundlePublisher
    {
        [Serializable]
        private class AppBundleBuildSettings
        {
            public string aabFilePath;
            public bool requirePrerequisiteChecks;
            public bool runOnDevice;
        }

#if !UNITY_2018_3_OR_NEWER
        /// <summary>
        /// A <see cref="PostBuildTask"/> that serializes state to resume an app bundle build after a script reload
        /// occurs. Starting with Unity 2018.3 player builds no longer reset the application domain.
        /// </summary>
        [Serializable]
        private class AppBundlePostBuildTask : PostBuildTask
        {
            public AppBundleBuildSettings buildSettings;
            public SerializableAssetPackConfig serializableAssetPackConfig;
            public string workingDirectoryPath;

            public override void RunPostBuildTask()
            {
                var buildToolLogger = new BuildToolLogger();
                var assetPackConfig = SerializationHelper.Deserialize(serializableAssetPackConfig);
                var appBundleBuilder = CreateAppBundleBuilder(workingDirectoryPath);
                if (appBundleBuilder.Initialize(buildToolLogger))
                {
                    CreateBundleAsync(appBundleBuilder, buildSettings, assetPackConfig);
                }
                else
                {
                    buildToolLogger.DisplayErrorDialog("Failed to initialize AppBundleBuilder in post-build task.");
                }
            }
        }
#endif

        private static IBuildModeProvider _buildModeProvider;

        /// <summary>
        /// Builds an Android App Bundle at a location specified in a file save dialog.
        /// </summary>
        public static void Build()
        {
            // Check that we're ready to build before displaying the file save dialog.
            var appBundleBuilder = CreateAppBundleBuilder();
            if (!appBundleBuilder.Initialize(new BuildToolLogger()))
            {
                return;
            }

            var aabFilePath = EditorUtility.SaveFilePanel("Create Android App Bundle", null, null, "aab");
            if (string.IsNullOrEmpty(aabFilePath))
            {
                // Assume cancelled.
                return;
            }

            var buildSettings = new AppBundleBuildSettings
            {
                requirePrerequisiteChecks = false,
                runOnDevice = false
            };

            var buildPlayerOptions = AndroidBuildHelper.CreateBuildPlayerOptions(aabFilePath);
            var assetPackConfig = AssetPackConfigSerializer.LoadConfig();
            Build(appBundleBuilder, buildPlayerOptions, assetPackConfig, buildSettings);
        }

        /// <summary>
        /// Builds an Android App Bundle to a temp directory and then runs it on device.
        /// </summary>
        public static void BuildAndRun()
        {
            var buildSettings = new AppBundleBuildSettings
            {
                requirePrerequisiteChecks = true,
                runOnDevice = true
            };

            var appBundleBuilder = CreateAppBundleBuilder();
            var tempOutputFilePath = Path.Combine(appBundleBuilder.WorkingDirectoryPath, "temp.aab");
            var buildPlayerOptions = AndroidBuildHelper.CreateBuildPlayerOptions(tempOutputFilePath);
            var assetPackConfig = AssetPackConfigSerializer.LoadConfig();
            Build(appBundleBuilder, buildPlayerOptions, assetPackConfig, buildSettings);
        }

        /// <summary>
        /// Builds an Android App Bundle given the specified <see cref="BuildPlayerOptions"/>.
        /// </summary>
        /// <returns>True if the build succeeded, false if it failed or was cancelled.</returns>
        public static bool Build(BuildPlayerOptions buildPlayerOptions, AssetPackConfig assetPackConfig)
        {
            var buildSettings = new AppBundleBuildSettings
            {
                requirePrerequisiteChecks = true,
                runOnDevice = false
            };

            var appBundleBuilder = CreateAppBundleBuilder();
            return Build(appBundleBuilder, buildPlayerOptions, assetPackConfig, buildSettings);
        }

        /// <summary>
        /// Sets the current BuildModeProvider. Should be called in a class annotated with "InitializeOnLoad" in order
        /// for it to be assigned after assembly reload.
        /// </summary>
        public static void SetBuildModeProvider(IBuildModeProvider buildModeProvider)
        {
            _buildModeProvider = buildModeProvider;
        }

        private static bool Build(AppBundleBuilder appBundleBuilder, BuildPlayerOptions buildPlayerOptions,
            AssetPackConfig assetPackConfig, AppBundleBuildSettings buildSettings)
        {
            if (buildSettings.requirePrerequisiteChecks && !appBundleBuilder.Initialize(new BuildToolLogger()))
            {
                return false;
            }

            buildSettings.aabFilePath = buildPlayerOptions.locationPathName;
            Debug.LogFormat("Building app bundle: {0}", buildSettings.aabFilePath);
            bool buildSucceeded = appBundleBuilder.BuildAndroidPlayer(buildPlayerOptions);
            if (!buildSucceeded)
            {
                return false;
            }

            if (IsBatchMode)
            {
                return appBundleBuilder.CreateBundle(buildSettings.aabFilePath, assetPackConfig);
            }

#if UNITY_2018_3_OR_NEWER
            CreateBundleAsync(appBundleBuilder, buildSettings, assetPackConfig);
#else
            var task = new AppBundlePostBuildTask
            {
                buildSettings = buildSettings,
                serializableAssetPackConfig = SerializationHelper.Serialize(assetPackConfig),
                workingDirectoryPath = appBundleBuilder.WorkingDirectoryPath
            };
            PostBuildRunner.RunTask(task);
#endif
            return true;
        }

        private static void CreateBundleAsync(
            AppBundleBuilder appBundleBuilder, AppBundleBuildSettings buildSettings, AssetPackConfig assetPackConfig)
        {
            var callback =
                buildSettings.runOnDevice
                    ? (AppBundleBuilder.PostBuildCallback) RunBundle
                    : EditorUtility.RevealInFinder;
            appBundleBuilder.CreateBundleAsync(buildSettings.aabFilePath, assetPackConfig, callback);
        }

        private static void RunBundle(string aabFile)
        {
            var buildToolLogger = new BuildToolLogger();
            var appBundleRunner = CreateAppBundleRunner();
            if (!appBundleRunner.Initialize(buildToolLogger))
            {
                buildToolLogger.DisplayErrorDialog("Failed to initialize AppBundleRunner.");
                return;
            }

            appBundleRunner.RunBundle(aabFile, GetBuildMode());
        }

        private static AppBundleBuilder CreateAppBundleBuilder()
        {
            return CreateAppBundleBuilder(GetTempFolder());
        }

        private static AppBundleBuilder CreateAppBundleBuilder(string workingDirectoryPath)
        {
            var androidSdk = new AndroidSdk();
            var javaUtils = new JavaUtils();
            var androidSdkPlatform = new AndroidSdkPlatform(androidSdk);
            var androidBuildTools = new AndroidBuildTools(androidSdk);
            var apkSigner = new ApkSigner(androidBuildTools, javaUtils);
            var jarSigner = new JarSigner(javaUtils);
            return new AppBundleBuilder(
                new AndroidAssetPackagingTool(androidBuildTools, androidSdkPlatform),
                new AndroidBuilder(androidSdkPlatform, apkSigner),
                new BundletoolHelper(javaUtils),
                jarSigner,
                workingDirectoryPath,
                new ZipUtils(javaUtils));
        }

        private static AppBundleRunner CreateAppBundleRunner()
        {
            var androidSdk = new AndroidSdk();
            var javaUtils = new JavaUtils();
            var adb = new AndroidDebugBridge(androidSdk);
            var bundletool = new BundletoolHelper(javaUtils);
            return new AppBundleRunner(adb, bundletool);
        }

        private static string GetTempFolder()
        {
            // Create a new temp folder with each build. Some developers prefer a random path here since there may be
            // multiple builds running concurrently, e.g. on an automated build machine. See Issue #69.
            // Note: this plugin doesn't clear out old temporary build folders, so disk usage will grow over time.
            // Note: we use the 2 argument Path.Combine() to support .NET 3.
            return Path.Combine(Path.Combine(Path.GetTempPath(), "play-unity-build"), Path.GetRandomFileName());
        }

        private static BundletoolBuildMode GetBuildMode()
        {
            if (_buildModeProvider != null)
            {
                return _buildModeProvider.GetBuildMode();
            }

            return BundletoolBuildMode.Persistent;
        }

        private static bool IsBatchMode
        {
            get
            {
#if UNITY_2018_2_OR_NEWER
                return Application.isBatchMode;
#else
                return Environment.CommandLine.Contains("-batchmode");
#endif
            }
        }
    }
}