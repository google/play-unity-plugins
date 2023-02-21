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

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Tools for adding device tier targeting to folders of an Android App Bundle.
    /// </summary>
    public static class DeviceTierTargetingTools
    {
        /// <summary>
        /// The key used by bundletool to recognize a folder targeted for a specific device tier,
        /// for example "tier" in "textures#tier_high".
        /// </summary>
        private const string DeviceTierTargetingKey = "tier";

        /// <summary>
        /// Returns the string used to designate a device tier in bundletool.
        /// </summary>
        public static string GetBundleToolDeviceTierFormatName(DeviceTier tier)
        {
            return tier.ToString();
        }

        /// <summary>
        /// Get the targeting suffix for a given device tier, to
        /// be appended to a folder name containing containing AssetBundles or raw files to be targeted.
        /// </summary>
        public static string GetTargetingSuffix(DeviceTier tier)
        {
            return string.Format("#{0}_{1}", DeviceTierTargetingKey,
                GetBundleToolDeviceTierFormatName(tier));
        }
    }
}
