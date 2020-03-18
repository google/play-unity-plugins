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

using Google.Android.AppBundle.Editor;

namespace Google.Play.Instant.Editor
{
    /// <summary>
    /// Provides methods for accessing and persisting Play Instant build settings.
    /// </summary>
    public static class PlayInstantBuildSettings
    {
        private const string PlayInstantScriptingDefineSymbol = "PLAY_INSTANT";

        /// <summary>
        /// Returns true if the currently selected build type is "Instant" or false if "Installed".
        /// This method can only be called on the Editor's main thread.
        /// </summary>
        public static bool IsInstantBuildType()
        {
            return AndroidBuildHelper.IsScriptingSymbolDefined(PlayInstantScriptingDefineSymbol);
        }

        /// <summary>
        /// Sets the instant build type. If true, changes the build type to instant and creates a "PLAY_INSTANT"
        /// scripting define symbol. If false, removes the "PLAY_INSTANT" scripting define symbol.
        /// This setting only affects builds started through <see cref="Bundletool"/> or the "Google" Editor menu.
        /// This method can only be called on the Editor's main thread.
        /// </summary>
        public static void SetInstantBuildType(bool instantBuild)
        {
            if (instantBuild)
            {
                AndroidBuildHelper.AddScriptingDefineSymbol(PlayInstantScriptingDefineSymbol);
            }
            else
            {
                AndroidBuildHelper.RemoveScriptingDefineSymbol(PlayInstantScriptingDefineSymbol);
            }
        }
    }
}