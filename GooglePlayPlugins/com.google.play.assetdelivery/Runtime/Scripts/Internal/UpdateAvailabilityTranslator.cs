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

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Translates PlayCore's AssetPackUpdateAvailability Java enum into its corresponding public-facing
    /// Unity enum.
    /// </summary>
    internal class UpdateAvailabilityTranslator
    {
        private static class PlayCoreAssetPackUpdateAvailability
        {
            public const int Unknown = 0;
            public const int UpdateNotAvailable = 1;
            public const int UpdateAvailable = 2;
        }

        private static readonly Dictionary<int, AssetPackUpdateAvailability> PlayCoreToAssetPackUpdateAvailability
            =
            new Dictionary<int, AssetPackUpdateAvailability>
            {
                {PlayCoreAssetPackUpdateAvailability.Unknown, AssetPackUpdateAvailability.Unknown},
                {
                    PlayCoreAssetPackUpdateAvailability.UpdateNotAvailable,
                    AssetPackUpdateAvailability.UpdateNotAvailable
                },
                {PlayCoreAssetPackUpdateAvailability.UpdateAvailable, AssetPackUpdateAvailability.UpdateAvailable},
            };

        /// <summary>
        /// Translates Play Core's AssetPackUpdateAvailability into its corresponding public-facing AssetPackUpdateAvailability.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided update availability does not have a corresponding value in AssetPackUpdateAvailability.
        /// </exception>
        public static AssetPackUpdateAvailability TranslatePlayCoreUpdateAvailability(
            int assetPackUpdateAvailability)
        {
            AssetPackUpdateAvailability translatedAvailability;
            if (!PlayCoreToAssetPackUpdateAvailability.TryGetValue(assetPackUpdateAvailability,
                out translatedAvailability))
            {
                throw new NotImplementedException("Unexpected pack update availability: " +
                                                  assetPackUpdateAvailability);
            }

            return translatedAvailability;
        }
    }
}