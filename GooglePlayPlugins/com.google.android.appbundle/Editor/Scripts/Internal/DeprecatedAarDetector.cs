// Copyright 2021 Google LLC
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

using System.IO;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using UnityEditor;

// IPreprocessBuild was deprecated in 2018.1 in favor of IPreprocessBuildWithReport.
#if UNITY_2018_1_OR_NEWER
using IPreprocessBuildCompat = UnityEditor.Build.IPreprocessBuildWithReport;
#else
using IPreprocessBuildCompat = UnityEditor.Build.IPreprocessBuild;
#endif

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Detects if removed AARs are still present in the project, and prompts the user to remove them.
    /// This can happen if an AAR has been removed in the latest update,
    /// and a user installs the .unitypackage containing that update on top of an existing installation.
    /// </summary>
    public class DeprecatedAarDetector : IPreprocessBuildCompat
    {
        public int callbackOrder { get; set; }

        /// <summary>
        /// Search for the playcore.aar file using the GUID so that we don't delete the one downloaded by EDM4U.
        /// </summary>
        private const string PlayCoreAarGuid = "554c76a0cd4cc49f9bcf95b2eae6616a";

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            DetectAar(report.summary.platform);
        }
#else
        public void OnPreprocessBuild(BuildTarget target, string outputPath)
        {
            DetectAar(target);
        }
#endif

        private void DetectAar(BuildTarget target)
        {
            if (target != BuildTarget.Android)
            {
                return;
            }

            var playCorePath = AssetDatabase.GUIDToAssetPath(PlayCoreAarGuid);
            if (string.IsNullOrEmpty(playCorePath))
            {
                return;
            }

            // Even after calling AssetDatabase.DeleteAsset, AssetDatabase.GUIDToAssetPath may return
            // the path of the asset before it was deleted. So we check here to make sure the file exists.
            if (!File.Exists(Path.GetFullPath(playCorePath)))
            {
                return;
            }

            var logger = new BuildToolLogger();
            var message =
                string.Format(
                    "Detected an outdated playcore.aar left over from a previous version at path:\n\n{0}.\n\nPress OK to delete it.",
                    playCorePath);

            var clickedOk = logger.DisplayActionableErrorDialog(message);

            if (clickedOk)
            {
                AssetDatabase.DeleteAsset(playCorePath);
            }
        }
    }
}