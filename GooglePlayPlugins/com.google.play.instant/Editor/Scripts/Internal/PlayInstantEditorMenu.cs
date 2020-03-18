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

        [MenuItem(PlayInstant + "Build Settings...", false, AboveLine)]
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

        [MenuItem(PlayInstant + "View Documentation", false, BelowLine)]
        private static void ViewDocumentation()
        {
            Application.OpenURL("https://g.co/InstantApps");
        }

        [MenuItem(PlayInstant + "View License", false, BelowLine + 1)]
        private static void ViewLicense()
        {
            // The guid is for GooglePlayPlugins/com.google.play.instant/LICENSE.md
            GoogleEditorMenu.OpenFileByGuid("0a58b4c913b854a34a3d5b54256d3588");
        }
    }
}