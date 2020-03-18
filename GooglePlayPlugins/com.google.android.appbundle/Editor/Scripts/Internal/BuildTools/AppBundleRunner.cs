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

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Helper to build an Android App Bundle file on Unity.
    /// </summary>
    public class AppBundleRunner : IBuildTool
    {
        private readonly AndroidDebugBridge _adb;
        private readonly BundletoolHelper _bundletool;
        private string _packageName;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AppBundleRunner(AndroidDebugBridge adb, BundletoolHelper bundletool)
        {
            _adb = adb;
            _bundletool = bundletool;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            _packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            return _adb.Initialize(buildToolLogger) && _bundletool.Initialize(buildToolLogger);
        }

        /// <summary>
        /// Converts the specified app bundle into an apk set file,
        /// installs the proper apks,
        /// then runs the app on device.
        /// Note: This is designed to run in the main thread.
        /// TODO: Explore running this in a background thread.
        /// </summary>
        public void RunBundle(string aabFilePath, BundletoolBuildMode buildMode)
        {
            string apkSetFilePath;
            var errorMessage = ConvertAabToApkSet(aabFilePath, buildMode, out apkSetFilePath);
            if (errorMessage != null)
            {
                DisplayRunError("Creating apk set", errorMessage);
                return;
            }

            errorMessage = _bundletool.InstallApkSet(apkSetFilePath, _adb.GetAdbPath());
            if (errorMessage != null)
            {
                DisplayRunError("Installing app bundle", errorMessage);
                return;
            }

            Debug.Log("Installing app bundle");

            // TODO: Check the number of devices before launching to display a nicer error message.
            errorMessage = _adb.LaunchApp(_packageName);
            if (errorMessage != null)
            {
                DisplayRunError("Launching app bundle", errorMessage);
            }

            Debug.Log("Launching app bundle");
        }

        /// <summary>
        /// Converts the specified app bundle into an apk set file.
        /// </summary>
        /// <returns>An error message if the operation failed and null otherwise.</returns>
        public string ConvertAabToApkSet(string aabFilePath, BundletoolBuildMode buildMode, out string apkSetFilePath)
        {
            var aabFileDirectory = Path.GetDirectoryName(aabFilePath);
            if (aabFileDirectory == null)
            {
                apkSetFilePath = null;
                return "App Bundle does not exist at path " + aabFilePath;
            }

            var apkSetFileName = Path.GetFileNameWithoutExtension(aabFilePath);
            apkSetFilePath = Path.Combine(aabFileDirectory, apkSetFileName + ".apks");
            File.Delete(apkSetFilePath);

            // TODO: Set this value to be true regardless of BuildMode once local testing works on instant.
            var enableLocalTesting = buildMode == BundletoolBuildMode.Persistent;

            return _bundletool.BuildApkSet(aabFilePath, apkSetFilePath, buildMode, enableLocalTesting);
        }

        // TODO: Display a dialog prompt and/or progress bar.
        private void DisplayRunError(string errorType, string errorMessage)
        {
            var fullErrorMessage = string.Format("{0} failed: {1}", errorType, errorMessage);
            Debug.LogError(fullErrorMessage);
        }
    }
}