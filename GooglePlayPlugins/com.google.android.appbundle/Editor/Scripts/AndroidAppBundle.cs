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

// In the Unity 2017 series the EditorUserBuildSettings.buildAppBundle field was introduced in 2017.4.17.
// It might seem preferable to modify buildAppBundle using reflection, but the field is extern.
// Instead check for quite a few versions in the 2017.4.17+ series.
// NOTE: this supports up to UNITY_2017_4_50 and will have to be extended if additional versions are released.

#if UNITY_2018_3_OR_NEWER || UNITY_2017_4_17 || UNITY_2017_4_18 || UNITY_2017_4_19 || UNITY_2017_4_20 || UNITY_2017_4_21 || UNITY_2017_4_22 || UNITY_2017_4_23 || UNITY_2017_4_24 || UNITY_2017_4_25 || UNITY_2017_4_26 || UNITY_2017_4_27 || UNITY_2017_4_28 || UNITY_2017_4_29 || UNITY_2017_4_30 || UNITY_2017_4_31 || UNITY_2017_4_32 || UNITY_2017_4_33 || UNITY_2017_4_34 || UNITY_2017_4_35 || UNITY_2017_4_36 || UNITY_2017_4_37 || UNITY_2017_4_38 || UNITY_2017_4_39 || UNITY_2017_4_40 || UNITY_2017_4_41 || UNITY_2017_4_42 || UNITY_2017_4_43 || UNITY_2017_4_44 || UNITY_2017_4_45 || UNITY_2017_4_46 || UNITY_2017_4_47 || UNITY_2017_4_48 || UNITY_2017_4_49 || UNITY_2017_4_50
#define GOOGLE_ANDROID_APP_BUNDLE_HAS_NATIVE_SUPPORT
using UnityEditor;
#endif
using System;
using System.Text.RegularExpressions;
using Google.Android.AppBundle.Editor.Internal.Utils;

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Provides utilities related to
    /// <a href="https://developer.android.com/platform/technology/app-bundle/">Android App Bundle</a>.
    /// </summary>
    public static class AndroidAppBundle
    {
        /// <summary>
        /// The base module contains the Unity game engine.
        /// </summary>
        public const string BaseModuleName = "base";

        /// <summary>
        /// Regex used to determine whether a module name is valid.
        /// See https://github.com/google/bundletool/blob/master/src/main/java/com/android/tools/build/bundletool/model/BundleModuleName.java#L38
        /// </summary>
        private static readonly Regex NameRegex = RegexHelper.CreateCompiled(@"^[a-zA-Z][a-zA-Z0-9_]*$");

        /// <summary>
        /// Returns true if the specified name is a valid Android App Bundle module name, false otherwise.
        /// The name "base" is reserved for the base module, so also return false in that case.
        /// </summary>
        public static bool IsValidModuleName(string name)
        {
            // TODO: enforce a name length limit if we make it much smaller than 65535.
            return name != null
                   && NameRegex.IsMatch(name)
                   && !name.Equals(BaseModuleName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns true if this version of the Unity Editor has native support for building an Android App Bundle,
        /// and false otherwise.
        /// </summary>
        public static bool HasNativeBuildSupport()
        {
#if GOOGLE_ANDROID_APP_BUNDLE_HAS_NATIVE_SUPPORT
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Returns EditorUserBuildSettings.buildAppBundle if it is defined and false otherwise.
        /// </summary>
        public static bool IsNativeBuildEnabled()
        {
#if GOOGLE_ANDROID_APP_BUNDLE_HAS_NATIVE_SUPPORT
            return EditorUserBuildSettings.buildAppBundle;
#else
            return false;
#endif
        }

        /// <summary>
        /// Enable the EditorUserBuildSettings.buildAppBundle field if it is defined.
        /// </summary>
        public static void EnableNativeBuild()
        {
#if GOOGLE_ANDROID_APP_BUNDLE_HAS_NATIVE_SUPPORT
            EditorUserBuildSettings.buildAppBundle = true;
#endif
        }

        /// <summary>
        /// Disable the EditorUserBuildSettings.buildAppBundle field if it is defined.
        /// </summary>
        public static void DisableNativeBuild()
        {
#if GOOGLE_ANDROID_APP_BUNDLE_HAS_NATIVE_SUPPORT
            EditorUserBuildSettings.buildAppBundle = false;
#endif
        }
    }
}
