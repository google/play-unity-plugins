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
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Provides methods for identifying if a project is configured to use Unity's built-in Play Asset Delivery support for
    /// building Android App Bundles with asset packs.
    /// </summary>
    public static class BuiltInPadHelper
    {
        private const string AndroidPackDirectorySuffix = ".androidpack";

        /// <summary>
        /// Returns true if building the project with Unity's build system will produce an Android App Bundle with asset packs.
        /// </summary>
        public static bool ProjectUsesBuiltInPad()
        {
            return EditorSupportsPad() && EditorUserBuildSettings.buildAppBundle &&
                   (ProjectHasAndroidPacks() || PlayerSettings.Android.useAPKExpansionFiles);
        }

        /// <summary>
        /// True if the project has a mainTemplate.gradle file in its Assets/Plugins/Android folder.
        /// </summary>
        public static bool ProjectUsesGradleTemplate()
        {
            return File.Exists(Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle"));
        }

        /// <summary>
        /// Returns true if the project contains folders ending in ".androidpack".
        /// The .androidpack folder may optionally contain a build.gradle specifying the delivery mode.
        /// </summary>
        public static bool ProjectHasAndroidPacks()
        {
            // AssetDatabase.FindAssets is faster than searching the directories directly (e.g. Directory.EnumerateDirectories).
            var androidPackGuids = AssetDatabase.FindAssets(AndroidPackDirectorySuffix);
            foreach (var androidPackGuid in androidPackGuids)
            {
                var androidPackPath = AssetDatabase.GUIDToAssetPath(androidPackGuid);
                if (androidPackPath.EndsWith(AndroidPackDirectorySuffix))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the current version of Unity includes built-in support for building Android App Bundles with asset packs.
        /// </summary>
        public static bool EditorSupportsPad()
        {
#if UNITY_2021_1_0 || UNITY_2021_1_1 || UNITY_2021_1_2 || UNITY_2021_1_3 || UNITY_2021_1_4 || UNITY_2021_1_5 || UNITY_2021_1_6 || UNITY_2021_1_7 || UNITY_2021_1_8 || UNITY_2021_1_9 || UNITY_2021_1_10 || UNITY_2021_1_11 || UNITY_2021_1_12 || UNITY_2021_1_13 || UNITY_2021_1_14
            // Unity 2021 got built-in PAD support in version 2021.1.15
            return false;
#elif UNITY_2020_1 || UNITY_2020_2 || UNITY_2020_3_0 || UNITY_2020_3_1 || UNITY_2020_3_2 || UNITY_2020_3_3 || UNITY_2020_3_4 || UNITY_2020_3_5 || UNITY_2020_3_6 || UNITY_2020_3_7 || UNITY_2020_3_8 || UNITY_2020_3_9 || UNITY_2020_3_10 || UNITY_2020_3_11 || UNITY_2020_3_12 || UNITY_2020_3_13 || UNITY_2020_3_14
            // Unity 2020 got built-in PAD support in version 2020.3.15
            return false;
#elif UNITY_2019_1|| UNITY_2019_2 || UNITY_2019_3 || UNITY_2019_4_0 || UNITY_2019_4_1 || UNITY_2019_4_2 || UNITY_2019_4_3 || UNITY_2019_4_4 || UNITY_2019_4_5 || UNITY_2019_4_6 || UNITY_2019_4_7 || UNITY_2019_4_8 || UNITY_2019_4_9 || UNITY_2019_4_10 || UNITY_2019_4_11 || UNITY_2019_4_12 || UNITY_2019_4_13 || UNITY_2019_4_14 || UNITY_2019_4_15 || UNITY_2019_4_16 || UNITY_2019_4_17 || UNITY_2019_4_18 || UNITY_2019_4_19 || UNITY_2019_4_20 || UNITY_2019_4_21 || UNITY_2019_4_22 || UNITY_2019_4_23 || UNITY_2019_4_24 || UNITY_2019_4_25 || UNITY_2019_4_26 || UNITY_2019_4_27 || UNITY_2019_4_28
            // Unity 2019 got built-in PAD support in version 2019.4.29
            return false;
#elif !UNITY_2019_1_OR_NEWER
            // Unity versions 2018 and below do not have built-in PAD support
            return false;
#else
            return true;
#endif
        }
    }
}