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
using System.Text.RegularExpressions;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Provides utilities related to
    /// <a href="https://developer.android.com/platform/technology/app-bundle/">Android App Bundle</a>.
    /// </summary>
    public static class AndroidAppBundle
    {
        /// <summary>
        /// Reserved name for the base module that contains the Unity game engine.
        /// </summary>
        public const string BaseModuleName = "base";

        /// <summary>
        /// Reserved name for the module that separates the base module's assets into an install-time asset pack.
        /// </summary>
        public const string BaseAssetsModuleName = "base_assets";

        /// <summary>
        /// Regex used to determine whether a module name is valid.
        /// See https://github.com/google/bundletool/blob/master/src/main/java/com/android/tools/build/bundletool/model/BundleModuleName.java#L38
        /// </summary>
        private static readonly Regex NameRegex = RegexHelper.CreateCompiled(@"^[a-zA-Z][a-zA-Z0-9_]*$");

        /// <summary>
        /// Returns true if the specified name is a valid Android App Bundle module name, false otherwise.
        /// Certain names like "base" are reserved, so also return false in those cases.
        /// </summary>
        public static bool IsValidModuleName(string name)
        {
            // TODO: enforce a name length limit if we make it much smaller than 65535.
            return name != null
                   && NameRegex.IsMatch(name)
                   && CheckReservedName(name, BaseModuleName)
                   && CheckReservedName(name, BaseAssetsModuleName);
        }

        /// <summary>
        /// Always returns true. Previously indicated if this version of the Unity Editor has native support for
        /// building an Android App Bundle.
        /// </summary>
        // TODO: Needed for 1.x API compatibility. Should be removed with 2.x.
        [Obsolete("This is always true")]
        public static bool HasNativeBuildSupport()
        {
            return true;
        }

        /// <summary>
        /// Returns EditorUserBuildSettings.buildAppBundle if it is defined and false otherwise.
        /// </summary>
        // TODO: Needed for 1.x API compatibility. Should be removed with 2.x.
        [Obsolete("Use EditorUserBuildSettings.buildAppBundle directly instead")]
        public static bool IsNativeBuildEnabled()
        {
            return EditorUserBuildSettings.buildAppBundle;
        }

        /// <summary>
        /// Enable the EditorUserBuildSettings.buildAppBundle field if it is defined.
        /// </summary>
        // TODO: Needed for 1.x API compatibility. Should be removed with 2.x.
        [Obsolete("Use EditorUserBuildSettings.buildAppBundle directly instead")]
        public static void EnableNativeBuild()
        {
            EditorUserBuildSettings.buildAppBundle = true;
        }

        /// <summary>
        /// Disable the EditorUserBuildSettings.buildAppBundle field if it is defined.
        /// </summary>
        // TODO: Needed for 1.x API compatibility. Should be removed with 2.x.
        [Obsolete("Use EditorUserBuildSettings.buildAppBundle directly instead")]
        public static void DisableNativeBuild()
        {
            EditorUserBuildSettings.buildAppBundle = false;
        }

        private static bool CheckReservedName(string name, string reserved)
        {
            return !name.Equals(reserved, StringComparison.OrdinalIgnoreCase);
        }
    }
}