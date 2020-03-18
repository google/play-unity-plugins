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
using System.IO;
using System.Text.RegularExpressions;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Build tool for discovering Android SDK Platform versions.
    /// </summary>
    public class AndroidSdkPlatform : IBuildTool
    {
        // The minimum Android target SDK version supported by Google Play is described here:
        // https://support.google.com/googleplay/android-developer/answer/113469#targetsdk
        private const int MinimumVersion = 28;
        private const int LatestVersion = 29;

        private static readonly Regex PlatformVersionRegex = RegexHelper.CreateCompiled(@"^android-(\d+)$");

        private readonly AndroidSdk _androidSdk;

        public AndroidSdkPlatform(AndroidSdk androidSdk)
        {
            _androidSdk = androidSdk;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            if (!_androidSdk.Initialize(buildToolLogger))
            {
                return false;
            }

            string ignoredPath;
            var newestVersion = GetNewestVersionAndPath(out ignoredPath);
            if (newestVersion == null)
            {
                // Being unable to find any existing SDK may indicate a config issue, so don't try to install a new SDK.
                buildToolLogger.DisplayErrorDialog(
                    "Failed to locate any Android SDK Platform version. Check that the Android SDK specified "
                    + "through Preferences -> External Tools has at least one Android SDK Platform installed.");
                return false;
            }

            if (newestVersion < MinimumVersion)
            {
                var installedVersionMessage = string.Format(
                    "The highest installed Android API Level is {0}, however version {1} is the minimum "
                    + "required to build for Google Play.\n\nClick \"OK\" to install Android API Level {2}.",
                    newestVersion, MinimumVersion, LatestVersion);
                if (buildToolLogger.DisplayActionableErrorDialog(installedVersionMessage))
                {
                    // Note: this install can be slow, but it's not clear that it's any slower through Unity.
                    AndroidSdkPackageInstaller.InstallPackage(
                        string.Format("platforms;android-{0}", LatestVersion),
                        string.Format("Android SDK Platform {0}", LatestVersion));
                }

                return false;
            }

            var targetSdkVersion = PlayerSettings.Android.targetSdkVersion;
            if (targetSdkVersion == AndroidSdkVersions.AndroidApiLevelAuto || (int) targetSdkVersion >= MinimumVersion)
            {
                return true;
            }

            var selectedVersionMessage = string.Format(
                "The currently selected Android Target API Level is {0}, however version {1} is the minimum "
                + "required to build for Google Play.\n\nClick \"OK\" to change the Target API Level to "
                + "\"Automatic (highest installed)\", which is currently {2}.",
                (int) targetSdkVersion, MinimumVersion, newestVersion);
            if (buildToolLogger.DisplayActionableErrorDialog(selectedVersionMessage))
            {
                PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            }

            return false;
        }

        /// <summary>
        /// Returns the path to the newest installed version of the Android SDK Platform, or null if it isn't found.
        /// </summary>
        public virtual string GetNewestAndroidSdkPlatformPath()
        {
            string newestPlatformPath;
            GetNewestVersionAndPath(out newestPlatformPath);
            return newestPlatformPath;
        }

        private int? GetNewestVersionAndPath(out string newestPlatformPath)
        {
            newestPlatformPath = null;
            var platformsPath = Path.Combine(_androidSdk.RootPath, "platforms");
            var platformsDirectoryInfo = new DirectoryInfo(platformsPath);
            if (!platformsDirectoryInfo.Exists)
            {
                Debug.LogErrorFormat("Unable to locate Android SDK platforms directory: {0}", platformsPath);
                return null;
            }

            int? newestPlatformVersion = null;
            DirectoryInfo newestPlatformDirectory = null;
            foreach (var platformDirectory in platformsDirectoryInfo.GetDirectories())
            {
                var match = PlatformVersionRegex.Match(platformDirectory.Name);
                if (!match.Success)
                {
                    continue;
                }

                var platformVersionString = match.Groups[1].Value;
                var platformVersion = int.Parse(platformVersionString);
                newestPlatformVersion = Math.Max(platformVersion, newestPlatformVersion ?? -1);
                if (platformVersion == newestPlatformVersion)
                {
                    newestPlatformDirectory = platformDirectory;
                }
            }

            if (newestPlatformDirectory == null)
            {
                Debug.LogErrorFormat("Unable to locate newest Android SDK platform in directory: {0}", platformsPath);
                return null;
            }

            newestPlatformPath = newestPlatformDirectory.FullName;
            return newestPlatformVersion;
        }
    }
}