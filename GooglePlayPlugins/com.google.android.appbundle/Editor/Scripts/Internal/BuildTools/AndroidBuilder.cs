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
using Google.Android.AppBundle.Editor.Internal.AssetPacks;
using UnityEditor;
using UnityEngine;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;

#endif

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Indicates the results of an <see cref="AndroidBuilder"/> build. This provides similar information as
    /// Task&lt;AndroidBuildReport&gt; without requiring the .NET 4.0 async task API.
    /// </summary>
    public class AndroidBuildResult
    {
        /// <summary>
        /// Returns true if Android Player build Succeeded, otherwise false.
        /// </summary>
        public bool Succeeded
        {
            get { return !Cancelled && ErrorMessage == null; }
        }

        /// <summary>
        /// Returns true if the Android Player build was cancelled before it finished.
        /// </summary>
        public bool Cancelled { get; internal set; }

        /// <summary>
        /// If non-null, the build failed and this message may indicate the cause.
        /// </summary>
        public string ErrorMessage { get; internal set; }

#if UNITY_2018_1_OR_NEWER
        /// <summary>
        /// Contains the BuildReport generated during the creation of the Android Player.
        /// </summary>
        public BuildReport Report { get; internal set; }
#endif
    }

    /// <summary>
    /// Provides methods for building an Android app for distribution on Google Play.
    /// </summary>
    public class AndroidBuilder : IBuildTool
    {
        // This string is returned by pre-2018.1's BuildPipeline.BuildPlayer() when a build is cancelled.
        private const string BuildCancelledMessage = "Building Player was cancelled";

        private readonly AndroidSdkPlatform _androidSdkPlatform;

        private BuildToolLogger _buildToolLogger;

        public AndroidBuilder(AndroidSdkPlatform androidSdkPlatform)
        {
            _androidSdkPlatform = androidSdkPlatform;
        }

        // TODO: Needed for 1.x API compatibility. Should be removed with 2.x.
        public AndroidBuilder(AndroidSdkPlatform androidSdkPlatform, ApkSigner apkSigner) : this(androidSdkPlatform)
        {
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            _buildToolLogger = buildToolLogger;

            if (EditorUserBuildSettings.androidBuildSystem != AndroidBuildSystem.Gradle)
            {
                // We require Gradle builds for APK Signature Scheme V2, which is required for Play Instant on Unity 2017+.
                const string message = "This build requires the Gradle Build System.\n\n" +
                                       "Click \"OK\" to change the Android Build System to Gradle.";
                if (buildToolLogger.DisplayActionableErrorDialog(message))
                {
                    EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
                }

                return false;
            }

            // TODO: remove this check if we add support for exporting a Gradle project.
            if (EditorUserBuildSettings.exportAsGoogleAndroidProject)
            {
                const string message = "This build doesn't support exporting to Android Studio.\n\n" +
                                       "Click \"OK\" to disable exporting a Gradle project.";
                if (buildToolLogger.DisplayActionableErrorDialog(message))
                {
                    EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                }

                return false;
            }

            if (BuiltInPadHelper.EditorSupportsPad() && BuiltInPadHelper.ProjectHasAndroidPacks())
            {
                var message =
                    "This build method doesn't support .androidpack folders. Assets within those folders will not be included" +
                    " in the build.";
                buildToolLogger.DisplayOptOutDialog(message, "androidPackError");
            }

            if (PlayerSettings.Android.useAPKExpansionFiles)
            {
                string messagePrefix;
                if (BuiltInPadHelper.EditorSupportsPad())
                {
                    messagePrefix = "This build method doesn't support Unity's \"Split Application Binary\" option.";
                }
                else
                {
                    messagePrefix = "This build method doesn't support APK Expansion (OBB) files.";
                }

                var message = string.Format(
                    "{0}\n\nLarge games can instead use the " +
                    "\"{1}\" option available through the \"Google > Android App Bundle > Asset Delivery " +
                    "Settings\" menu or the AssetPackConfig API's SplitBaseModuleAssets field.\n\n" +
                    "Click \"OK\" to disable the \"Split Application Binary\" setting.",
                    messagePrefix,
                    AssetDeliveryWindow.SeparateAssetsLabel);
                if (buildToolLogger.DisplayActionableErrorDialog(message))
                {
                    PlayerSettings.Android.useAPKExpansionFiles = false;
                }

                return false;
            }

            return _androidSdkPlatform.Initialize(buildToolLogger);
        }

        // TODO: Needed for 1.x API compatibility. Should be removed with 2.x.
        public virtual bool BuildAndSign(BuildPlayerOptions buildPlayerOptions)
        {
            return Build(buildPlayerOptions).Succeeded;
        }

        /// <summary>
        /// Builds an APK or AAB based on the specified options.
        /// Displays warning/error dialogs if there are issues during the build.
        /// </summary>
        public virtual AndroidBuildResult Build(BuildPlayerOptions buildPlayerOptions)
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

            // Note: the type of the variable below differs by version. On 2018+ it's BuildReport. On pre-2018 it's
            // string: if the string is null, the build was successful, otherwise it's a build error message.
            var buildReportOrErrorMessage = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var androidBuildResult = GetAndroidBuildResult(buildReportOrErrorMessage);
            if (androidBuildResult.Cancelled)
            {
                // Don't display an error message dialog if the user asked to cancel, just log to the Console.
                Debug.Log(BuildCancelledMessage);
                return androidBuildResult;
            }

            if (androidBuildResult.Succeeded && !File.Exists(buildPlayerOptions.locationPathName))
            {
                // Sometimes the build "succeeds" but the AAB/APK file is missing.
                androidBuildResult.ErrorMessage =
                    string.Format(
                        "The Android Player file \"{0}\" is missing, possibly because of a late cancellation.",
                        buildPlayerOptions.locationPathName);
#if UNITY_2018_1_OR_NEWER
                androidBuildResult.ErrorMessage += " TotalErrors=" + buildReportOrErrorMessage.summary.totalErrors;
#endif
            }

            if (androidBuildResult.Succeeded)
            {
                Debug.Log("Android Player build succeeded");
            }
            else
            {
                _buildToolLogger.DisplayErrorDialog(androidBuildResult.ErrorMessage);
            }

            return androidBuildResult;
        }

