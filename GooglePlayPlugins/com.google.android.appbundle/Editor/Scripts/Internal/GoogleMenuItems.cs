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
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Provides infrastructure for adding "Google" menu items in the Unity Editor.
    /// </summary>
    // Note: this class's name doesn't match its file name, but it should only be changed when breaking backwards
    // compatibility by changing the package major version.
    public static class GoogleEditorMenu
    {
        /// <summary>
        /// Name of the top-level menu for use by Google plugins. Note: the <see cref="MenuItem"/> name parameter
        /// cannot be created with string formatting, but must be a constant created with string concatenation.
        /// </summary>
        public const string MainMenuName = "Google";

        /// <summary>
        /// Menu item name for opening a build settings window.
        /// </summary>
        public const string BuildSettings = "Build Settings...";

        /// <summary>
        /// Menu item name for viewing documentation.
        /// </summary>
        public const string ViewDocumentation = "View Documentation";

        /// <summary>
        /// Menu item name for viewing a license.
        /// </summary>
        public const string ViewLicense = "View License";

        /// <summary>
        /// Menu item name for filing a bug.
        /// </summary>
        public const string FileBug = "File a Bug";

        /// <summary>
        /// A number of priority steps between menu items to guarantee that a separator is shown.
        /// </summary>
        public const int SeparatorSize = 100;

        // A gap of 11 or more in menu item priority will cause a separator line between folders.
        // Keep these within 10, but leave a little room for others to be added later.
        public const int AndroidAppBundlePriority = 1;
        public const int PlayBillingPriority = 6;
        public const int PlayInstantPriority = 8;

        // We want a separator before the build options
        public const int RootMenuPriority = 1000;


        /// <summary>
        /// Displays the Google Play Plugins for Unity "Issues" page in a browser.
        /// </summary>
        public static void ViewPlayPluginsIssuesPage()
        {
            Application.OpenURL("https://github.com/google/play-unity-plugins/issues");
        }

        /// <summary>
        /// Displays a file in a browser, as specified by the file's guid.
        /// </summary>
        public static void OpenFileByGuid(string fileGuid)
        {
            var guidPath = AssetDatabase.GUIDToAssetPath(fileGuid);
            if (string.IsNullOrEmpty(guidPath))
            {
                DisplayErrorDialog(string.Format("Unable to locate the file with guid \"{0}\".", fileGuid));
                return;
            }

            var fileInfo = new FileInfo(guidPath);
            if (!fileInfo.Exists)
            {
                DisplayErrorDialog(string.Format("Unable to locate the file with path \"{0}\".", guidPath));
                return;
            }

            var uri = new Uri(fileInfo.FullName);
            Application.OpenURL(uri.ToString());
        }

        private static void DisplayErrorDialog(string message)
        {
            EditorUtility.DisplayDialog("File Error", message, WindowUtils.OkButtonText);
        }
    }
}