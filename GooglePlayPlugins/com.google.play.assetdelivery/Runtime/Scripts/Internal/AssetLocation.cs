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

using System.Text;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Wraps Play Core's AssetLocation which represents the location of an Asset within an asset pack on disk.
    /// </summary>
    internal class AssetLocation
    {
        /// <summary>
        /// If the AssetPackStorageMethod for the pack is <see cref="AssetPackStorageMethod#ApkAssets"/>, this
        /// will be the path to the APK containing the asset.
        ///
        /// If the AssetPackStorageMethod for the pack is <see cref="AssetPackStorageMethod#StorageFiles"/>,
        /// this will be the path to the specific asset.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// If the AssetPackStorageMethod for the pack is <see cref="AssetPackStorageMethod#ApkAssets"/>, this
        /// will be the offset of the asset within the APK.
        ///
        /// If the AssetPackStorageMethod for the pack is <see cref="AssetPackStorageMethod#StorageFiles"/>,
        /// this will be 0.
        /// </summary>
        public uint Offset { get; private set; }

        /// <summary>
        /// If the AssetPackStorageMethod for the pack is <see cref="AssetPackStorageMethod#ApkAssets"/>, this
        /// will be the size of the asset within the APK.
        ///
        /// If the AssetPackStorageMethod for the pack is <see cref="AssetPackStorageMethod#StorageFiles"/>,
        /// this will be the size of the asset file.
        /// </summary>
        public long Size { get; private set; }

        /// <summary>
        /// Creates an AssetPackLocation with all the fields of the underlying Java object, disposing the Java
        /// object in the process.
        /// </summary>
        public AssetLocation(AndroidJavaObject packLocation)
        {
            using (packLocation)
            {
                Path = packLocation.Call<string>("path");

                // Cast to uint because Unity AssetBundle loading APIs expect uint offset.
                Offset = (uint) packLocation.Call<long>("offset");
                Size = packLocation.Call<long>("size");
            }
        }

        public override string ToString()
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendFormat("path: {0}\n", Path);
            stateDescription.AppendFormat("offset: {0}\n", Offset);
            stateDescription.AppendFormat("size: {0}\n", Size);
            return stateDescription.ToString();
        }
    }
}