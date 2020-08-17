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

using Google.Android.AppBundle.Editor.Internal.Utils;
using Google.Play.Instant.Editor.Internal.AndroidManifest;
using UnityEditor;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal
{
    /// <summary>
    /// A window for managing settings related to building an Android instant app for distribution on Google Play.
    /// </summary>
    public class PlayInstantBuildSettingsWindow : EditorWindow
    {
        public const string WindowTitle = "Play Instant Build Settings";
        private const int WindowMinWidth = 400;
        private const int WindowMinHeight = 260;
        private const int FieldWidth = 175;
        private static readonly string[] PlatformOptions = {"Installed", "Instant"};

        private readonly IAndroidManifestUpdater _androidManifestUpdater =
#if UNITY_2018_1_OR_NEWER
            new PostGenerateGradleProjectAndroidManifestUpdater();
#else
            new LegacyAndroidManifestUpdater();
#endif
        private static PlayInstantBuildSettingsWindow _windowInstance;

        private bool _isInstant;
        private bool _playGamesEnabled;

        /// <summary>
        /// Displays this window, creating it if necessary.
        /// </summary>
        public static void ShowWindow()
        {
            _windowInstance = GetWindow<PlayInstantBuildSettingsWindow>(true, WindowTitle);
            _windowInstance.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
        }

        private void OnDestroy()
        {
            _windowInstance = null;
        }

        private void Awake()
        {
            ReadFromBuildConfiguration();
        }

        /// <summary>
        /// Read and update the window with most recent build configuration values.
        /// </summary>
        void ReadFromBuildConfiguration()
        {
            _isInstant = PlayInstantBuildSettings.IsInstantBuildType();
            _playGamesEnabled = PlayInstantBuildConfig.PlayGamesEnabled;
        }

        /// <summary>
        /// Update window with most recent build configuration values if the window is open.
        /// </summary>
        public static void UpdateWindowIfOpen()
        {
            if (_windowInstance != null)
            {
                _windowInstance.ReadFromBuildConfiguration();
                _windowInstance.Repaint();
            }
        }

        private void OnGUI()
        {
            // Edge case that takes place when the plugin code gets re-compiled while this window is open.
            if (_windowInstance == null)
            {
                _windowInstance = this;
            }

            var descriptionTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Italic,
                wordWrap = true
            };

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Android Build Type", EditorStyles.boldLabel, GUILayout.Width(FieldWidth));
            var index = EditorGUILayout.Popup(_isInstant ? 1 : 0, PlatformOptions);
            _isInstant = index == 1;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (_isInstant)
            {
                var packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) ?? "package-name";
                EditorGUILayout.LabelField(
                    "Users can try instant apps in the Play Store by clicking the \"Try Now\" button. " +
                    "Instant apps can also be launched using the following Launch API URL:",
                    descriptionTextStyle);
                EditorGUILayout.SelectableLabel(
                    string.Format("https://play.google.com/store/apps/details?id={0}&launch=true", packageName),
                    descriptionTextStyle);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Full \"Instant play\" game", EditorStyles.boldLabel,
                    GUILayout.Width(FieldWidth));
                _playGamesEnabled = EditorGUILayout.Toggle(_playGamesEnabled);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField(
                    "Check this box to indicate that this is a full experience instant game that is eligible for " +
                    "\"Instant play\" featuring in the Play Games app. Leave the box unchecked if this is a trial or " +
                    "demo instant game.",
                    descriptionTextStyle);
                if (GUILayout.Button("Learn More", GUILayout.Width(90)))
                {
                    Application.OpenURL("https://g.co/play/instant");
                }
            }
            else
            {
                EditorGUILayout.LabelField(
                    "The \"Installed\" build type is used when creating a traditional installable APK. " +
                    "Select \"Instant\" to enable building a Google Play Instant APK or app bundle.",
                    descriptionTextStyle);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Disable the Save button unless one of the fields has changed.
            GUI.enabled = IsAnyFieldChanged();

            if (GUILayout.Button("Save"))
            {
                if (_isInstant)
                {
                    SelectPlatformInstant();
                }
                else
                {
                    SelectPlatformInstalled();
                }
            }

            GUI.enabled = true;
        }

        private bool IsAnyFieldChanged()
        {
            return _isInstant != PlayInstantBuildSettings.IsInstantBuildType() ||
                   _playGamesEnabled != PlayInstantBuildConfig.PlayGamesEnabled;
        }

        private void SelectPlatformInstant()
        {
            var errorMessage = _androidManifestUpdater.SwitchToInstant();
            if (errorMessage != null)
            {
                var message = string.Format("Error updating AndroidManifest.xml: {0}", errorMessage);
                Debug.LogError(message);
                EditorUtility.DisplayDialog("Error Saving", message, WindowUtils.OkButtonText);
                return;
            }

            SaveConfiguration();
        }

        private void SelectPlatformInstalled()
        {
            _androidManifestUpdater.SwitchToInstalled();
            SaveConfiguration();
        }

        private void SaveConfiguration()
        {
            PlayInstantBuildSettings.SetInstantBuildType(_isInstant);
            PlayInstantBuildConfig.SaveConfiguration(_playGamesEnabled);
            Debug.Log("Saved Play Instant Build Settings");
        }
    }
}