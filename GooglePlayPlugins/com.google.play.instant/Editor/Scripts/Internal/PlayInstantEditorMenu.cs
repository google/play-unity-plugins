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

using Google.Android.AppBundle.Editor.Internal;
using Google.Play.Instant.Editor.Internal.QuickDeploy;
using UnityEditor;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal
{
    /// <summary>
    /// Provides "Play Instant" menu items for the Unity Editor.
    /// </summary>
    public static class PlayInstantEditorMenu
    {
        private const string PlayInstant = GoogleEditorMenu.MainMenuName + "/Play Instant/";

        private const int AboveLine = GoogleEditorMenu.PlayInstantPriority;
        private const int BelowLine = GoogleEditorMenu.PlayInstantPriority + GoogleEditorMenu.SeparatorSize;

        // These fields are defined in GoogleEditorMenu version 1.0.1, but were not present in the 1.0.0 release.
        private const string BuildSettingsText = "Build Settings...";
        private const string FileBugText = "File a Bug";
        private const string ViewDocumentationText = "View Documentation";
        private const string ViewLicenseText = "View License";

        [MenuItem(PlayInstant + BuildSettingsText, false, AboveLine)]
        private static void OpenBuildSettings()
        {
            PlayInstantBuildSettingsWindow.ShowWindow();
        }

        [MenuItem(PlayInstant + "Player Settings...", false, AboveLine + 1)]
        private static void CheckPlayerSettings()
        {
            PlayerSettingsWindow.ShowWindow();
        }

        [MenuItem(PlayInstant + "Quick Deploy...", false, AboveLine + 2)]
        private static void QuickDeployOverview()
        {
            QuickDeployWindow.ShowWindow();
        }

        [MenuItem(PlayInstant + ViewDocumentationText, false, BelowLine)]
        private static void ViewDocumentation()
        {
            Application.OpenURL(
                "https://developer.android.com/topic/google-play-instant/getting-started/game-unity-plugin");
        }

        [MenuItem(PlayInstant + ViewLicenseText, false, BelowLine + 1)]
        private static void ViewLicense()
        {
            // The guid is for GooglePlayPlugins/com.google.play.instant/LICENSE.md
            GoogleEditorMenu.OpenFileByGuid("0a58b4c913b854a34a3d5b54256d3588");
        }

        [MenuItem(PlayInstant + FileBugText, false, BelowLine + 2)]
        private static void ViewPlayPluginsIssuesPage()
        {
            // The GoogleEditorMenu version 1.0.0 did not contain ViewPlayPluginsIssuesPage().
            Application.OpenURL("https://github.com/google/play-unity-plugins/issues");
        }
    }
}