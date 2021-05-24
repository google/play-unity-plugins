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
using System.Text;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Wraps Play Core's AssetPackStates which represents the current state of all requested asset packs.
    /// </summary>
    internal class AssetPackStates
    {
        /// <summary>
        /// The total size of all requested packs in bytes.
        /// </summary>
        public long TotalBytes { get; private set; }

        /// <summary>
        /// A dictionary containing the download states of all requested asset packs.
        /// </summary>
        public Dictionary<string, AssetPackState> PackStates { get; private set; }

        /// <summary>
        /// Creates an AssetPackStates with all the fields of the underlying Java object.
        /// </summary>
        public AssetPackStates(AndroidJavaObject packStates)
        {
            using (packStates)
            {
                TotalBytes = packStates.Call<long>("totalBytes");
                PackStates = ConvertPackStatesJavaMapToDictionary(packStates);
            }
        }

        private Dictionary<string, AssetPackState> ConvertPackStatesJavaMapToDictionary(
            AndroidJavaObject packStates)
        {
            using (var javaMap = packStates.Call<AndroidJavaObject>("packStates"))
            {
                return PlayCoreHelper.ConvertJavaMap<string, AndroidJavaObject>(javaMap)
                    .ToDictionary(kvp => kvp.Key, kvp => new AssetPackState(kvp.Value));
            }
        }

        public override string ToString()
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendFormat("total bytes: {0}\n", TotalBytes);
            stateDescription.AppendLine("pack names: " + string.Join(", ", PackStates.Keys.ToArray()));

            return stateDescription.ToString();
        }
    }
}