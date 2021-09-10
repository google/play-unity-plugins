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

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Contains configuration about an asset pack including its <see cref="AssetPackDeliveryMode"/>
    /// and the location on disk of the files that should be included in the pack.
    /// </summary>
    public class AssetPack
    {
        private string _assetBundleFilePath;
        private string _assetPackDirectoryPath;
        private Dictionary<TextureCompressionFormat, string> _compressionFormatToAssetBundleFilePath;
        private Dictionary<TextureCompressionFormat, string> _compressionFormatToAssetPackDirectoryPath;

        /// <summary>
        /// Indicates how this asset pack will be delivered.
        /// </summary>
        public AssetPackDeliveryMode DeliveryMode { get; set; }

        /// <summary>
        /// Location on disk of a single AssetBundle file. This file can be fetched at runtime using the Play Asset
        /// Delivery APIs.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown on set if any other path property is already set (non-null).
        /// </exception>
        public string AssetBundleFilePath
        {
            get { return _assetBundleFilePath; }
            set
            {
                if (value == null)
                {
                    _assetBundleFilePath = null;
                    return;
                }

                if (_assetPackDirectoryPath != null)
                {
                    throw new ArgumentException("AssetPackDirectoryPath is already set.");
                }

                if (_compressionFormatToAssetBundleFilePath != null)
                {
                    throw new ArgumentException("CompressionFormatToAssetBundleFilePath is already set.");
                }

                if (_compressionFormatToAssetPackDirectoryPath != null)
                {
                    throw new ArgumentException("CompressionFormatToAssetPackDirectoryPath is already set.");
                }

                _assetBundleFilePath = value;
            }
        }

        /// <summary>
        /// Location on disk of a folder containing raw asset files.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown on set if any other path property is already set (non-null).
        /// </exception>
        public string AssetPackDirectoryPath
        {
            get { return _assetPackDirectoryPath; }
            set
            {
                if (value == null)
                {
                    _assetPackDirectoryPath = null;
                    return;
                }

                if (_assetBundleFilePath != null)
                {
                    throw new ArgumentException("AssetBundleFilePath is already set.");
                }

                if (_compressionFormatToAssetBundleFilePath != null)
                {
                    throw new ArgumentException("CompressionFormatToAssetBundleFilePath is already set.");
                }

                if (_compressionFormatToAssetPackDirectoryPath != null)
                {
                    throw new ArgumentException("CompressionFormatToAssetPackDirectoryPath is already set.");
                }

                _assetPackDirectoryPath = value;
            }
        }

        /// <summary>
        /// Dictionary from <see cref="TextureCompressionFormat"/> to the location on disk of a single AssetBundle file.
        /// The AssetBundles should be identical except for their texture compression format. When using Play
        /// Asset Delivery APIs, only the AssetBundle for the device's preferred texture compression format will
        /// be delivered.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown on set if any other path property is already set (non-null).
        /// </exception>
        public Dictionary<TextureCompressionFormat, string> CompressionFormatToAssetBundleFilePath
        {
            get { return _compressionFormatToAssetBundleFilePath; }
            set
            {
                if (value == null)
                {
                    _compressionFormatToAssetBundleFilePath = null;
                    return;
                }

                if (_assetBundleFilePath != null)
                {
                    throw new ArgumentException("AssetBundleFilePath is already set.");
                }

                if (_assetPackDirectoryPath != null)
                {
                    throw new ArgumentException("AssetPackDirectoryPath is already set.");
                }

                if (_compressionFormatToAssetPackDirectoryPath != null)
                {
                    throw new ArgumentException("CompressionFormatToAssetPackDirectoryPath is already set.");
                }

                _compressionFormatToAssetBundleFilePath = value;
            }
        }

        /// <summary>
        /// Dictionary from <see cref="TextureCompressionFormat"/> to the location on disk of a folder containing
        /// raw asset files.
        /// When using Play Asset Delivery APIs, only the folder for the device's preferred texture compression format
        /// will be delivered.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown on set if any other path property is already set (non-null).
        /// </exception>
        public Dictionary<TextureCompressionFormat, string> CompressionFormatToAssetPackDirectoryPath
        {
            get { return _compressionFormatToAssetPackDirectoryPath; }
            set
            {
                if (value == null)
                {
                    _compressionFormatToAssetPackDirectoryPath = null;
                    return;
                }

                if (_assetBundleFilePath != null)
                {
                    throw new ArgumentException("AssetBundleFilePath is already set.");
                }

                if (_assetPackDirectoryPath != null)
                {
                    throw new ArgumentException("AssetPackDirectoryPath is already set.");
                }

                if (_compressionFormatToAssetBundleFilePath != null)
                {
                    throw new ArgumentException("CompressionFormatToAssetBundleFilePath is already set.");
                }

                _compressionFormatToAssetPackDirectoryPath = value;
            }
        }
    }
}