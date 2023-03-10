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
using UnityEditor;
#if !UNITY_2018_3_OR_NEWER
using Google.Android.AppBundle.Editor.Internal.Config;
#endif
#if UNITY_2018_2_OR_NEWER
using UnityEngine;
#endif
#if UNITY_2018_4_OR_NEWER && !NET_LEGACY
using System.Threading.Tasks;
#endif

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Helper to build <a href="https://developer.android.com/platform/technology/app-bundle/">Android App Bundles</a>.
    /// </summary>
    public static class AppBundlePublisher
    {
        private class AppBundleBuildSettings
        {
            public BuildPlayerOptions buildPlayerOptions;
            public AssetPackConfig assetPackConfig;
            public bool runOnDevice;
            public bool forceSynchronousBuild;
        }

#if !UNITY_2018_3_OR_NEWER
        /// <summary>
        /// A <see cref="PostBuildTask"/> that serializes state to resume an app bundle build after a script reload
        /// occurs. Starting with Unity 2018.3 player builds no longer reset the application domain.
        /// </summary>
        [Serializable]
        private class AppBundlePostBuildTask : PostBuildTask
        {
            public SerializableAssetPackConfig serializableAssetPackConfig;
            public string workingDirectoryPath;
            public string aabFilePath;
            public bool runOnDevice;

            public override void RunPostBuildTask()
            {
                var buildToolLogger = new BuildToolLogger();
                var appBundleBuilder = CreateAppBundleBuilder(workingDirectoryPath);
                if (!appBundleBuilder.Initialize(buildToolLogger))
                {
                    buildToolLogger.DisplayErrorDialog("Failed to initialize AppBundleBuilder in post-build task.");
                    return;
                }

                var assetPackConfig = SerializationHelper.Deserialize(serializableAssetPackConfig);
                CreateBundleAsync(appBundleBuilder, aabFilePath, assetPackConfig, runOnDevice);
            }
        }
#endif

        private static IBuildModeProvider _buildModeProvider;

#if UNITY_2018_4_OR_NEWER && !NET_LEGACY
        /// <summary>
        /// Builds an Android App Bundle given the specified configuration.
        /// </summary>
        public static async Task<AndroidBuildReport> BuildTask(AndroidBuildOptions androidBuildOptions)
        {
            var appBundleBuilder = CreateAppBundleBuilder();
            if (!appBundleBuilder.Initialize(new BuildToolLogger()))
            {
                throw new Exception("Failed to initialize AppBundleBuilder");
            }

            return await appBundleBuilder.CreateBundleWithTask(androidBuildOptions);
        }

        /// <summary>
        /// Builds an Android App Bundle containing only asset packs.
        /// </summary>
        public static async Task BuildAssetOnlyBundle(AssetOnlyBuildOptions assetOnlyBuildOptions)
        {
            var appBundleBuilder = CreateAppBundleBuilder();
            if (!appBundleBuilder.Initialize(new BuildToolLogger()))
            {
                throw new Exception("Failed to initialize AppBundleBuilder");
            }

            await appBundleBuilder.CreateAssetOnlyBundle(assetOnlyBuildOptions);
        }
#endif

        /// <summary>
        /// Builds an Android App Bundle given the specified configuration.
        /// </summary>
        public static bool Build(
            BuildPlayerOptions buildPlayerOptions, AssetPackConfig assetPackConfig, bool forceSynchronousBuild)
        {
            var appBundleBuilder = CreateAppBundleBuilder();
            if (!appBundleBuilder.Initialize(new BuildToolLogger()))
            {
                return false;
            }

            var buildSettings = new AppBundleBuildSettings
            {
                buildPlayerOptions = buildPlayerOptions,
                assetPackConfig = assetPackConfig ?? new AssetPackConfig(),
                forceSynchronousBuild = forceSynchronousBuild
            };
            return Build(appBundleBuilder, buildSettings);
        }

        /// <summary>
        /// Builds an Android App Bundle at a location specified in a file save dialog.
        /// The AAB will include asset packs configured in Asset Delivery Settings.
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
                buildPlayerOptions = AndroidBuildHelper.CreateBuildPlayerOptions(aabFilePath),
                assetPackConfig = AssetPackConfigSerializer.LoadConfig()
            };
            Build(appBundleBuilder, buildSettings);
        }

        /// <summary>
        /// Builds an Android App Bundle to a temp directory and then runs it on device.
        /// </summary>
        /// <param name="assetPackConfig">The asset pack configuration to use when building.</param>
        public static void BuildAndRun(AssetPackConfig assetPackConfig)
        {
            var appBundleBuilder = CreateAppBundleBuilder();
            if (!appBundleBuilder.Initialize(new BuildToolLogger()))
            {
                return;
            }

            var tempOutputFilePath = Path.Combine(appBundleBuilder.WorkingDirectoryPath, "temp.aab");
            var buildSettings = new AppBundleBuildSettings
            {
                buildPlayerOptions = AndroidBuildHelper.CreateBuildPlayerOptions(tempOutputFilePath),
                assetPackConfig = assetPackConfig,
                runOnDevice = true
            };
            Build(appBundleBuilder, buildSettings);
        }

        /// <summary>
        /// Sets the current BuildModeProvider. Should be called in a class annotated with "InitializeOnLoad" in order
        /// for it to be assigned after assembly reload.
        /// </summary>
        public static void SetBuildModeProvider(IBuildModeProvider buildModeProvider)
        {
            _buildModeProvider = buildModeProvider;
        }

        private static bool Build(AppBundleBuilder appBundleBuilder, AppBundleBuildSettings buildSettings)
        {
            var androidBuildResult = appBundleBuilder.BuildAndroidPlayer(buildSettings.buildPlayerOptions);
            if (!androidBuildResult.Succeeded)
            {
                return false;
            }

            var aabFilePath = buildSettings.buildPlayerOptions.locationPathName;
            if (IsBatchMode || buildSettings.forceSynchronousBuild)
            {
                var createBundleOptions = new CreateBundleOptions
                {
                    AabFilePath = aabFilePath, AssetPackConfig = buildSettings.assetPackConfig
                };
                var errorMessage = appBundleBuilder.CreateBundle(createBundleOptions);
                return errorMessage == null;
            }

#if UNITY_2018_3_OR_NEWER
            CreateBundleAsync(appBundleBuilder, aabFilePath, buildSettings.assetPackConfig, buildSettings.runOnDevice);
#else
            var task = new AppBundlePostBuildTask
            {
                serializableAssetPackConfig = SerializationHelper.Serialize(buildSettings.assetPackConfig),
                workingDirectoryPath = appBundleBuilder.WorkingDirectoryPath,
                aabFilePath = aabFilePath,
                runOnDevice = buildSettings.runOnDevice
            };
            PostBuildRunner.RunTask(task);
#endif
            // This return value should be ignored since the build is being finished asynchronously.
            return true;
        }

        private static void CreateBundleAsync(
            AppBundleBuilder appBundleBuilder, string aabFilePath, AssetPackConfig assetPackConfig, bool runOnDevice)
        {
            var callback =
                runOnDevice
                    ? (AppBundleBuilder.PostBuildCallback) RunBundle
                    : EditorUtility.RevealInFinder;

            var createBundleOptions = new CreateBundleOptions
            {
                AabFilePath = aabFilePath, AssetPackConfig = assetPackConfig
            };
            appBundleBuilder.CreateBundleAsync(createBundleOptions, callback);
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
            var jarSigner = new JarSigner(javaUtils);
            return new AppBundleBuilder(
                new AndroidAssetPackagingTool(androidBuildTools, androidSdkPlatform),
                new AndroidBuilder(androidSdkPlatform),
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