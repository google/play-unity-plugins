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

namespace Google.Play.AssetDelivery
{
    /// <summary>
    /// Wraps Play Core's AssetLocation which represents the location of an Asset within an asset pack on disk.
    /// </summary>
    public abstract class AssetLocation
    {
        /// <summary>
        /// If the asset pack is stored within an apk, this will be the path to the APK containing the asset.
        /// Otherwise, this will be the path to the specific asset.
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// If the asset pack is stored within an apk this will be the offset of the asset within the APK.
        /// Otherwise, this will be 0.
        /// </summary>
        public ulong Offset { get; protected set; }

        /// <summary>
        /// If the asset pack is stored within an apk, this will be the size of the asset within the APK.
        /// Otherwise, this will be the size of the asset file.
        /// </summary>
        public ulong Size { get; protected set; }
    }
}