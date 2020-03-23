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

using Google.Android.AppBundle.Editor.Internal.AssetPacks;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Provides "Android App Bundle" and Android build related menu items for the Unity Editor.
    /// </summary>
    public static class AppBundleEditorMenu
    {
        private const string AppBundleMenuName = GoogleEditorMenu.MainMenuName + "/Android App Bundle/";
        private const string RootMenuName = GoogleEditorMenu.MainMenuName + "/";

        private const int AboveLine = GoogleEditorMenu.AndroidAppBundlePriority;
        private const int BelowLine = GoogleEditorMenu.AndroidAppBundlePriority + GoogleEditorMenu.SeparatorSize;

        [MenuItem(AppBundleMenuName + "Asset Delivery Settings...", false, AboveLine)]
        private static void ShowAssetDeliveryWindow()
        {
            AssetDeliveryWindow.ShowWindow();
        }

        [MenuItem(AppBundleMenuName + GoogleEditorMenu.BuildSettings, false, AboveLine + 1)]
        private static void OpenEditorSettings()
        {
            BuildSettingsWindow.ShowWindow();
        }

        [MenuItem(AppBundleMenuName + GoogleEditorMenu.ViewDocumentation, false, BelowLine)]
        private static void ViewDocumentation()
        {
            Application.OpenURL("https://developer.android.com/guide/app-bundle/asset-delivery/build-unity");
        }

        [MenuItem(AppBundleMenuName + GoogleEditorMenu.ViewLicense, false, BelowLine + 1)]
        private static void ViewLicense()
        {
            // The guid is for GooglePlayPlugins/com.google.android.appbundle/LICENSE.md
            GoogleEditorMenu.OpenFileByGuid("4b995f494568e47259d526c5211d778b");
        }

        [MenuItem(AppBundleMenuName + GoogleEditorMenu.FileBug, false, BelowLine + 2)]
        private static void ViewPlayPluginsIssuesPage()
        {
            GoogleEditorMenu.ViewPlayPluginsIssuesPage();
        }

        [MenuItem(RootMenuName + "Build Android App Bundle...", false, GoogleEditorMenu.RootMenuPriority + 10)]
        private static void BuildAndroidAppBundle()
        {
            AppBundlePublisher.Build();
        }

        [MenuItem(RootMenuName + "Build and Run #%r", false, GoogleEditorMenu.RootMenuPriority + 11)]
        private static void BuildAndRun()
        {
            BuildAndRunner.BuildAndRun();
        }
    }
}