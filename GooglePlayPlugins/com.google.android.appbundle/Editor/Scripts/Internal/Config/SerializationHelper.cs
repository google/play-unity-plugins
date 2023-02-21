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
using System.Collections.Generic;
using System.Linq;

namespace Google.Android.AppBundle.Editor.Internal.Config
{
    public static class SerializationHelper
    {
        public const string ConfigurationFilePath = "Library/PlayAssetPackConfig.json";

        public static AssetPackDeliveryMode GetAssetPackDeliveryMode(string deliveryMode)
        {
            return (AssetPackDeliveryMode)Enum.Parse(typeof(AssetPackDeliveryMode), deliveryMode);
        }

        public static TextureCompressionFormat GetTextureCompressionFormat(string textureCompressionFormat)
        {
            return (TextureCompressionFormat)Enum.Parse(typeof(TextureCompressionFormat), textureCompressionFormat);
        }

        public static DeviceTier GetDeviceTier(string deviceTier)
        {
            int parsedTier;
            if (Int32.TryParse(deviceTier, out parsedTier))
            {
                return DeviceTier.From(parsedTier);
            }

            return null;
        }


        /// <summary>
        /// Returns a deep copy of the specified <see cref="AssetPackConfig"/>.
        /// </summary>
        /// <param name="assetPackConfig">The AssetPackConfig to copy.</param>
        /// <returns>A new copy of the original AssetPackConfig.</returns>
        public static AssetPackConfig DeepCopy(AssetPackConfig assetPackConfig)
        {
            // We copy the object's fields explicitly instead of calling Deserialize(Serialize(config)) because
            // Deserialize performs checks and transformations that can produce an inexact copy.
            // See https://github.com/google/play-unity-plugins/issues/143 for one such case.
            var copy = new AssetPackConfig
            {
                SplitBaseModuleAssets = assetPackConfig.SplitBaseModuleAssets,
                DefaultTextureCompressionFormat = assetPackConfig.DefaultTextureCompressionFormat,
                DefaultDeviceTier = assetPackConfig.DefaultDeviceTier,
            };

            foreach (var pair in assetPackConfig.AssetPacks)
            {
                var assetPack = pair.Value;
                var assetPackCopy = new AssetPack
                {
                    DeliveryMode = assetPack.DeliveryMode,
                    AssetBundleFilePath = assetPack.AssetBundleFilePath,
                    AssetPackDirectoryPath = assetPack.AssetPackDirectoryPath,
                    CompressionFormatToAssetBundleFilePath =
                        CopyDictionary(assetPack.CompressionFormatToAssetBundleFilePath),
                    CompressionFormatToAssetPackDirectoryPath =
                        CopyDictionary(assetPack.CompressionFormatToAssetPackDirectoryPath),
                    DeviceTierToAssetBundleFilePath = assetPack.DeviceTierToAssetBundleFilePath,
                    DeviceTierToAssetPackDirectoryPath = assetPack.DeviceTierToAssetPackDirectoryPath,
                };
                copy.AssetPacks.Add(pair.Key, assetPackCopy);
            }

            return copy;
        }

