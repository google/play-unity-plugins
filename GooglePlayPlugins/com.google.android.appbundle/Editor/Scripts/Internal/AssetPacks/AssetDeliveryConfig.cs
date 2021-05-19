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
using System.Linq;
using Google.Android.AppBundle.Editor.Internal.Config;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Internal-only representation of asset packs being considered for packaging in an Android App Bundle.
    /// </summary>
    public class AssetDeliveryConfig
    {
        /// <summary>
        /// Default texture compression format for building standalone APKs for Android pre-Lollipop devices.
        /// </summary>
        public TextureCompressionFormat DefaultTextureCompressionFormat = TextureCompressionFormat.Default;

        /// <summary>
        /// Whether to split the assets in an AAB's base module into a separate install-time asset pack.
        /// </summary>
        public bool SplitBaseModuleAssets;

        /// <summary>
        /// Dictionary from a folder path to a AssetBundleFolder object describing the folder's contents.
        /// </summary>
        public readonly IDictionary<string, AssetBundleFolder> Folders =
            new SortedDictionary<string, AssetBundleFolder>();

        /// <summary>
        /// Dictionary from AssetBundle name to AssetBundlePacks.
        /// </summary>
        public IDictionary<string, AssetBundlePack> AssetBundlePacks { get; private set; }

        // TODO: add support for managing this type of asset pack in the UI.
        public List<SerializableAssetPack> rawAssetsPacks;

        private readonly IEnumerable<IAssetPackValidator> _packValidators;

        public AssetDeliveryConfig()
        {
            AssetBundlePacks = new Dictionary<string, AssetBundlePack>();
            _packValidators = AssetPackValidatorRegistry.Registry.ConstructInstances();
        }

        /// <summary>
        /// Updates the in-memory state based on the AssetBundles on disk.
        /// </summary>
        public void Refresh()
        {
            // Refresh the AssetBundlePacks according to AssetBundles contained in folders.
            RefreshAssetBundlePacks();

            // Check for errors.
            foreach (var assetBundlePack in AssetBundlePacks.Values)
            {
                CheckBuildTypeErrors(assetBundlePack);
                assetBundlePack.CheckDependencyErrors(
                    (string name, out AssetBundlePack pack) => AssetBundlePacks.TryGetValue(name, out pack));
            }
        }

        /// <summary>
        /// Potentially refresh all folders and/or add and remove the specified folders.
        /// </summary>
        /// <returns>true if changes were made.</returns>
        public bool UpdateAndRefreshFolders(bool refreshFolders, List<string> foldersToAdd,
            List<string> foldersToRemove)
        {
            if (!refreshFolders && foldersToRemove.Count == 0 && foldersToAdd.Count == 0)
            {
                return false;
            }

            foreach (var folder in foldersToAdd)
            {
                if (Folders.ContainsKey(folder))
                {
                    Debug.LogWarningFormat("Skipping folder \"{0}\" because it is already in the list.", folder);
                }
                else
                {
                    Folders.Add(folder, new AssetBundleFolder(folder));
                }
            }

            foreach (var folder in foldersToRemove)
            {
                Folders.Remove(folder);
            }

            Refresh();
            return true;
        }

        /// <summary>
        /// Return a list of error messages that are applicable to AssetBundles marked for packaging/delivery
        /// along with the number of AssetBundles that are marked for packaging/delivery.
        /// </summary>
        public List<string> GetPackagingErrorMessages(out int numAssetPacksToDeliver)
        {
            // Check for errors in asset packs
            numAssetPacksToDeliver = 0;
            var errors = new List<string>();

            foreach (var assetBundlePack in AssetBundlePacks.Values)
            {
                if (assetBundlePack.DeliveryMode == AssetPackDeliveryMode.DoNotPackage)
                {
                    continue;
                }

                var errorSummary = assetBundlePack.ErrorSummary;
                if (errorSummary != null)
                {
                    errors.Add(string.Format(
                        "Unable to package AssetBundle pack \"{0}\" because {1}.",
                        assetBundlePack.Name, errorSummary.ToLower()));
                }

                numAssetPacksToDeliver++;
                foreach (var variant in assetBundlePack.Variants.Values)
                {
                    var variantErrorSummary = variant.ErrorSummary;
                    if (variantErrorSummary != null)
                    {
                        errors.Add(string.Format(
                            "Unable to package AssetBundle \"{0}\" because {1}.",
                            variant.Path, variantErrorSummary.ToLower()));
                    }
                }
            }

            // Check for errors in the rest of the configuration.
            if (HasTextureCompressionFormatTargeting() &&
                !GetAllTextureCompressionFormats().Contains(DefaultTextureCompressionFormat))
            {
                errors.Add(string.Format(
                    "The default texture compression format ({0}) is not used by any AssetBundles.",
                    DefaultTextureCompressionFormat.ToString()));
            }

            // TODO: think about moving to a build check.
            if (HasTextureCompressionFormatTargeting() &&
                EditorUserBuildSettings.androidBuildSubtarget != MobileTextureSubtarget.Generic)
            {
                errors.Add(string.Format(
                    "Texture Compression in the Build Settings for Android is set to \"{0}\". This is not compatible with asset packs having targeted textures. Set the texture compression in the Build Settings for Android to \"Don't override'\" before continuing.",
                    EditorUserBuildSettings.androidBuildSubtarget));
            }

            return errors;
        }

        /// <summary>
        /// Refresh the list of all the <see cref="AssetBundlePacks"/> represented by the folders.
        /// </summary>
        public void RefreshAssetBundlePacks()
        {
            var assetBundlePacks = new Dictionary<string, AssetBundlePack>();
            foreach (var folder in Folders.Values)
            {
                var assetPacks = folder.ExtractAssetBundleVariant();

                foreach (var targetedAssetPackPair in assetPacks)
                {
                    var targetedAssetPackName = targetedAssetPackPair.Key;
                    var targetedAssetPack = targetedAssetPackPair.Value;

                    AssetBundlePack assetBundlePack;
                    if (!assetBundlePacks.TryGetValue(targetedAssetPackName, out assetBundlePack))
                    {
                        var deliveryMode = GetMultiTargetingAssetPackDeliveryMode(targetedAssetPackName);
                        assetBundlePack = new AssetBundlePack(targetedAssetPackName, deliveryMode);
                        assetBundlePacks.Add(targetedAssetPackName, assetBundlePack);
                    }

                    if (!assetBundlePack.Add(folder.TextureCompressionFormat, targetedAssetPack))
                    {
                        assetBundlePack.Errors.Add(AssetPackError.DuplicateName);
                    }
                }
            }

            AssetBundlePacks = assetBundlePacks;
        }

        /// <summary>
        /// Returns the delivery mode for the AssetBundlePack with the given name, or DoNotPackage if not found.
        /// </summary>
        private AssetPackDeliveryMode GetMultiTargetingAssetPackDeliveryMode(string multiTargetingAssetPackName)
        {
            AssetBundlePack assetBundlePack;
            if (AssetBundlePacks.TryGetValue(multiTargetingAssetPackName, out assetBundlePack))
            {
                return assetBundlePack.DeliveryMode;
            }

            return AssetPackDeliveryMode.DoNotPackage;
        }

        /// <summary>
        /// Returns all texture compression formats used by asset packs.
        /// </summary>
        public IEnumerable<TextureCompressionFormat> GetAllTextureCompressionFormats()
        {
            return AssetBundlePacks.Values.SelectMany(
                multiTargetingAssetPack => multiTargetingAssetPack.Variants.Keys).Distinct();
        }

        /// <summary>
        /// Check if some asset packs are using targeted texture compression format.
        /// </summary>
        public bool HasTextureCompressionFormatTargeting()
        {
            return GetAllTextureCompressionFormats().Any(textureCompressionFormat =>
                textureCompressionFormat != TextureCompressionFormat.Default);
        }

        /// <summary>
        /// Checks the specified bundlePack for build errors using validators registered in AssetPackValidatorRegistry.
        /// </summary>
        private void CheckBuildTypeErrors(AssetBundlePack bundlePack)
        {
            var buildErrors = new HashSet<AssetPackError>();
            foreach (var buildValidator in _packValidators)
            {
                buildErrors.UnionWith(buildValidator.CheckBuildErrors(bundlePack));
            }

            foreach (var error in buildErrors)
            {
                bundlePack.AddErrorToVariants(error);
            }
        }
    }
}