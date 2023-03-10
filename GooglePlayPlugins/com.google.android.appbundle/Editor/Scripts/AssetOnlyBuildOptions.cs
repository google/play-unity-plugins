// Copyright 2021 Google LLC
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
    /// Options that configure the behavior of an asset-only app bundle build.
    /// </summary>
    public class AssetOnlyBuildOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="assetPackConfig">AssetPackConfig specifying which asset packs to include in the build.</param>>
        /// <exception cref="ArgumentException">Thrown if the AssetPackConfig contains install-time packs.</exception>
        public AssetOnlyBuildOptions(AssetPackConfig assetPackConfig)
        {
            foreach (var keyValue in assetPackConfig.AssetPacks)
            {
                var deliveryMode = keyValue.Value.DeliveryMode;
                if (deliveryMode == AssetPackDeliveryMode.InstallTime)
                {
                    throw new ArgumentException(
                        "AssetPackConfig contains install-time asset packs which are not supported in asset-only app bundles.");
                }
            }

            AssetPackConfig = assetPackConfig;
        }

        /// <summary>
        /// Returns the AssetPackConfig to use for the build, or null.
        /// </summary>
        public AssetPackConfig AssetPackConfig { get; private set; }

        /// <summary>
        /// The version codes identifying which app versions should be served these asset packs.
        /// </summary>
        public IList<long> AppVersions { get; set; }

        /// <summary>
        /// A string that uniquely identifies this asset-only bundle.
        /// </summary>
        public string AssetVersionTag { get; set; }

        /// <summary>
        /// The path where the asset only bundle will be built. 
        /// </summary>
        public string LocationPathName { get; set; }

        /// <summary>
        /// If true, forces the entire build to run on the main thread, potentially freezing the Editor UI during some
        /// build steps. This setting doesn't affect batch mode builds, which always run on the main thread.
        /// </summary>
        public bool ForceSingleThreadedBuild { get; set; }
    }
}