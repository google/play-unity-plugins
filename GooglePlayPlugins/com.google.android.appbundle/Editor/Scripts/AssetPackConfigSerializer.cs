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

using System.IO;
using Google.Android.AppBundle.Editor.Internal.Config;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.AssetPacks
{
    /// <summary>
    /// Provides methods for serializing <see cref="AssetPackConfig"/> to a JSON file and for deserializing the config.
    /// </summary>
    public static class AssetPackConfigSerializer
    {
        /// <summary>
        /// Save the specified <see cref="AssetPackConfig"/> config to disk.
        /// </summary>
        public static void SaveConfig(
            AssetPackConfig assetPackConfig, string configurationFilePath = SerializationHelper.ConfigurationFilePath)
        {
            Debug.LogFormat("Saving {0}", configurationFilePath);
            var config = SerializationHelper.Serialize(assetPackConfig);
            var jsonText = JsonUtility.ToJson(config);
            File.WriteAllText(configurationFilePath, jsonText);
        }

        /// <summary>
        /// Returns an <see cref="AssetPackConfig"/> loaded from disk.
        /// </summary>
        public static AssetPackConfig LoadConfig()
        {
            if (!File.Exists(SerializationHelper.ConfigurationFilePath))
            {
                Debug.LogFormat("Creating new config {0}", SerializationHelper.ConfigurationFilePath);
                return new AssetPackConfig();
            }

            Debug.LogFormat("Loading config {0}", SerializationHelper.ConfigurationFilePath);
            var jsonText = File.ReadAllText(SerializationHelper.ConfigurationFilePath);
            var config = JsonUtility.FromJson<SerializableAssetPackConfig>(jsonText);
            return SerializationHelper.Deserialize(config);
        }

        /// <summary>
        /// Returns an <see cref="AssetPackConfig"/> loaded from the specified file on disk.
        /// </summary>
        public static AssetPackConfig LoadConfig(string configurationFilePath)
        {
            Debug.LogFormat("Loading custom config {0}", configurationFilePath);
            if (!File.Exists(configurationFilePath))
            {
                throw new FileNotFoundException("AssetPackConfig not found", configurationFilePath);
            }

            var jsonText = File.ReadAllText(configurationFilePath);
            var config = JsonUtility.FromJson<SerializableAssetPackConfig>(jsonText);
            return SerializationHelper.Deserialize(config);
        }

        /// <summary>
        /// Delete config file.
        /// </summary>
        public static void DeleteConfig()
        {
            File.Delete(SerializationHelper.ConfigurationFilePath);
        }
    }
}