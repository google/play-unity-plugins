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

using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Provides methods for logging errors and/or displaying dialogs if there is a build issue.
    /// </summary>
    public class BuildToolLogger
    {
        public const string BuildErrorTitle = "Build Error";
        public const string BuildWarningTitle = "Build Warning";

        /// <summary>
        /// Returns true if the running instance of Unity is headless (e.g. a command line build),
        /// and false if it's a normal instance of Unity.
        /// </summary>
        public virtual bool IsHeadlessMode()
        {
            return WindowUtils.IsHeadlessMode();
        }

        /// <summary>
        /// Displays the specified message indicating that a build error occurred. Displays an "OK" button that can
        /// be used to indicate that the user wants to perform a followup action, e.g. fixing a build setting.
        /// </summary>
        /// <returns>True if the user clicks "OK", otherwise false.</returns>
        public virtual bool DisplayActionableErrorDialog(string message)
        {
            Debug.LogErrorFormat("Actionable build error: {0}", message);

            if (IsHeadlessMode())
            {
                // During a headless build it isn't possible to prompt to fix the issue, so always return false.
                return false;
            }

            return EditorUtility.DisplayDialog(
                BuildErrorTitle, message, WindowUtils.OkButtonText, WindowUtils.CancelButtonText);
        }

        /// <summary>
        /// Displays dialog with an additional option to "Opt out for this session". If the user selects that setting,
        /// they won't be shown the dialog again until they restart Unity. Instead, the specified message will be logged
        /// as a warning.
        ///
        /// On Unity versions 2019.2 and below, this method displays a normal dialog without the opt out option.
        /// </summary>
        /// <param name="message">The message included in the dialog.</param>
        /// <param name="optOutPreferenceKey">The unique key used to determine if a user has opted out of this dialog.</param>
        public virtual void DisplayOptOutDialog(string message, string optOutPreferenceKey)
        {
            Debug.LogWarningFormat("Build warning: {0}", message);
            if (IsHeadlessMode())
            {
                return;
            }

#if UNITY_2019_3_OR_NEWER
            var didOptOut =
                EditorUtility.GetDialogOptOutDecision(DialogOptOutDecisionType.ForThisSession, optOutPreferenceKey);
            if (didOptOut)
            {
                return;
            }


            EditorUtility.DisplayDialog(BuildWarningTitle, message, WindowUtils.OkButtonText,
                DialogOptOutDecisionType.ForThisSession, optOutPreferenceKey);
#else
            EditorUtility.DisplayDialog(BuildWarningTitle, message, WindowUtils.OkButtonText);
#endif
        }

        /// <summary>
        /// Displays the specified message indicating that a build error occurred.
        /// </summary>
        public virtual void DisplayErrorDialog(string message)
        {
            Debug.LogErrorFormat("Build error: {0}", message);

            if (!IsHeadlessMode())
            {
                EditorUtility.DisplayDialog(BuildErrorTitle, message, WindowUtils.OkButtonText);
            }
        }
    }
}