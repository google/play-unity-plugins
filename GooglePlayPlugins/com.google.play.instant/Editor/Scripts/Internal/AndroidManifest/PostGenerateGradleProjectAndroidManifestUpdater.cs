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

#if UNITY_2018_1_OR_NEWER
using System.IO;
using System.Xml.Linq;
using Google.Android.AppBundle.Editor.Internal.AndroidManifest;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal.AndroidManifest
{
    /// <summary>
    /// An IAndroidManifestUpdater for Unity 2018+ that obtains the AndroidManifest.xml after it is fully merged
    /// but before the build occurs and updates it according to whether this is a Play Instant build.
    /// </summary>
    public class PostGenerateGradleProjectAndroidManifestUpdater : IAndroidManifestUpdater,
        IPostGenerateGradleAndroidProject
    {
        // The path to the base manifest changed as part of Unity's "use Unity as a library" feature:
        // https://blogs.unity3d.com/2019/06/17/add-features-powered-by-unity-to-native-mobile-apps/
        private const string AndroidManifestRelativePath =
#if UNITY_2019_3_OR_NEWER
            // Expected gradle project path: "Temp/gradleOut/unityLibrary"
            "../launcher/src/main/AndroidManifest.xml";
#else
            // Expected gradle project path: "Temp/gradleOut"
            "src/main/AndroidManifest.xml";
#endif

        private const string PlayCoreGuid = "554c76a0cd4cc49f9bcf95b2eae6616a";

        private const string PlayCoreDialogWrapperActivity =
            "com.google.android.play.core.common.PlayCoreDialogWrapperActivity";

        private const string AssetPackExtractionService =
            "com.google.android.play.core.assetpacks.AssetPackExtractionService";

        public int callbackOrder
        {
            get { return 100; }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (!PlayInstantBuildSettings.IsInstantBuildType())
            {
                return;
            }

            var buildToolLogger = new BuildToolLogger();

            // Update the final merged AndroidManifest.xml prior to the gradle build.
            var manifestPath = Path.Combine(path, AndroidManifestRelativePath);
            Debug.LogFormat("Updating manifest for Play Instant: {0}", manifestPath);
            var doc = XDocument.Load(manifestPath);
            var errorMessage =
                AndroidManifestHelper.ConvertManifestToInstant(doc, PlayInstantBuildConfig.PlayGamesEnabled);
            if (errorMessage != null)
            {
                DisplayErrorDialog(buildToolLogger, errorMessage);
                return;
            }

            errorMessage = ApplyPlayCoreInstantWorkaround(doc);
            if (errorMessage != null)
            {
                DisplayErrorDialog(buildToolLogger, errorMessage);
                return;
            }

            doc.Save(manifestPath);
        }

        /// <summary>
        /// Enables play core components by default in the manifest, if the play core library exists.
        /// </summary>
        private static string ApplyPlayCoreInstantWorkaround(XDocument doc)
        {
            var playCoreImporter =
                AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(PlayCoreGuid)) as PluginImporter;

            if (playCoreImporter == null)
            {
                return null;
            }

            if (!playCoreImporter.GetCompatibleWithPlatform(BuildTarget.Android))
            {
                return null;
            }

            var applicationElement = AndroidManifestHelper.GetApplicationXElement(doc);
            if (applicationElement == null)
            {
                return "missing application element";
            }

            applicationElement.Add(AndroidManifestHelper.GetReplaceComponentElement(PlayCoreDialogWrapperActivity,
                ManifestConstants.Activity));
            applicationElement.Add(AndroidManifestHelper.GetReplaceComponentElement(AssetPackExtractionService,
                ManifestConstants.Service));
            return null;
        }

        private static void DisplayErrorDialog(BuildToolLogger buildToolLogger, string errorMessage)
        {
            buildToolLogger.DisplayErrorDialog(
                string.Format("Error updating AndroidManifest.xml: {0}", errorMessage));
        }

        public string SwitchToInstant()
        {
            // Unused on 2018+
            return null;
        }

        public void SwitchToInstalled()
        {
            // Unused on 2018+
        }
    }
}
#endif