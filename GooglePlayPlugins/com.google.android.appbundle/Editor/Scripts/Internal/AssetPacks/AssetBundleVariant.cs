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

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Internal-only wrapper around an AssetBundle that by virtue of its texture compression format is a
    /// variant of the other AssetBundles contained in an <see cref="AssetBundlePack"/>.
    /// </summary>
    public class AssetBundleVariant
    {
        /// <summary>
        /// Special value for <see cref="FileSizeBytes"/> indicating the AssetBundle file does not exist.
        /// </summary>
        public const long FileSizeIfMissing = -1L;

        // Visible for testing.
        public const string FileMissingText = "file missing";

        // Visible for testing.
        public const string NoneText = "none";

        private static readonly string[] EmptyStringArray = new string[0];

        /// <summary>
        /// Constructor. Prefer <see cref="CreateVariant"/> for all cases except deserialization and testing.
        /// </summary>
        public AssetBundleVariant()
        {
            DirectDependencies = EmptyStringArray;
            AllDependencies = EmptyStringArray;
        }

        /// <summary>
        /// If non-empty, these errors will prevent this AssetBundle from being packaged in an app bundle.
        /// </summary>
        public readonly HashSet<AssetPackError> Errors = new HashSet<AssetPackError>();

        /// <summary>
        /// The names of any AssetBundles that this one directly depends on.
        /// </summary>
        public string[] DirectDependencies { get; private set; }

        /// <summary>
        /// The names of any AssetBundles that this one directly or transitively depends on.
        /// </summary>
        public string[] AllDependencies { get; private set; }

        /// <summary>
        /// The size of the file in bytes, or -1L if the file doesn't exist.
        /// </summary>
        public long FileSizeBytes { get; private set; }

        /// <summary>
        /// The path to the AssetBundle represented by this wrapper.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// A UI friendly way of displaying dependency info.
        /// </summary>
        public string DependenciesText
        {
            // TODO: consider how to handle an extra long string in the UI.
            get { return DirectDependencies.Length == 0 ? NoneText : string.Join(", ", DirectDependencies); }
        }

        /// <summary>
        /// A UI friendly way of displaying file size or that the file doesn't exist on disk.
        /// </summary>
        public string FileSizeText
        {
            get
            {
                // Display file size in Kibibytes using "JEDEC" standard. See https://en.wikipedia.org/wiki/Kilobyte
                return FileSizeBytes == FileSizeIfMissing ? FileMissingText : (FileSizeBytes / 1024) + " KB";
            }
        }

        /// <summary>
        /// A UI friendly way of displaying the error with this AssetBundle (if there is one) or the number of errors
        /// (if there is more than one). If there are no errors, this is null.
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
        /// Delegate to find a variant when doing the dependencies check.
        /// See <see cref="AssetBundleVariant.CheckDependencyErrors"/>.
        /// </summary>
        /// <param name="name">The asset pack to search.</param>
        /// <param name="dependencyVariant">If found, will contain the searched asset pack.</param>
        /// <param name="dependencyDeliveryMode">If found, will contain the delivery mode of the searched asset pack.</param>
        /// <returns>true if the asset pack was found, false otherwise.</returns>
        public delegate bool TryGetDependency(string name, out AssetBundleVariant dependencyVariant,
            out AssetPackDeliveryMode dependencyDeliveryMode);

        /// <summary>
        /// Evaluates whether to set any <see cref="Errors"/> based on problems with this variant's dependencies.
        /// </summary>
        /// <param name="deliveryMode">The delivery mode of this asset pack.</param>
        /// <param name="tryGetDependency">A function returning the asset pack for the given dependency name,
        /// along with the delivery mode of the asset pack.</param>
        public void CheckDependencyErrors(AssetPackDeliveryMode deliveryMode, TryGetDependency tryGetDependency)
        {
            if (deliveryMode == AssetPackDeliveryMode.DoNotPackage)
            {
                return;
            }

            foreach (var dependencyName in AllDependencies)
            {
                AssetBundleVariant dependencyVariant;
                AssetPackDeliveryMode dependencyDeliveryMode;
                if (!tryGetDependency(dependencyName, out dependencyVariant, out dependencyDeliveryMode))
                {
                    Errors.Add(AssetPackError.DependencyMissing);
                    continue;
                }

                if (dependencyVariant.Errors.Count > 0)
                {
                    Errors.Add(AssetPackError.DependencyError);
                }

                // If this asset pack is marked for delivery, all of its dependencies must be packaged for delivery.
                if (dependencyDeliveryMode == AssetPackDeliveryMode.DoNotPackage)
                {
                    Errors.Add(AssetPackError.DependencyNotPackaged);
                    continue;
                }

                // An asset pack cannot have dependencies on asset packs with later delivery modes,
                // e.g. A FastFollow asset pack cannot depend on an OnDemand asset pack.
                if (deliveryMode < dependencyDeliveryMode)
                {
                    Errors.Add(AssetPackError.DependencyIncompatibleDelivery);
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="AssetBundleVariant"/>. Evaluates some conditions and may set <see cref="Errors"/>.
        /// </summary>
        /// <param name="assetBundleName">Name of the AssetBundle represented by this variant.</param>
        /// <param name="fileSizeBytes">Size of the file in bytes, or <see cref="FileSizeIfMissing"/>.</param>
        /// <param name="directDependencies">AssetBundles that are direct dependencies of this one.</param>
        /// <param name="allDependencies">AssetBundles that are direct or transitive dependencies of this one.</param>
        /// <param name="path">The path to the AssetBundle represented by this variant.</param>
        public static AssetBundleVariant CreateVariant(
            string assetBundleName,
            long fileSizeBytes,
            string[] directDependencies,
            string[] allDependencies,
            string path)
        {
            var variant = new AssetBundleVariant
            {
                DirectDependencies = directDependencies ?? EmptyStringArray,
                AllDependencies = allDependencies ?? EmptyStringArray,
                FileSizeBytes = fileSizeBytes,
                Path = path,
            };

            if (fileSizeBytes == FileSizeIfMissing)
            {
                variant.Errors.Add(AssetPackError.FileMissing);
            }

            if (!AndroidAppBundle.IsValidModuleName(assetBundleName))
            {
                variant.Errors.Add(AssetPackError.InvalidName);
            }

            return variant;
        }
    }
}