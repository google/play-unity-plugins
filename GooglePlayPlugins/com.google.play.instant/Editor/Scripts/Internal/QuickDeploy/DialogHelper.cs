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
using System.Linq;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal.QuickDeploy
{
    /// <summary>
    /// Utility class for displaying popup window messages.
    /// </summary>
    public static class DialogHelper
    {
        /// <summary>
        /// Displays a popup window that displays a message with a specified title.
        /// </summary>
        public static void DisplayMessage(string title, string message)
        {
            EditorUtility.DisplayDialog(title, message, WindowUtils.OkButtonText);
        }

        /// <summary>
        /// Displays a save dialog pointing to the default path.
        /// If the path points to a file then that file will be used as the default file name.
        /// </summary>
        /// <returns>The user selected path or null if the dialog is closed</returns>
        public static string SaveFilePanel(string title, string defaultPath, string extension)
        {
            string fileName;
            try
            {
                fileName = Path.GetFileName(defaultPath);
            }
            catch (ArgumentException)
            {
                fileName = "";
            }

            string directory;
            try
            {
                directory = Path.GetDirectoryName(defaultPath);
            }
            catch (ArgumentException)
            {
                directory = "";
            }

            return EditorUtility.SaveFilePanel(title, directory, fileName, extension);
        }

        /// <summary>
        /// Displays a save dialog pointing to the default path,
        /// and requires the user to select a path within the Assets folder.
        /// If the path points to a file then that file will be used as the default file name.
        /// </summary>
        /// <returns>The user selected path relative to the Assets folder, or null if the dialog is closed</returns>
        public static string SaveFilePanelInProject(string title, string defaultPath, string extension)
        {
            // We use this instead of EditorUtility.SaveFilePanelInProject,
            // because that function doesn't allow us to specify a default path.

            string saveFilePath = null;
            while (saveFilePath == null)
            {
                saveFilePath = SaveFilePanel(title, defaultPath, extension);
                if (string.IsNullOrEmpty(saveFilePath))
                {
                    // Assume cancelled.
                    return null;
                }

                saveFilePath = AbsoluteToRelativePath(saveFilePath, Application.dataPath);
                if (string.IsNullOrEmpty(saveFilePath))
                {
                    DisplayMessage("Need to save in the Assets folder",
                        "You need to save the file inside of the project's assets folder");
                }
            }

            return saveFilePath;
        }

        // Visible for testing.
        /// <summary>
        /// Converts the specified absolute path to a path relative to the specified parent path.
        /// Assumes that path directories are separated with forward slashes.
        /// Returns null if the path doesn't contain the specified parent path.
        /// </summary>
        public static string AbsoluteToRelativePath(string absolutePath, string parentPath)
        {
            if (string.IsNullOrEmpty(absolutePath) || string.IsNullOrEmpty(parentPath))
            {
                return null;
            }

            // Strip trailing slash.
            if (parentPath.Last() == '/')
            {
                parentPath = parentPath.Remove(parentPath.Length - 1);
            }

            if (!absolutePath.StartsWith(parentPath, StringComparison.Ordinal))
            {
                return null;
            }

            // If parentPath is Application.dataPath, this directory will be "Assets".
            var lastSharedDirectoryName = parentPath.Split('/').Last();
            if (absolutePath.Length == parentPath.Length)
            {
                return lastSharedDirectoryName;
            }

            var relativePath = absolutePath.Remove(0, parentPath.Length + 1);

            // Combine paths manually because Path.Combine will use backslashes on windows.
            return string.Format("{0}/{1}", lastSharedDirectoryName, relativePath);
        }
    }
}