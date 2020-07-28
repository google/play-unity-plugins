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
    /// Enum indicating the current status of an asset pack request.
    /// </summary>
    public enum AssetDeliveryStatus
    {
        /// <summary>
        /// Asset pack download is pending and will be processed soon.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Asset pack is being downloaded and transferred to the app's storage.
        /// </summary>
        Retrieving = 1,

        /// <summary>
        /// Asset pack is available on disk. For <see cref="PlayAssetPackRequest"/>s this indicates that the request is
        /// complete.
        /// </summary>
        Available = 2,

        /// <summary>
        /// Contained AssetBundle is being loaded.
        /// </summary>
        Loading = 3,

        /// <summary>
        /// Contained AssetBundle has finished loading, assets can now be loaded. For
        /// <see cref="PlayAssetBundleRequest"/>s this indicates that the request is complete.
        /// </summary>
        Loaded = 4,

        /// <summary>
        /// Asset pack retrieval has failed.
        /// </summary>
        Failed = 5,

        /// <summary>
        /// Asset pack download is paused until the device acquires wifi, or the user confirms a dialog
        /// presented with <see cref="PlayAssetDelivery.ShowCellularDataConfirmation"/>.
        /// </summary>
        WaitingForWifi = 6,
    }
}