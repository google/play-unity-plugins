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
using UnityEditor;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal
{
    /// <summary>
    /// Helper to build Play Instant APKs and run them on a device.
    /// </summary>
    public class PlayInstantRunner : IBuildAndRunExtension
    {
        // Paths where the ia.jar may potentially be found.
        private const string PackagePath = "com.google.play.instant/Editor/Tools/ia.jar";
        private const string PackagesPath = "Packages/" + PackagePath;
        private const string PluginPath = "Assets/GooglePlayPlugins/" + PackagePath;

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

            // Check Play Instant build requirements before checking general build requirements, e.g. because OBB
            // creation is handled differently on Instant, where install-time asset packs aren't a viable alternative.
            if (!playInstantBuildHelper.Initialize(buildToolLogger)
                || !androidBuilder.Initialize(buildToolLogger)
                || !javaUtils.Initialize(buildToolLogger))
            {
                return;
            }

            var jarPath = IaJarPath;
            if (jarPath == null)
            {
                buildToolLogger.DisplayErrorDialog("Build and Run failed to locate ia.jar file");
                return;
            }

            EditorUserBuildSettings.buildAppBundle = false;

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

        /// <summary>
        /// Returns the absolute path to the ia.jar packaged with the plugin.
        /// Returning the absolute path handles cases where Unity treats relative paths
        /// differently than the command line. For example Unity treats the Packages/
        /// folder as a symlink to Library/PackageCache while the command line does not.
        /// </summary>
        private static string IaJarPath
        {
            get
            {
                // GUIDToAssetPath throws an exception if called on a non-main thread. To catch this case early, we
                // define this variable here, rather than below, where it is used.
                var guidPath = AssetDatabase.GUIDToAssetPath("c052bb994bc2b4a92a30f18d8e390a51");

                string relativePath = null;
                if (File.Exists(PackagesPath))
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