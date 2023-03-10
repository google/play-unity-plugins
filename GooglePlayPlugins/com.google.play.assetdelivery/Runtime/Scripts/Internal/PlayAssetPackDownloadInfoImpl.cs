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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Wraps the subset of Play Core's AssetPackState that pertains to asset-only updates.
    /// It contains information regarding newer versions of a particular asset pack.
    /// </summary>
    public class PlayAssetPackDownloadInfoImpl : PlayAssetPackDownloadInfo
    {
        /// <summary>
        /// Creates a dictionary with download info, keyed by asset pack name, from the asset packs contained within a PlayCore
        /// AssetPackStates object.
        /// </summary>
        /// <param name="javaAssetPackStates">
        /// The PlayCore AssetPacksStates object containing the desired asset pack info.
        /// </param>
        /// <returns>
        /// A dictionary, keyed by asset pack name, containing download info about the packs within the specified
        /// AssetPackStates.
        /// </returns>
        public static IDictionary<string, PlayAssetPackDownloadInfo> FromAssetPackStates(
            AndroidJavaObject javaAssetPackStates)
        {
            using (var javaMap = javaAssetPackStates.Call<AndroidJavaObject>("packStates"))
            {
                return PlayCoreHelper.ConvertJavaMap<string, AndroidJavaObject>(javaMap)
                    .ToDictionary<KeyValuePair<string, AndroidJavaObject>, string, PlayAssetPackDownloadInfo>(
                        kvp => kvp.Key,
                        kvp => new PlayAssetPackDownloadInfoImpl(kvp.Value));
            }
        }

        /// <summary>
        /// Creates a PlayAssetPackDownloadInfoImpl from the underlying Java AssetPackState.
        /// Disposes the Java object in the process.
        /// </summary>
        /// <param name="javaAssetPackState">
        /// The PlayCore AssetPackState Java object from which to create the PlayAssetPackDownloadInfoImpl.
        /// </param>
        public PlayAssetPackDownloadInfoImpl(AndroidJavaObject javaAssetPackState)
        {
            var javaAvailability = javaAssetPackState.Call<int>("updateAvailability");
            UpdateAvailability = UpdateAvailabilityTranslator.TranslatePlayCoreUpdateAvailability(javaAvailability);

            DownloadSize = javaAssetPackState.Call<long>("totalBytesToDownload");
            AvailableVersionTag = javaAssetPackState.Call<string>("availableVersionTag");
            InstalledVersionTag = javaAssetPackState.Call<string>("installedVersionTag");

            javaAssetPackState.Dispose();
        }

        public override string ToString()
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendFormat("update availability: {0}\n", UpdateAvailability);
            stateDescription.AppendFormat("available version tag: {0}\n", AvailableVersionTag);
            stateDescription.AppendFormat("installed version tag: {0}\n", InstalledVersionTag);

            return stateDescription.ToString();
        }
    }
}