#if UNITY_2018_1_OR_NEWER
        private static AndroidBuildResult GetAndroidBuildResult(BuildReport buildReport)
        {
            var androidBuildResult = new AndroidBuildResult();
            androidBuildResult.Report = buildReport;
            switch (buildReport.summary.result)
            {
                case BuildResult.Succeeded:
                    // Do nothing.
                    break;
                case BuildResult.Cancelled:
                    androidBuildResult.Cancelled = true;
                    break;
                case BuildResult.Failed:
                    androidBuildResult.ErrorMessage =
                        string.Format("Build failed with {0} error(s)", buildReport.summary.totalErrors);
                    break;
                case BuildResult.Unknown:
                    androidBuildResult.ErrorMessage = "Build failed with unknown result";
                    break;
                default:
                    androidBuildResult.ErrorMessage =
                        "Build failed with unexpected result: " + buildReport.summary.result;
                    break;
            }

            return androidBuildResult;
        }
#else
        private static AndroidBuildResult GetAndroidBuildResult(string errorMessage)
        {
            var androidBuildResult = new AndroidBuildResult();
            if (errorMessage == BuildCancelledMessage)
            {
                androidBuildResult.Cancelled = true;
            }
            else if (!string.IsNullOrEmpty(errorMessage))
            {
                // Assume that a null or empty error message string indicates success.
                androidBuildResult.ErrorMessage = errorMessage;
            }

            return androidBuildResult;
        }
#endif
    }
}