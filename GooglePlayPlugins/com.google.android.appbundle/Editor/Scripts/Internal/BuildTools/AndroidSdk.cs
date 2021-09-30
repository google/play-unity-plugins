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
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Build tool for finding the Android SDK root path.
    /// </summary>
    public class AndroidSdk : IBuildTool
    {
        /// <summary>
        /// The ANDROID_HOME environment variable key.
        /// </summary>
        public const string AndroidHomeEnvironmentVariableKey = "ANDROID_HOME";

        private const string AndroidNdkHomeEnvironmentVariableKey = "ANDROID_NDK_HOME";

        /// <summary>
        /// The environment variable key we use when overriding existing Unity editor preferences.
        /// We use UNITY_JAVA_HOME instead of JAVA_HOME because Unity will override JAVA_HOME to match it's editor preferences.
        /// </summary>
        private const string JavaHomeEnvironmentVariableKey = "UNITY_JAVA_HOME";
        private const string AndroidSdkRootEditorPrefsKey = "AndroidSdkRoot";
        private const string AndroidNdkRootEditorPrefsKey = "AndroidNdkRoot";
        private const string JdkPathEditorPrefsKey = "JdkPath";
        private const string JdkUseEmbeddedEditorPrefsKey = "JdkUseEmbedded";

        private string _androidSdkRoot;

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
#if !UNITY_2019_3_OR_NEWER
            return SetSdkRoot(buildToolLogger, EditorPrefs.GetString(AndroidSdkRootEditorPrefsKey));
#elif UNITY_ANDROID
            // Guard with #if UNITY_ANDROID since UnityEditor.Android.Extensions.dll might be unavailable,
            // e.g. on a build machine without the Android platform installed.
            return SetSdkRoot(buildToolLogger, UnityEditor.Android.AndroidExternalToolsSettings.sdkRootPath);
#else
            // This would be an unexpected and hopefully transient error.
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                buildToolLogger.DisplayErrorDialog("ActiveBuildTarget disagrees with #if UNITY_ANDROID.");
                return false;
            }

            // Handle the case where the UnityEditor.Android.Extensions.dll file may not be available by requiring that
            // Android be the selected platform on 2019.3+.
            const string message = "The Build Settings > Platform must be set to Android." +
                                   "\n\nClick \"OK\" to switch the active platform to Android.";
            if (buildToolLogger.DisplayActionableErrorDialog(message))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            }

            return false;
#endif
        }

        /// <summary>
        /// Returns the AndroidSdkRoot from Unity preferences or from the ANDROID_HOME environment variable.
        /// </summary>
        public virtual string RootPath
        {
            get
            {
                if (_androidSdkRoot == null)
                {
                    throw new BuildToolNotInitializedException(this);
                }

                return _androidSdkRoot;
            }
        }

        /// <summary>
        /// Override the AndroidSdkRoot/AndroidNdkRoot/JdkPath EditorPrefs with corresponding environment variables.
        /// This is especially helpful for automated builds where the preferences may not be set.
        /// </summary>
        public static void OverrideEditorPreferences()
        {
#if !UNITY_ANDROID
            return;
#elif UNITY_2019_3_OR_NEWER
            OverrideEditorPreference(AndroidHomeEnvironmentVariableKey,
                () => UnityEditor.Android.AndroidExternalToolsSettings.sdkRootPath,
                v => { UnityEditor.Android.AndroidExternalToolsSettings.sdkRootPath = v; });
            OverrideEditorPreference(AndroidNdkHomeEnvironmentVariableKey,
                () => UnityEditor.Android.AndroidExternalToolsSettings.ndkRootPath,
                v => { UnityEditor.Android.AndroidExternalToolsSettings.ndkRootPath = v; });
            OverrideEditorPreference(JavaHomeEnvironmentVariableKey,
                () => UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath,
                v => { UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath = v; });
#else
            OverrideEditorPreference(AndroidHomeEnvironmentVariableKey,
                () => EditorPrefs.GetString(AndroidSdkRootEditorPrefsKey),
                v => { EditorPrefs.SetString(AndroidSdkRootEditorPrefsKey, v); });
            OverrideEditorPreference(AndroidNdkHomeEnvironmentVariableKey,
                () => EditorPrefs.GetString(AndroidNdkRootEditorPrefsKey),
                v => { EditorPrefs.SetString(AndroidNdkRootEditorPrefsKey, v); });

            OverrideEditorPreference(JavaHomeEnvironmentVariableKey,
                () => EditorPrefs.GetString(JdkPathEditorPrefsKey),
                v => { EditorPrefs.SetString(JdkPathEditorPrefsKey, v); });

            // Older versions of Unity have an additional boolean preference that must be disabled.
            // Otherwise, Unity will use the JDK installed with the editor instead of the path we set in EditorPrefs.
            EditorPrefs.SetInt(JdkUseEmbeddedEditorPrefsKey, 0);
#endif
        }

        // TODO: Move this to a helper class and move the JDK portion of OverrideEditorPreferences into JavaUtils.
        private static void OverrideEditorPreference(
            string environmentVariableKey, Func<string> getPreference, Action<string> setPreference)
        {
            var environmentVariableValue = Environment.GetEnvironmentVariable(environmentVariableKey);
            var existingPreferenceValue = getPreference();
            Debug.LogFormat("Existing preference for {0} is \"{1}\"", environmentVariableKey, existingPreferenceValue);
            if (environmentVariableValue == existingPreferenceValue)
            {
                // Nothing to do.
                return;
            }

            if (string.IsNullOrEmpty(environmentVariableValue))
            {
                Debug.LogWarningFormat("Skipping empty environment variable: {0}", environmentVariableKey);
            }
            else
            {
                Debug.LogFormat("Setting preference for {0} to {1}", environmentVariableKey, environmentVariableValue);
                setPreference(environmentVariableValue);
            }
        }

        private bool SetSdkRoot(BuildToolLogger buildToolLogger, string sdkPath)
        {
            if (string.IsNullOrEmpty(sdkPath))
            {
                sdkPath = Environment.GetEnvironmentVariable(AndroidHomeEnvironmentVariableKey);
            }

            if (!Directory.Exists(sdkPath))
            {
                buildToolLogger.DisplayErrorDialog(
                    "Failed to locate the Android SDK. Check Preferences -> External Tools to set the path.");
                return false;
            }

            _androidSdkRoot = sdkPath;
            return true;
        }
    }
}