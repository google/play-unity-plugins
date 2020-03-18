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

using System.IO;
using Google.Android.AppBundle.Editor;
using Google.Android.AppBundle.Editor.Internal;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using Google.Android.AppBundle.Editor.Internal.PlayServices;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal
{
    /// <summary>
    /// Helper to build Play Instant APKs and run them on a device.
    /// </summary>
    public class PlayInstantRunner : IBuildAndRunExtension
    {
        private const string InstantAppsSdkDisplayName = "Google Play Instant Development SDK";
        private const string InstantAppsSdkPackageName = "extras;google;instantapps";
        private const string InstantAppsJarPath = "extras/google/instantapps/tools/ia.jar";

        bool IBuildAndRunExtension.ShouldOverride()
        {
            return PlayInstantBuildSettings.IsInstantBuildType();
        }

        void IBuildAndRunExtension.BuildAndRun()
        {
            var buildToolLogger = new BuildToolLogger();
            // TODO Build an app bundle once bundletool supports locally testing asset packs on instant.
            BuildAndRunApk(buildToolLogger);
        }

        /// <summary>
        /// Builds an APK to a temporary location, then installs and runs it using ia.jar
        /// </summary>
        private static void BuildAndRunApk(BuildToolLogger buildToolLogger)
        {
            var androidSdk = new AndroidSdk();
            var androidSdkPlatform = new AndroidSdkPlatform(androidSdk);
            var androidBuildTools = new AndroidBuildTools(androidSdk);
            var javaUtils = new JavaUtils();
            var apkSigner = new ApkSigner(androidBuildTools, javaUtils);
            var androidBuilder = new AndroidBuilder(androidSdkPlatform, apkSigner);
            var playInstantBuildHelper = new PlayInstantBuildHelper(isInstantRequired: true);

            if (!androidBuilder.Initialize(buildToolLogger) || !playInstantBuildHelper.Initialize(buildToolLogger))
            {
                return;
            }

            var jarPath = GetInstantAppJarPath(androidSdk, buildToolLogger);
            if (jarPath == null)
            {
                return;
            }

            AndroidAppBundle.DisableNativeBuild();

            var apkPath = Path.Combine(Path.GetTempPath(), "temp.apk");
            Debug.LogFormat("Build and Run package location: {0}", apkPath);

            var buildPlayerOptions = AndroidBuildHelper.CreateBuildPlayerOptions(apkPath);
            if (!androidBuilder.BuildAndSign(buildPlayerOptions))
            {
                // Do not log here. The method we called was responsible for logging.
                return;
            }

            InstallInstantApp(jarPath, apkPath, androidSdk, javaUtils);
        }

        private static void InstallInstantApp(string jarPath, string artifactPath, AndroidSdk androidSdk,
            JavaUtils javaUtils)
        {
            var window = PostBuildCommandLineDialog.CreateDialog("Install and run app");
            window.modal = false;
            window.summaryText = "Installing app on device";
            window.bodyText = "Built successfully.\n\n";
            window.autoScrollToBottom = true;
            window.CommandLineParams = new CommandLineParameters
            {
                FileName = javaUtils.JavaBinaryPath,
                Arguments = string.Format(
                    "-jar {0} run {1}",
                    CommandLine.QuotePath(jarPath),
                    CommandLine.QuotePath(artifactPath))
            };
            window.CommandLineParams.AddEnvironmentVariable(
                AndroidSdk.AndroidHomeEnvironmentVariableKey, androidSdk.RootPath);
            window.Show();
        }

        private static string GetInstantAppJarPath(AndroidSdk androidSdk, BuildToolLogger buildToolLogger)
        {
            var jarPath = Path.Combine(androidSdk.RootPath, InstantAppsJarPath);
            if (File.Exists(jarPath))
            {
                return jarPath;
            }

            Debug.LogErrorFormat("Build and Run failed to locate ia.jar file at: {0}", jarPath);
            var message =
                string.Format(
                    "Failed to locate version 1.2 or later of the {0}.\n\nClick \"OK\" to install the {0}.",
                    InstantAppsSdkDisplayName);
            if (buildToolLogger.DisplayActionableErrorDialog(message))
            {
                AndroidSdkPackageInstaller.InstallPackage(InstantAppsSdkPackageName, InstantAppsSdkDisplayName);
            }

            return null;
        }
    }
}