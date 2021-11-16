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
using System.Collections.Generic;
using System.Linq;
using Google.Android.AppBundle.Editor.Internal.Config;
using UnityEditor;

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Provides helper methods for accessing or persisting build-related settings.
    /// </summary>
    public static class AndroidBuildHelper
    {
        // Allowed characters for splitting PlayerSettings.GetScriptingDefineSymbolsForGroup().
        private static readonly char[] ScriptingDefineSymbolsSplitChars = {';', ',', ' '};

        /// <summary>
        /// Returns an array of enabled scenes from the "Scenes In Build" section of Unity's Build Settings window.
        /// </summary>
        public static string[] GetEditorBuildEnabledScenes()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled && !string.IsNullOrEmpty(scene.path))
                .Select(scene => scene.path)
                .ToArray();
        }

        /// <summary>
        /// Creates a <see cref="BuildPlayerOptions"/> for the specified output file path with all other fields set to
        /// reasonable default values.
        /// </summary>
        /// <param name="locationPathName">
        /// The output file that will be created when building a player with <see cref="BuildPipeline"/>.
        /// </param>
        /// <returns>A new <see cref="BuildPlayerOptions"/> object.</returns>
        public static BuildPlayerOptions CreateBuildPlayerOptions(string locationPathName)
        {
            return new BuildPlayerOptions
            {
                assetBundleManifestPath = AndroidBuildConfiguration.AssetBundleManifestPath,
                locationPathName = locationPathName,
                options = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None,
                scenes = GetEditorBuildEnabledScenes(),
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android
            };
        }

        /// <summary>
        /// Adds the specified scripting define symbol for Android, but only if it isn't already defined.
        /// </summary>
        public static void AddScriptingDefineSymbol(string symbol)
        {
            var scriptingDefineSymbols = GetScriptingDefineSymbols();
            if (!IsScriptingSymbolDefined(scriptingDefineSymbols, symbol))
            {
                SetScriptingDefineSymbols(scriptingDefineSymbols.Concat(new[] {symbol}));
            }
        }

        /// <summary>
        /// Removes the specified scripting define symbol if it exists.
        /// </summary>
        public static void RemoveScriptingDefineSymbol(string symbol)
        {
            var scriptingDefineSymbols = GetScriptingDefineSymbols();
            if (IsScriptingSymbolDefined(scriptingDefineSymbols, symbol))
            {
                SetScriptingDefineSymbols(scriptingDefineSymbols.Where(sym => sym != symbol));
            }
        }

        /// <summary>
        /// Returns true if the specified scripting symbol is defined for Android.
        /// </summary>
        public static bool IsScriptingSymbolDefined(string symbol)
        {
            return IsScriptingSymbolDefined(GetScriptingDefineSymbols(), symbol);
        }

        private static bool IsScriptingSymbolDefined(string[] scriptingDefineSymbols, string symbol)
        {
            return Array.IndexOf(scriptingDefineSymbols, symbol) >= 0;
        }

        private static string[] GetScriptingDefineSymbols()
        {
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (string.IsNullOrEmpty(symbols))
            {
                return new string[0];
            }

            return symbols.Split(ScriptingDefineSymbolsSplitChars, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void SetScriptingDefineSymbols(IEnumerable<string> scriptingDefineSymbols)
        {
            var symbols = string.Join(";", scriptingDefineSymbols.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbols);
        }
    }
}