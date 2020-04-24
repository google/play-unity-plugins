// Copyright 2020 Google LLC
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

namespace Google.Play.Instant.Editor.Internal
{
    /// <summary>
    /// Provides methods for accessing and persisting Play Instant build settings.
    /// </summary>
    public static class PlayInstantBuildConfig
    {
        /// <summary>
        /// Configuration class that is serialized as JSON. See associated properties for field documentation.
        /// </summary>
        [Serializable]
        private class Configuration
        {
            public bool playGamesEnabled;
        }

        private static readonly string ConfigurationFilePath =
            Path.Combine("Library", "PlayInstantBuildConfiguration.json");

        // Holds an in-memory copy of configuration for quick access.
        private static Configuration _config;

        /// <summary>
        /// Whether or not the instant app is claiming "Instant play" support for the Play Games App.
        /// See https://developer.android.com/topic/google-play-instant/instant-play-games
        /// https://developer.android.com/topic/google-play-instant/instant-play-games-checklist
        /// </summary>
        public static bool PlayGamesEnabled
        {
            get
            {
                LoadConfigIfNecessary();
                return _config.playGamesEnabled;
            }
        }

        /// <summary>
        /// Persists the specified configuration to disk.
        /// </summary>
        public static void SaveConfiguration(bool playGamesEnabled)
        {
            _config = _config ?? new Configuration();
            _config.playGamesEnabled = playGamesEnabled;
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