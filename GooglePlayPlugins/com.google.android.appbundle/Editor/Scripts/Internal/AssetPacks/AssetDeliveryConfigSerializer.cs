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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Android.AppBundle.Editor.Internal.Config;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Provides methods for serializing <see cref="AssetDeliveryConfig"/> to a JSON file and for deserializing it.
    /// </summary>
    public static class AssetDeliveryConfigSerializer
    {
        /// <summary>
        /// Save the specified AssetDeliveryConfig config to disk.
        /// </summary>
        public static void SaveConfig(AssetDeliveryConfig assetDeliveryConfig)
        {
            Debug.LogFormat("Saving {0}", SerializationHelper.ConfigurationFilePath);
            var config = Serialize(assetDeliveryConfig);
            var jsonText = JsonUtility.ToJson(config);
            File.WriteAllText(SerializationHelper.ConfigurationFilePath, jsonText);
        }

        /// <summary>
        /// Returns an AssetDeliveryConfig loaded from disk.
        /// </summary>
        public static AssetDeliveryConfig LoadConfig()
        {
            Debug.LogFormat("Loading {0}", SerializationHelper.ConfigurationFilePath);
            SerializableAssetPackConfig config;
            if (File.Exists(SerializationHelper.ConfigurationFilePath))
            {
                var jsonText = File.ReadAllText(SerializationHelper.ConfigurationFilePath);
                config = JsonUtility.FromJson<SerializableAssetPackConfig>(jsonText);
            }
            else
            {
                config = new SerializableAssetPackConfig();
            }

            return Deserialize(config);
        }

        private static SerializableAssetPackConfig Serialize(AssetDeliveryConfig assetDeliveryConfig)
        {
            var config = new SerializableAssetPackConfig
            {
                DefaultTextureCompressionFormat = assetDeliveryConfig.DefaultTextureCompressionFormat
            };

            foreach (var assetPack in assetDeliveryConfig.AssetBundlePacks.Values)
            {
                config.assetBundles.Add(new SerializableMultiTargetingAssetBundle
                {
                    name = assetPack.Name,
                    DeliveryMode = assetPack.DeliveryMode,
                    assetBundles = assetPack.Variants.Select(pack => new SerializableAssetBundle
                    {
                        path = pack.Value.Path,
                        TextureCompressionFormat = pack.Key
                    }).ToList()
                });
            }

            config.assetPacks = assetDeliveryConfig.rawAssetsPacks;

            return config;
        }

        private static AssetDeliveryConfig Deserialize(SerializableAssetPackConfig config)
        {
            var assetDeliveryConfig = new AssetDeliveryConfig
            {
                DefaultTextureCompressionFormat = config.DefaultTextureCompressionFormat
            };

            var paths = new HashSet<string>();
            foreach (var multiTargetingAssetBundle in config.assetBundles)
            {
                paths.UnionWith(
                    multiTargetingAssetBundle.assetBundles.Select(item => Path.GetDirectoryName(item.path)));
                assetDeliveryConfig.AssetBundlePacks.Add(multiTargetingAssetBundle.name,
                    new AssetBundlePack(multiTargetingAssetBundle.name,
                        multiTargetingAssetBundle.DeliveryMode));
            }

            paths.ToList().ForEach(path => assetDeliveryConfig.Folders.Add(path, new AssetBundleFolder(path)));

            assetDeliveryConfig.rawAssetsPacks = config.assetPacks;

            return assetDeliveryConfig;
        }
    }
}