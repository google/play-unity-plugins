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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Internal-only representation of a folder containing AssetBundles.
    /// </summary>
    public class AssetBundleFolder
    {
        /// <summary>
        /// The error state of this folder or <see cref="AssetPackFolderState.Ok"/>.
        /// </summary>
        public AssetPackFolderState State { get; private set; }

        /// <summary>
        /// The path represented by the folder.
        /// </summary>
        public string FolderPath { get; private set; }

        /// <summary>
        /// The texture compression format of AssetBundles inside this folder, based on the folder name.
        /// </summary>
        public TextureCompressionFormat TextureCompressionFormat { get; private set; }

        /// <summary>
        /// The number of AssetBundles found in this folder.
        /// Note: this value is cached during calls to <see cref="ExtractAssetBundleVariant"/>.
        /// </summary>
        public int AssetBundleCount { get; private set; }

        private string _searchedManifestFileName;

        /// <summary>
        /// Create a new AssetBundleFolder for a given path (which is then immutable).
        /// Call <see cref="Refresh"/> to update the folder state and texture compression format targeting.
        /// </summary>
        public AssetBundleFolder(string folderPath)
        {
            AssetBundleCount = 0;
            FolderPath = folderPath;
        }

        /// <summary>
        /// Update the folder state and texture compression format targeting, if any.
        /// </summary>
        /// <returns>true if the refresh was properly done (no errors)</returns>
        public bool Refresh()
        {
            var directoryInfo = new DirectoryInfo(FolderPath);
            if (!directoryInfo.Exists)
            {
                AssetBundleCount = 0;
                State = AssetPackFolderState.FolderMissing;
                return false;
            }

            var files = directoryInfo.GetFiles();
            if (files.Length == 0)
            {
                AssetBundleCount = 0;
                State = AssetPackFolderState.FolderEmpty;
                return false;
            }

            TextureCompressionFormat textureCompressionFormat;
            TextureTargetingTools.GetTextureCompressionFormatAndStripSuffix(
                directoryInfo.Name,
                out textureCompressionFormat,
                out _searchedManifestFileName);
            TextureCompressionFormat = textureCompressionFormat;

            return true;
        }

        /// <summary>
        /// Scans and returns every <see cref="AssetBundleVariant"/> found in this folder.
        /// </summary>
        public IDictionary<string, AssetBundleVariant> ExtractAssetBundleVariant()
        {
            AssetBundleCount = 0;
            var assetBundleVariants = new Dictionary<string, AssetBundleVariant>();
            if (!Refresh())
            {
                return assetBundleVariants;
            }

            var directoryInfo = new DirectoryInfo(FolderPath);
            var files = directoryInfo.GetFiles();

            var fileDictionary = files.ToDictionary(file => file.Name, file => file);

            // Look for an AssetBundle file with the same name as the directory that contains it.
            // This AssetBundle file contains a single asset, which is of type AssetBundleManifest.
            // See https://unity3d.com/learn/tutorials/topics/best-practices/assetbundle-fundamentals
            FileInfo manifestFileInfo;
            if (!fileDictionary.TryGetValue(_searchedManifestFileName, out manifestFileInfo))
            {
                State = AssetPackFolderState.ManifestFileMissing;
                return assetBundleVariants;
            }

            var manifestFilePath = manifestFileInfo.FullName;
            AssetBundle manifestAssetBundle;
            try
            {
                manifestAssetBundle = AssetBundle.LoadFromFile(manifestFilePath);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Exception loading AssetBundle file containing manifest ({0}): {1}",
                    manifestFilePath, ex);
                State = AssetPackFolderState.ManifestFileLoadError;
                return assetBundleVariants;
            }

            try
            {
                var manifest = manifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                if (manifest == null)
                {
                    State = AssetPackFolderState.ManifestAssetMissing;
                    return assetBundleVariants;
                }

                var allAssetBundles = manifest.GetAllAssetBundles();
                if (allAssetBundles.Length == 0)
                {
                    State = AssetPackFolderState.AssetBundlesMissing;
                    return assetBundleVariants;
                }

                foreach (var assetBundleName in allAssetBundles)
                {
                    FileInfo assetBundleFileInfo;
                    var fileSizeBytes = fileDictionary.TryGetValue(assetBundleName, out assetBundleFileInfo)
                        ? assetBundleFileInfo.Length
                        : AssetBundleVariant.FileSizeIfMissing;

                    assetBundleVariants.Add(
                        assetBundleName,
                        AssetBundleVariant.CreateVariant(
                            assetBundleName,
                            fileSizeBytes,
                            manifest.GetDirectDependencies(assetBundleName),
                            manifest.GetAllDependencies(assetBundleName),
                            Path.Combine(FolderPath, assetBundleName)));
                }

                AssetBundleCount = assetBundleVariants.Count;
                return assetBundleVariants;
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat(
                    "Exception loading AssetBundleManifest from AssetBundle ({0}): {1}", manifestFilePath, ex);
                State = AssetPackFolderState.ManifestAssetLoadError;
                return assetBundleVariants;
            }
            finally
            {
                // If an AssetBundle isn't unloaded, the Editor will have to be restarted to load it again.
                manifestAssetBundle.Unload(true);
            }
        }
    }
}
