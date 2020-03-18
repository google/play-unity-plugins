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
using Google.Android.AppBundle.Editor.Internal.PlayServices;
using Google.Android.AppBundle.Editor.Internal.Utils;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Build tool for running <a href="https://developer.android.com/studio/command-line/aapt2">aapt2</a> commands.
    /// </summary>
    public class AndroidAssetPackagingTool : IBuildTool
    {
        /// <summary>
        /// Minimum version of Android SDK Build-Tools where aapt2 supports the "convert" command.
        /// </summary>
        private const string BuildToolsMinimumVersion = "28.0.0";

        /// <summary>
        /// Latest version of Android SDK Build-Tools known to be compatible with this plugin.
        /// </summary>
        private const string BuildToolsLatestVersion = "28.0.3";

        private const string BuildToolsDisplayName = "Android SDK Build-Tools";
        private const string BuildToolsPackageName = "build-tools;" + BuildToolsLatestVersion;

        private readonly AndroidBuildTools _androidBuildTools;
        private readonly AndroidSdkPlatform _androidSdkPlatform;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AndroidAssetPackagingTool(AndroidBuildTools androidBuildTools, AndroidSdkPlatform androidSdkPlatform)
        {
            _androidBuildTools = androidBuildTools;
            _androidSdkPlatform = androidSdkPlatform;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            if (!_androidBuildTools.Initialize(buildToolLogger) || !_androidSdkPlatform.Initialize(buildToolLogger))
            {
                return false;
            }

            var newestBuildToolsVersion = _androidBuildTools.GetNewestBuildToolsVersion();
            if (newestBuildToolsVersion == null)
            {
                buildToolLogger.DisplayErrorDialog(string.Format("Failed to locate {0}", BuildToolsDisplayName));
                return false;
            }

            if (AndroidBuildTools.IsBuildToolsVersionAtLeast(newestBuildToolsVersion, BuildToolsMinimumVersion))
            {
                return true;
            }

            var message = string.Format(
                "App Bundle creation requires {0} version {1} or later.\n\nClick \"OK\" to install {0} version {2}.",
                BuildToolsDisplayName, BuildToolsMinimumVersion, BuildToolsLatestVersion);
            if (buildToolLogger.DisplayActionableErrorDialog(message))
            {
                AndroidSdkPackageInstaller.InstallPackage(BuildToolsPackageName, BuildToolsDisplayName);
            }

            return false;
        }

        /// <summary>
        /// Given the specified APK, produces a new APK where resources.arsc is converted to proto format.
        /// </summary>
        /// <returns>An error message if there was a problem running aapt2, or null if successful.</returns>
        public virtual string Convert(string inputPath, string outputPath)
        {
            return Run(
                "convert {0} --output-format proto -o {1}",
                CommandLine.QuotePath(inputPath),
                CommandLine.QuotePath(outputPath));
        }

        /// <summary>
        /// Given the AndroidManifest.xml file path, creates an APK whose
        /// files are exploded into the specified output directory path.
        /// </summary>
        /// <returns>An error message if there was a problem running aapt2, or null if successful.</returns>
        public virtual string Link(string manifestPath, string outputPath)
        {
            return Run(
                "link -I {0} --manifest {1} --proto-format --output-to-dir -o {2}",
                CommandLine.QuotePath(GetAndroidJarPath()),
                CommandLine.QuotePath(manifestPath),
                CommandLine.QuotePath(outputPath));
        }

        private string Run(string aaptCommand, params object[] args)
        {
            var aaptPath = Path.Combine(_androidBuildTools.GetNewestBuildToolsPath(), "aapt2");
            var result = CommandLine.Run(aaptPath, string.Format(aaptCommand, args));
            return result.exitCode == 0 ? null : result.message;
        }

        private string GetAndroidJarPath()
        {
            var newestPlatformPath = _androidSdkPlatform.GetNewestAndroidSdkPlatformPath();
            if (newestPlatformPath == null)
            {
                throw new Exception("Unable to locate the latest version of the Android SDK Platform.");
            }

            var androidJarPath = Path.Combine(newestPlatformPath, "android.jar");
            if (!File.Exists(androidJarPath))
            {
                throw new Exception("Unable to locate android.jar in path:" + androidJarPath);
            }

            return androidJarPath;
        }
    }
}