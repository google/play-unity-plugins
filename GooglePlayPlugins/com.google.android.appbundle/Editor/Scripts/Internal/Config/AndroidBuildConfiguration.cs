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
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.Config
{
    /// <summary>
    /// Provides methods for accessing and persisting build settings.
    /// </summary>
    public static class AndroidBuildConfiguration
    {
        /// <summary>
        /// Configuration class that is serialized as JSON. See associated properties for field documentation.
        /// </summary>
        [Serializable]
        private class Configuration
        {
            public string assetBundleManifestPath;
        }

        private static readonly string ConfigurationFilePath = Path.Combine("Library", "AndroidBuildConfig.json");

        // Holds an in-memory copy of configuration for quick access.
        private static Configuration _config;

        /// <summary>
        /// Optional field used to prevent removal of required components when building with engine stripping.
        /// <a href="https://docs.unity3d.com/ScriptReference/BuildPlayerOptions-assetBundleManifestPath.html"/>
        /// Never null.
        /// </summary>
        public static string AssetBundleManifestPath
        {
            get
            {
                LoadConfigIfNecessary();
                return _config.assetBundleManifestPath ?? string.Empty;
            }
        }

        /// <summary>
        /// Persists the specified configuration to disk.
        /// </summary>
        public static void SaveConfiguration(string assetBundleManifestPath)
        {
            _config = _config ?? new Configuration();
            _config.assetBundleManifestPath = assetBundleManifestPath;
            File.WriteAllText(ConfigurationFilePath, JsonUtility.ToJson(_config));
        }

        private static void LoadConfigIfNecessary()
        {
            if (_config != null)
            {
                return;
            }

            LoadConfig(ConfigurationFilePath);
            _config = _config ?? new Configuration();
        }

        private static void LoadConfig(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            try
            {
                var configurationJson = File.ReadAllText(path);
                _config = JsonUtility.FromJson<Configuration>(configurationJson);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Failed to load {0} due to exception: {1}", path, ex);
            }
        }
    }
}