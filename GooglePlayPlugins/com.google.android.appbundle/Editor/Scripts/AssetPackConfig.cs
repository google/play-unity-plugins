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
using System.IO;
using System.Linq;

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Configuration for all asset packs that will be packaged in an Android App Bundle.
    /// </summary>
    public class AssetPackConfig
    {
        /// <summary>
        /// Dictionary from asset pack name to <see cref="AssetPack"/> object containing info such as delivery mode.
        /// Note: asset pack names must start with a letter and can contain only letters, numbers, and underscores.
        /// Note: convenience methods such as <see cref="AddAssetBundle"/> are recommended to more safely add new
        /// Asset packs, but this dictionary is available for direct modification for advanced use cases.
        /// </summary>
        public readonly Dictionary<string, AssetPack> AssetPacks = new Dictionary<string, AssetPack>();

        /// <summary>
        /// A dictionary containing the subset of <see cref="AssetPacks"/> that are marked for delivery.
        /// </summary>
        public Dictionary<string, AssetPack> DeliveredAssetPacks
        {
            get
            {
                return AssetPacks
                    .Where(pair => pair.Value.DeliveryMode != AssetPackDeliveryMode.DoNotPackage)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }
        }

        /// <summary>
        /// When asset packs for multiple texture compression formats are present, this specifies the format used
        /// when building standalone APKs for Android pre-Lollipop devices.
        /// </summary>
        public TextureCompressionFormat DefaultTextureCompressionFormat = TextureCompressionFormat.Default;

        /// <summary>
        /// Package the specified AssetBundle file in its own <see cref="AssetPack"/> with the specified delivery mode.
        /// The name of the created asset pack will match that of the specified AssetBundle file.
        /// </summary>
        /// <param name="assetBundleFilePath">The path to a single AssetBundle file.</param>
        /// <param name="deliveryMode">The <see cref="AssetPackDeliveryMode"/> for the asset pack.</param>
        /// <exception cref="ArgumentException">If the asset pack name is invalid.</exception>
        /// <exception cref="FileNotFoundException">If the AssetBundle file doesn't exist.</exception>
        public void AddAssetBundle(string assetBundleFilePath, AssetPackDeliveryMode deliveryMode)
        {
            var assetPackName = GetAssetPackName(assetBundleFilePath);
            AssetPacks[assetPackName] = new AssetPack
            {
                DeliveryMode = deliveryMode,
                AssetBundleFilePath = assetBundleFilePath
            };
        }

        /// <summary>
        /// Package all raw assets in the specified folder in an <see cref="AssetPack"/> with the specified name and
        /// using the specified delivery mode.
        /// </summary>
        /// <param name="assetPackName">The name of the asset pack.</param>
        /// <param name="assetsFolderPath">
        /// The path to a directory whose files will be directly copied into the asset pack during app bundle creation.
        /// </param>
        /// <param name="deliveryMode">The <see cref="AssetPackDeliveryMode"/> for the asset pack.</param>
        /// <exception cref="ArgumentException">If the <see cref="assetPackName"/> is invalid.</exception>
        /// <exception cref="FileNotFoundException">If the <see cref="assetsFolderPath"/> doesn't exist.</exception>
        public void AddAssetsFolder(string assetPackName, string assetsFolderPath, AssetPackDeliveryMode deliveryMode)
        {
            var directoryInfo = new DirectoryInfo(assetsFolderPath);
            if (!directoryInfo.Exists)
            {
                throw new FileNotFoundException("Asset pack directory doesn't exist", assetsFolderPath);
            }

            CheckAssetPackName(assetPackName);
            AssetPacks[assetPackName] = new AssetPack
            {
                DeliveryMode = deliveryMode,
                AssetPackDirectoryPath = assetsFolderPath
            };
        }

        /// <summary>
        /// Package the specified raw assets in the specified folders, keyed by <see cref="TextureCompressionFormat"/>,
        /// in an <see cref="AssetPack"/> with the specified delivery mode.
        /// When using Play Asset Delivery APIs, only the folder for the device's preferred texture compression format
        /// will be delivered.
        /// </summary>
        /// <param name="assetPackName">The name of the asset pack.</param>
        /// <param name="compressionFormatToAssetPackDirectoryPath">
        /// A dictionary from <see cref="TextureCompressionFormat"/> to the path of directories of files that will be
        /// directly copied into the asset pack during app bundle creation.</param>
        /// <param name="deliveryMode">The <see cref="AssetPackDeliveryMode"/> for the asset pack.</param>
        /// <exception cref="ArgumentException">If the dictionary or asset pack name is invalid.</exception>
        public void AddAssetsFolders(
            string assetPackName,
            IDictionary<TextureCompressionFormat, string> compressionFormatToAssetPackDirectoryPath,
            AssetPackDeliveryMode deliveryMode)
        {
            if (compressionFormatToAssetPackDirectoryPath.Count == 0)
            {
                throw new ArgumentException("Dictionary should contain at least one path");
            }

            if (compressionFormatToAssetPackDirectoryPath.All(kvp => kvp.Key != TextureCompressionFormat.Default))
            {
                throw new ArgumentException("Dictionary should contain at least one Default compression path");
            }

            CheckAssetPackName(assetPackName);
            AssetPacks[assetPackName] = new AssetPack
            {
                DeliveryMode = deliveryMode,
                CompressionFormatToAssetPackDirectoryPath =
                    new Dictionary<TextureCompressionFormat, string>(compressionFormatToAssetPackDirectoryPath)
            };
        }

        /// <summary>
        /// Package the specified AssetBundle files, which vary only by <see cref="TextureCompressionFormat"/>, in an
        /// <see cref="AssetPack"/> with the specified delivery mode.
        /// When using Play Asset Delivery APIs, only the AssetBundle for the device's preferred texture compression
        /// format will be delivered.
        /// </summary>
        /// <param name="compressionFormatToAssetBundleFilePath">
        /// A dictionary from <see cref="TextureCompressionFormat"/> to AssetBundle files.</param>
        /// <param name="deliveryMode">The <see cref="AssetPackDeliveryMode"/> for the asset pack.</param>
        /// <exception cref="ArgumentException">If the dictionary or asset pack name is invalid.</exception>
        /// <exception cref="FileNotFoundException">If any AssetBundle file doesn't exist.</exception>
        public void AddAssetBundles(
            IDictionary<TextureCompressionFormat, string> compressionFormatToAssetBundleFilePath,
            AssetPackDeliveryMode deliveryMode)
        {
            if (compressionFormatToAssetBundleFilePath.Count == 0)
            {
                throw new ArgumentException("Dictionary should contain at least one AssetBundle");
            }

            if (compressionFormatToAssetBundleFilePath.All(kvp => kvp.Key != TextureCompressionFormat.Default))
            {
                throw new ArgumentException("Dictionary should contain at least one Default compression AssetBundle");
            }

            var assetPackName = GetAssetPackName(compressionFormatToAssetBundleFilePath.Values.First());
            if (compressionFormatToAssetBundleFilePath.Any(kvp => assetPackName != GetAssetPackName(kvp.Value)))
            {
                throw new ArgumentException("All AssetBundles in the Dictionary must have the same name");
            }

            AssetPacks[assetPackName] = new AssetPack
            {
                DeliveryMode = deliveryMode,
                CompressionFormatToAssetBundleFilePath =
                    new Dictionary<TextureCompressionFormat, string>(compressionFormatToAssetBundleFilePath)
            };
        }

        private static string GetAssetPackName(string assetBundleFilePath)
        {
            var fileInfo = new FileInfo(assetBundleFilePath);
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException("AssetBundle file doesn't exist", assetBundleFilePath);
            }

            var assetPackName = fileInfo.Name;
            CheckAssetPackName(assetPackName);
            return assetPackName;
        }

        private static void CheckAssetPackName(string assetPackName)
        {
            if (!AndroidAppBundle.IsValidModuleName(assetPackName))
            {
                throw new ArgumentException("Invalid asset pack name: " + assetPackName);
            }
        }
    }
}
