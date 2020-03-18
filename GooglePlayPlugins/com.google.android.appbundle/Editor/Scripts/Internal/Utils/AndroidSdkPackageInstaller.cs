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

using System.Collections.Generic;
using Google.Android.AppBundle.Editor.Internal.PlayServices;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.Utils
{
    /// <summary>
    /// Utility class that uses <see cref="AndroidSdkManager"/> to install Android SDK packages.
    /// </summary>
    public static class AndroidSdkPackageInstaller
    {
        /// <summary>
        /// Installs the specified Android SDK package.
        /// </summary>
        public static void InstallPackage(string packageName, string displayName)
        {
            // In many cases the SDK install is quick and clearly indicates download/install progress. However, on
            // some systems it can take a couple minutes and not indicate progress - set expectations with a dialog.
            // TODO: figure out if we can target this message only for when it is needed.
            const string message =
                "On some systems the install process is slow and does not provide feedback, " +
                "which may lead you to believe that Unity has frozen or crashed.\n\n" +
                "Click \"OK\" to continue.";
            if (!EditorUtility.DisplayDialog(
                "Install Note", message, WindowUtils.OkButtonText, WindowUtils.CancelButtonText))
            {
                Debug.LogFormat("Cancelled install of {0}", displayName);
                return;
            }

            AndroidSdkManager.Create(manager =>
            {
                manager.QueryPackages(collection =>
                {
                    var package = collection.GetMostRecentAvailablePackage(packageName);
                    if (package == null)
                    {
                        ShowMessage(string.Format("Unable to locate the {0} package", displayName));
                        return;
                    }

                    if (package.Installed)
                    {
                        ShowMessage(string.Format(
                            "The {0} package is already installed at the latest available version ({1})",
                            displayName, package.VersionString));
                        return;
                    }

                    var packages = new HashSet<AndroidSdkPackageNameVersion> {package};
                    manager.InstallPackages(packages, success =>
                    {
                        if (success)
                        {
                            ShowMessage(string.Format("Successfully updated the {0} package to version {1}",
                                displayName, package.VersionString));
                        }
                        else
                        {
                            ShowMessage(string.Format("Failed to install the {0} package", displayName));
                        }
                    });
                });
            });
        }

        private static void ShowMessage(string message)
        {
            Debug.LogFormat("AndroidSdkPackageInstaller: {0}", message);
            EditorUtility.DisplayDialog("Android SDK Package Installer", message, WindowUtils.OkButtonText);
        }
    }
}