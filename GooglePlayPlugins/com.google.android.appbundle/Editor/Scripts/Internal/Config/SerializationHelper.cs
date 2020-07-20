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
using System.Linq;

namespace Google.Android.AppBundle.Editor.Internal.Config
{
    public static class SerializationHelper
    {
        public const string ConfigurationFilePath = "Library/PlayAssetPackConfig.json";

        public static AssetPackDeliveryMode GetAssetPackDeliveryMode(string deliveryMode)
        {
            return (AssetPackDeliveryMode) Enum.Parse(typeof(AssetPackDeliveryMode), deliveryMode);
        }

        public static TextureCompressionFormat GetTextureCompressionFormat(string textureCompressionFormat)
        {
            return (TextureCompressionFormat) Enum.Parse(typeof(TextureCompressionFormat), textureCompressionFormat);
        }

        public static SerializableAssetPackConfig Serialize(AssetPackConfig assetPackConfig)
        {
            var config = new SerializableAssetPackConfig
            {
                DefaultTextureCompressionFormat = assetPackConfig.DefaultTextureCompressionFormat
            };

            foreach (var assetPackEntry in assetPackConfig.AssetPacks)
            {
                var name = assetPackEntry.Key;
                var assetPack = assetPackEntry.Value;
                if (assetPack.AssetBundleFilePath != null)
                {
                    var assetBundle = new SerializableMultiTargetingAssetBundle
                    {
                        name = name, DeliveryMode = assetPack.DeliveryMode
                    };
                    assetBundle.assetBundles.Add(new SerializableAssetBundle
                    {
                        path = assetPack.AssetBundleFilePath,
                        TextureCompressionFormat = TextureCompressionFormat.Default
                    });

                    config.assetBundles.Add(assetBundle);
                }

                if (assetPack.CompressionFormatToAssetBundleFilePath != null)
                {
                    var assetBundle = new SerializableMultiTargetingAssetBundle
                    {
                        name = name, DeliveryMode = assetPack.DeliveryMode
                    };
                    foreach (var compressionEntry in assetPack.CompressionFormatToAssetBundleFilePath)
                    {
                        assetBundle.assetBundles.Add(new SerializableAssetBundle
                        {
                            path = compressionEntry.Value,
                            TextureCompressionFormat = compressionEntry.Key
                        });
                    }

                    config.assetBundles.Add(assetBundle);
                }

                if (assetPack.AssetPackDirectoryPath != null)
                {
                    config.assetPacks.Add(new SerializableAssetPack
                    {
                        name = name, DeliveryMode = assetPack.DeliveryMode, path = assetPack.AssetPackDirectoryPath
                    });
                }

                if (assetPack.CompressionFormatToAssetPackDirectoryPath != null)
                {
                    var multiTargetingAssetPack = new SerializableMultiTargetingAssetPack
                    {
                        name = name, DeliveryMode = assetPack.DeliveryMode
                    };
                    foreach (var compressionEntry in assetPack.CompressionFormatToAssetPackDirectoryPath)
                    {
                        multiTargetingAssetPack.paths.Add(new SerializableTargetedDirectoryPath
                        {
                            path = compressionEntry.Value,
                            TextureCompressionFormat = compressionEntry.Key
                        });
                    }

                    config.targetedAssetPacks.Add(multiTargetingAssetPack);
                }
            }

            return config;
        }

        public static AssetPackConfig Deserialize(SerializableAssetPackConfig config)
        {
            var assetPackConfig = new AssetPackConfig
            {
                DefaultTextureCompressionFormat = config.DefaultTextureCompressionFormat
            };

            foreach (var multiTargetingAssetBundle in config.assetBundles)
            {
                var assetBundles = multiTargetingAssetBundle.assetBundles;
                if (assetBundles.Count == 0)
                {
                    continue;
                }

                // TODO: consider checking the folder name for "#tcf".
                if (assetBundles.Count == 1 &&
                    assetBundles[0].TextureCompressionFormat == TextureCompressionFormat.Default)
                {
                    assetPackConfig.AddAssetBundle(assetBundles[0].path, multiTargetingAssetBundle.DeliveryMode);
                    continue;
                }

                var dictionary = assetBundles.ToDictionary(item => item.TextureCompressionFormat, item => item.path);
                assetPackConfig.AddAssetBundles(dictionary, multiTargetingAssetBundle.DeliveryMode);
            }

            foreach (var pack in config.assetPacks)
            {
                assetPackConfig.AddAssetsFolder(pack.name, pack.path, pack.DeliveryMode);
            }

            foreach (var pack in config.targetedAssetPacks)
            {
                var compressionFormatToAssetPackDirectoryPath =
                    pack.paths.ToDictionary(item => item.TextureCompressionFormat, item => item.path);
                assetPackConfig.AddAssetsFolders(pack.name, compressionFormatToAssetPackDirectoryPath,
                    pack.DeliveryMode);
            }

            return assetPackConfig;
        }
    }
}
