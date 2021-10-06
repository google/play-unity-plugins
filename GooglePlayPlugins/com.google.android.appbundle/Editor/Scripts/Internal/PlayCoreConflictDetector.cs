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
using System.Linq;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using UnityEditor.Android;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal
{
#if UNITY_2019_4_OR_NEWER
    /// <summary>
    /// When Unity uses its built-in system to build Android App Bundle with asset packs, it adds Play Core as a gradle
    /// dependency. This will conflict with the Play Core version specified in the com.google.play.core plugin unless
    /// EDM4U resolves the conflict by modifying the mainTemplate.gradle file.
    ///
    /// This class checks if a build is using Unity's built-in PAD system and provides an instructional dialog if it
    /// detects that a conflict will occur.
    /// </summary>
    public class PlayCoreConflictDetector : IPostGenerateGradleAndroidProject
    {
        const string PlayCoreAarSearchPattern = "com.google.android.play.core*.aar";

        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (!BuiltInPadHelper.ProjectUsesBuiltInPad())
            {
                return;
            }

            if (BuiltInPadHelper.ProjectUsesGradleTemplate())
            {
                return;
            }

            var projectPath = Path.GetDirectoryName(Application.dataPath);
            var gradleLibFolder = Path.Combine(projectPath, path, "libs");
            if (!Directory.Exists(gradleLibFolder))
            {
                return;
            }

            if (!Directory.EnumerateFiles(gradleLibFolder, PlayCoreAarSearchPattern).Any())
            {
                return;
            }

            var logger = new BuildToolLogger();
            logger.DisplayErrorDialog(
                "Detected Play Core library conflict. To resolve the conflict, enable \"Custom Main Gradle Template\" in " +
                "\"Android Player > Publishing Settings\" and enable \"Patch mainTemplate.gradle\" in \"Assets > External " +
                "Dependency Manager > Android Resolver > Settings\"");
        }
    }
#endif
}