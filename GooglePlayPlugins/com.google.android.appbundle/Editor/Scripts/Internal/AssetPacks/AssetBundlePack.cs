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
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Internal-only wrapper around <see cref="AssetBundleVariant"/>s that are identical except for their
    /// texture compression formats.
    /// </summary>
    public class AssetBundlePack
    {
        /// <summary>
        /// The name of the asset pack, which is the same as the name of every included AssetBundle.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Indicates whether this pack will be included in an Android App Bundle, and if so how it is delivered.
        /// </summary>
        public AssetPackDeliveryMode DeliveryMode;

        /// <summary>
        /// The AssetBundles, all with the same name but generated for different texture compression formats.
        /// </summary>
        public IDictionary<TextureCompressionFormat, AssetBundleVariant> Variants { get; private set; }

        /// <summary>
        /// If non-empty, these errors will prevent this AssetBundlePack from being packaged in an App Bundle.
        /// </summary>
        public readonly HashSet<AssetPackError> Errors = new HashSet<AssetPackError>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public AssetBundlePack(string name, AssetPackDeliveryMode deliveryMode)
        {
            Name = name;
            DeliveryMode = deliveryMode;
            Variants = new Dictionary<TextureCompressionFormat, AssetBundleVariant>();
        }

        /// <summary>
        /// Adds an <see cref="AssetBundleVariant"/> to this asset pack.
        /// <returns>true if the AssetBundle was added, false if another AssetBundle with the same compression
        /// is already present.</returns>
        /// </summary>
        public bool Add(TextureCompressionFormat textureCompressionFormat, AssetBundleVariant variant)
        {
            AssetBundleVariant existingVariant;
            if (Variants.TryGetValue(textureCompressionFormat, out existingVariant))
            {
                Debug.LogErrorFormat("Multiple AssetBundles for texture format {0} are used for {1}.",
                    textureCompressionFormat.ToString(), Name);
                existingVariant.Errors.Add(AssetPackError.DuplicateName);
                return false;
            }

            Variants.Add(textureCompressionFormat, variant);
            return true;
        }

        /// <summary>
        /// Adds the specified error to all children.
        /// </summary>
        public void AddErrorToVariants(AssetPackError error)
        {
            foreach (var assetPack in Variants.Values)
            {
                assetPack.Errors.Add(error);
            }
        }

        /// <summary>
        /// The names of any AssetBundles that the AssetBundles inside this AssetBundlePack directly depend on.
        /// </summary>
        public IList<string> DirectDependencies
        {
            get
            {
                var directDependencies =
                    Variants.Values.SelectMany(pack => pack.DirectDependencies).Distinct().ToList();
                directDependencies.Sort();
                return directDependencies;
            }
        }

        /// <summary>
        /// A UI friendly way of displaying the error with this AssetBundlePack (if one) or the number of errors
        /// (if more than one). If there are no errors, returns null.
        /// </summary>
        public string ErrorSummary
        {
            get
            {
                switch (Errors.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return NameAndDescriptionAttribute.GetAttribute(Errors.First()).Name;
                    default:
                        return Errors.Count + " Errors";
                }
            }
        }

        /// <summary>
        /// Delegate to find an AssetBundlePack when doing the dependencies check.
        /// </summary>
        /// <param name="name">The name of the other AssetBundlePack.</param>
        /// <param name="assetBundlePack">If found, will contain the AssetBundlePack.</param>
        public delegate bool TryGetAssetBundlePack(string name, out AssetBundlePack assetBundlePack);

        /// <summary>
        /// Evaluates whether to set any <see cref="Errors"/> based on problems with the dependencies.
        /// </summary>
        /// <param name="tryGetAssetBundlePack">Function to get an <see cref="AssetBundlePack"/> given its name.</param>
        public void CheckDependencyErrors(TryGetAssetBundlePack tryGetAssetBundlePack)
        {
            if (DeliveryMode == AssetPackDeliveryMode.DoNotPackage)
            {
                return;
            }

            foreach (var variant in Variants)
            {
                var textureCompressionFormat = variant.Key;
                var assetPack = variant.Value;
                assetPack.CheckDependencyErrors(
                    DeliveryMode,
                    TryGetDependency(tryGetAssetBundlePack, textureCompressionFormat));
            }
        }

        private static AssetBundleVariant.TryGetDependency TryGetDependency(
            TryGetAssetBundlePack tryGetAssetBundlePack, TextureCompressionFormat textureCompressionFormat)
        {
            return (string name, out AssetBundleVariant dependency, out AssetPackDeliveryMode deliveryMode) =>
            {
                deliveryMode = AssetPackDeliveryMode.DoNotPackage;
                dependency = null;
                AssetBundlePack assetBundlePack;
                if (tryGetAssetBundlePack(name, out assetBundlePack) &&
                    assetBundlePack.Variants.TryGetValue(textureCompressionFormat, out dependency))
                {
                    deliveryMode = assetBundlePack.DeliveryMode;
                    return true;
                }

                return false;
            };
        }
    }
}