        public static SerializableAssetPackConfig Serialize(AssetPackConfig assetPackConfig)
        {
            var config = new SerializableAssetPackConfig
            {
                DefaultTextureCompressionFormat = assetPackConfig.DefaultTextureCompressionFormat,
                splitBaseModuleAssets = assetPackConfig.SplitBaseModuleAssets
            };

            config.DefaultDeviceTier = assetPackConfig.DefaultDeviceTier;


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

                if (assetPack.DeviceTierToAssetBundleFilePath != null)
                {
                    var assetBundle = new SerializableMultiTargetingAssetBundle
                    {
                        name = name, DeliveryMode = assetPack.DeliveryMode
                    };
                    foreach (var deviceTierEntry in assetPack.DeviceTierToAssetBundleFilePath)
                    {
                        assetBundle.assetBundles.Add(new SerializableAssetBundle
                        {
                            path = deviceTierEntry.Value,
                            DeviceTier = deviceTierEntry.Key
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

                if (assetPack.DeviceTierToAssetPackDirectoryPath != null)
                {
                    var multiTargetingAssetPack = new SerializableMultiTargetingAssetPack
                    {
                        name = name, DeliveryMode = assetPack.DeliveryMode
                    };
                    foreach (var deviceTierEntry in assetPack.DeviceTierToAssetPackDirectoryPath)
                    {
                        multiTargetingAssetPack.paths.Add(new SerializableTargetedDirectoryPath
                        {
                            path = deviceTierEntry.Value,
                            DeviceTier = deviceTierEntry.Key
                        });
                    }

                    config.targetedAssetPacks.Add(multiTargetingAssetPack);
                }

            }

            return config;
        }

        /// <summary>
        /// Creates an AssetPackConfig from the specified SerializableAssetPackConfig, validating its fields
        /// in the process. Note: AssetBundle names are interpreted from the AssetBundle path rather than
        /// the specified names.
        /// </summary>
        public static AssetPackConfig Deserialize(SerializableAssetPackConfig config)
        {
            var assetPackConfig = new AssetPackConfig
            {
                DefaultTextureCompressionFormat = config.DefaultTextureCompressionFormat,
                SplitBaseModuleAssets = config.splitBaseModuleAssets
            };
            assetPackConfig.DefaultDeviceTier = config.DefaultDeviceTier;


            foreach (var multiTargetingAssetBundle in config.assetBundles)
            {
                var assetBundles = multiTargetingAssetBundle.assetBundles;
                if (assetBundles.Count == 0)
                {
                    continue;
                }

                // TODO: consider checking the folder name for "#tcf".
                if (assetBundles.Count == 1 &&
                    assetBundles[0].TextureCompressionFormat == TextureCompressionFormat.Default
                    && assetBundles[0].DeviceTier == null)
                {
                    assetPackConfig.AddAssetBundle(assetBundles[0].path, multiTargetingAssetBundle.DeliveryMode);
                    continue;
                }

                // AssetBundles inside SerializableMultiTargetingAssetBundle are targeting only on one dimension.
                // Check out serialize function for more details.
                var dictionaryTextureCompression =
                    assetBundles.Any(assetBundle =>
                        assetBundle.TextureCompressionFormat != TextureCompressionFormat.Default)
                        ? assetBundles
                            .ToDictionary(item => item.TextureCompressionFormat, item => item.path)
                        : new Dictionary<TextureCompressionFormat, string>();
                if (dictionaryTextureCompression.Count != 0)
                {
                    assetPackConfig.AddAssetBundles(dictionaryTextureCompression,
                        multiTargetingAssetBundle.DeliveryMode);
                }

                var dictionaryDeviceTier =
                    assetBundles.Any(assetBundle => assetBundle.DeviceTier != null)
                        ? assetBundles
                            .ToDictionary(item => item.DeviceTier, item => item.path)
                        : new Dictionary<DeviceTier, string>();
                if (dictionaryDeviceTier.Count != 0)
                {
                    assetPackConfig.AddAssetBundles(dictionaryDeviceTier, multiTargetingAssetBundle.DeliveryMode);
                }

            }

            foreach (var pack in config.assetPacks)
            {
                assetPackConfig.AddAssetsFolder(pack.name, pack.path, pack.DeliveryMode);
            }

            foreach (var pack in config.targetedAssetPacks)
            {
                // Asset packs inside SerializableMultiTargetingAssetPack are targeting only on one dimension.
                // Check out serialize function for more details.
                var compressionFormatToAssetPackDirectoryPath =
                    pack.paths.Any(path => path.TextureCompressionFormat != TextureCompressionFormat.Default)
                        ? pack.paths
                            .ToDictionary(item => item.TextureCompressionFormat, item => item.path)
                        : new Dictionary<TextureCompressionFormat, String>();
                if (compressionFormatToAssetPackDirectoryPath.Count != 0)
                {
                    assetPackConfig.AddAssetsFolders(
                        pack.name, compressionFormatToAssetPackDirectoryPath, pack.DeliveryMode);
                }

                var deviceTierToAssetPackDirectoryPath =
                    pack.paths.Any(path => path.DeviceTier != null)
                        ? pack.paths
                            .ToDictionary(item => item.DeviceTier, item => item.path)
                        : new Dictionary<DeviceTier, string>();
                if (deviceTierToAssetPackDirectoryPath.Count != 0)
                {
                    assetPackConfig.AddAssetsFolders(pack.name, deviceTierToAssetPackDirectoryPath,
                        pack.DeliveryMode);
                }

            }

            return assetPackConfig;
        }

        private static Dictionary<TKey, TValue> CopyDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict == null)
            {
                return null;
            }

            return new Dictionary<TKey, TValue>(dict);
        }
    }
}