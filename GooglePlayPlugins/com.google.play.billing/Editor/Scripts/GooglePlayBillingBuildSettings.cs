// Copyright 2020 Google LLC
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

using UnityEditor;
using UnityEngine;

namespace Google.Play.Billing.Editor
{
    /// <summary>
    /// A window for making changes that related to building the app.
    /// </summary>
    public class GooglePlayBillingBuildSettings : EditorWindow
    {
        private const int WindowMinWidth = 500;
        private const int WindowMinHeight = 200;
        private const int ActionButtonWidth = 100;

        private bool _removeAidlFileSuccess = true;
        private bool _restoreAidlFileSuccess = true;

        private const string ConflictDetectionDescription =
            "The Google Play Billing Plugin includes the latest version of Google's Play Billing Library. This " +
            "library conflicts with the Unity IAP's library, causing a failure if both exist when the project is " +
            "built.";
        private const string FixButtonDescription =
            "Click \"Fix\" to create a backup of conflicting Unity IAP files and remove them from the project. " +
            "You can restore the files by using this dialog later.";
        private const string RestoreButtonDescription =
            "Click \"Restore\" to restore the backed-up conflicting Unity IAP files.";

        private const string OperationFailureMessage = "Operaton Failed, please check Console for error logs.";

        /// <summary>
        /// Displays this window, creating it if necessary.
        /// </summary>
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(GooglePlayBillingBuildSettings), true, "Play Billing Build Settings");
            window.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
        }

        private void OnGUI()
        {
            AddOptionForConflictingAarFile();
        }

        private void AddOptionForConflictingAarFile()
        {
            EditorGUILayout.LabelField("Conflict Detection", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GooglePlayBillingBuildHelper.HasConflictingGoogleAarFile())
            {
                OnConflictingAarFileDetected();
            }
            else
            {
                OnNoConflictingAarFileDetected();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void OnConflictingAarFileDetected()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(ConflictDetectionDescription, EditorStyles.wordWrappedLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Conflicts with Google Play Billing Plugin.", GetWarningStyle());
            if (GUILayout.Button("Fix", GUILayout.Width(ActionButtonWidth)))
            {
                _removeAidlFileSuccess = GooglePlayBillingBuildHelper.RemoveConflictingAarFiles();
            }

            EditorGUILayout.EndHorizontal();

            if (_removeAidlFileSuccess)
            {
                EditorGUILayout.LabelField(FixButtonDescription, EditorStyles.wordWrappedLabel);
            }
            else
            {
                EditorGUILayout.LabelField(OperationFailureMessage, GetWarningStyle());
            }

            EditorGUILayout.EndVertical();
        }

        private void OnNoConflictingAarFileDetected()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(ConflictDetectionDescription, EditorStyles.wordWrappedLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("No conflict with Google Play Billing Plugin.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Restore", GUILayout.Width(ActionButtonWidth)))
            {
                _restoreAidlFileSuccess = GooglePlayBillingBuildHelper.RestoreConflictingAarFiles();
            }

            EditorGUILayout.EndHorizontal();

            if (_restoreAidlFileSuccess)
            {
                EditorGUILayout.LabelField(RestoreButtonDescription, EditorStyles.wordWrappedLabel);
            }
            else
            {
                EditorGUILayout.LabelField(OperationFailureMessage, GetWarningStyle());
            }

            EditorGUILayout.EndVertical();
        }

        private static GUIStyle GetWarningStyle()
        {
            var s = new GUIStyle(EditorStyles.wordWrappedLabel);
            s.normal.textColor = Color.red;
            return s;
        }
    }
}
