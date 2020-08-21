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
using UnityEditor;
using UnityEngine;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Provides methods for building an Android app for distribution on Google Play.
    /// </summary>
    public class AndroidBuilder : IBuildTool
    {
        private readonly AndroidSdkPlatform _androidSdkPlatform;
        private readonly ApkSigner _apkSigner;

        private BuildToolLogger _buildToolLogger;

        public AndroidBuilder(AndroidSdkPlatform androidSdkPlatform, ApkSigner apkSigner)
        {
            _androidSdkPlatform = androidSdkPlatform;
            _apkSigner = apkSigner;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            _buildToolLogger = buildToolLogger;
            return _androidSdkPlatform.Initialize(buildToolLogger) && _apkSigner.Initialize(buildToolLogger);
        }

        /// <summary>
        /// Builds an APK or AAB based on the specified options and signs it (if necessary) via
        /// <a href="https://source.android.com/security/apksigning/v2">APK Signature Scheme V2</a>.
        /// Displays warning/error dialogs if there are issues during the build.
        /// </summary>
        /// <returns>True if the build succeeded, false if it failed or was cancelled.</returns>
        public virtual bool BuildAndSign(BuildPlayerOptions buildPlayerOptions)
        {
            if (!Build(buildPlayerOptions))
            {
                return false;
            }

#if UNITY_2018_1_OR_NEWER
            // On Unity 2018.1+ we require Gradle builds. Unity 2018+ Gradle builds always yield a properly signed APK.
            return true;
#else
            // ApkSigner is fast so we call it synchronously rather than wait for the post build AppDomain reset.
            Debug.Log("Checking for APK Signature Scheme V2...");
            var apkPath = buildPlayerOptions.locationPathName;
            if (_apkSigner.Verify(apkPath))
            {
                return true;
            }

            Debug.Log("APK must be re-signed for APK Signature Scheme V2...");
            var signingResult = _apkSigner.Sign(apkPath);
            if (signingResult == null)
            {
                Debug.Log("Re-signed with APK Signature Scheme V2.");
                return true;
            }

            _buildToolLogger.DisplayErrorDialog(
                string.Format("Failed to re-sign the APK using apksigner:\n\n{0}", signingResult));
            return false;
#endif
        }

        /// <summary>
        /// Builds an APK or AAB based on the specified options.
        /// Displays warning/error dialogs if there are issues during the build.
        /// </summary>
        /// <returns>True if the build succeeded, false if it failed or was cancelled.</returns>
        public virtual bool Build(BuildPlayerOptions buildPlayerOptions)
        {
            if (_buildToolLogger == null)
            {
                throw new BuildToolNotInitializedException(this);
            }

            if (buildPlayerOptions.target != BuildTarget.Android)
            {
                throw new ArgumentException("The build target must be Android.", "buildPlayerOptions");
            }

            if (buildPlayerOptions.targetGroup != BuildTargetGroup.Android)
            {
                throw new ArgumentException("The build target group must be Android.", "buildPlayerOptions");
            }

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
#if UNITY_2018_1_OR_NEWER
            var totalErrors = buildReport.summary.totalErrors;
            switch (buildReport.summary.result)
            {
                case BuildResult.Cancelled:
                    Debug.Log("Build cancelled");
                    return false;
                case BuildResult.Succeeded:
                    Debug.Log("Build succeeded");
                    return HandleBuildPlayerSucceeded(totalErrors);
                case BuildResult.Failed:
                    _buildToolLogger.DisplayErrorDialog(string.Format("Build failed with {0} error(s)", totalErrors));
                    return false;
                default:
                    _buildToolLogger.DisplayErrorDialog("Build failed with unknown error");
                    return false;
            }
#else
            if (string.IsNullOrEmpty(buildReport))
            {
                return true;
            }

            // Check for intended build cancellation.
            if (buildReport == "Building Player was cancelled")
            {
                Debug.Log(buildReport);
            }
            else
            {
                _buildToolLogger.DisplayErrorDialog(buildReport);
            }

            return false;
#endif
        }

#if UNITY_2018_1_OR_NEWER
        // This method is in an #if block since it's only called for 2018.1+
        private static bool HandleBuildPlayerSucceeded(int totalErrors)
        {
            if (totalErrors == 0)
            {
                return true;
            }

            var messagePrefix = string.Format("BuildPlayer \"succeeded\" with {0} error(s). ", totalErrors);
#if UNITY_2018_4_OR_NEWER
            // BuildPlayer can succeed and yet have non-zero totalErrors on some versions of Unity.
            Debug.LogWarning(messagePrefix + "Assuming success.");
            return true;
#else
            // BuildPlayer can fail and yet return BuildResult Succeeded on some versions of Unity.
            // No need to display an error dialog since Unity should already have done this.
            Debug.LogError(messagePrefix + "Assuming failure.");
            return false;
#endif
        }
#endif
    }
}