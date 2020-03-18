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

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Wraps Play Core's AssetPackLocation which represents the location of an asset pack on disk.
    /// </summary>
    internal enum AssetPackStorageMethod
    {
        /// <summary>
        /// Indicates that the asset pack is unpackaged into a folder containing individual asset files.
        /// Assets contained by this asset pack can be accessed via standard File APIs.
        /// </summary>
        StorageFiles = 0,

        /// <summary>
        /// Indicates that the asset pack is installed as APKs containing asset files.
        /// Assets contained by this asset pack can be accessed via Asset Manager.
        /// </summary>
        ApkAssets = 1,
    }
}