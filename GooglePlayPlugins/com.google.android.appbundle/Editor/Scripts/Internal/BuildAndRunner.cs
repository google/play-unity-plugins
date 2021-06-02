// Copyright 2019 Google LLC
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
using System.Linq;
using Google.Android.AppBundle.Editor.AssetPacks;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using UnityEditor;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Helper to build and run the app based on the current build configuration.
    /// </summary>
    public static class BuildAndRunner
    {
        /// <summary>
        /// Builds the app, then runs it on device.
        /// First checks if any IBuildAndRunnerExtensions opt to override this action.
        /// If so, it calls their BuildAndRun method.
        /// Otherwise, it builds an AppBundle or apk based on asset delivery settings.
        /// </summary>
        public static void BuildAndRun()
        {
            // IBuildAndRunExtensions can opt to override the default Build and Run functionality.
            var extensions = GetOverridingExtensions();
            switch (extensions.Count)
            {
                case 0:
                    break;
                case 1:
                    extensions.First().BuildAndRun();
                    return;
                default:
                    // TODO: Choose one of multiple implementations instead of throwing.
                    throw new InvalidOperationException(
                        "Multiple IBuildExtensions attempting to override Build and Run.");
            }

            BuildAndRunDefault();
        }

        /// <summary>
        /// First builds the current project as an APK or Android App Bundle, and then installs and runs it on a
        /// connected Android device.
        /// </summary>
        private static void BuildAndRunDefault()
        {
            var assetPackConfig = AssetPackConfigSerializer.LoadConfig();
            if (assetPackConfig.HasDeliveredAssetPacks())
            {
                AppBundlePublisher.BuildAndRun(assetPackConfig);
            }
            else
            {
                EmulateUnityBuildAndRun();
            }
        }

        /// <summary>
        /// Emulates Unity's File -> Build And Run menu option.
        /// </summary>
        private static void EmulateUnityBuildAndRun()
        {
            var androidSdk = new AndroidSdk();
            var androidSdkPlatform = new AndroidSdkPlatform(androidSdk);
            var androidBuilder = new AndroidBuilder(androidSdkPlatform);
            var buildToolLogger = new BuildToolLogger();
            if (!androidBuilder.Initialize(buildToolLogger))
            {
                return;
            }

            var artifactName = EditorUserBuildSettings.buildAppBundle ? "temp.aab" : "temp.apk";
            var artifactPath = Path.Combine(Path.GetTempPath(), artifactName);
            var buildPlayerOptions = AndroidBuildHelper.CreateBuildPlayerOptions(artifactPath);
            buildPlayerOptions.options |= BuildOptions.AutoRunPlayer;
            androidBuilder.Build(buildPlayerOptions);
        }

        // Visible for testing
        public static List<IBuildAndRunExtension> GetOverridingExtensions()
        {
            return GetExtensions().Where(x => x.ShouldOverride()).ToList();
        }

        private static IEnumerable<IBuildAndRunExtension> GetExtensions()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type =>
                    typeof(IBuildAndRunExtension).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IBuildAndRunExtension>();
        }
    }
}