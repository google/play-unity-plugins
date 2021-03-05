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
    /// Errors that can be encountered while interacting with asset packs.
    /// </summary>
    public enum AssetDeliveryErrorCode
    {
        /// <summary>
        /// No error has occurred.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// The requesting app is unavailable.
        ///
        /// <para>This could be caused by multiple reasons:
        ///
        /// <list type="bullet">
        ///   <item><description>
        ///     The app isn't published in the Play Store.
        ///   </description></item>
        ///   <item><description>
        ///     The app version code isn't published in the Play Store. Note: an older version may exist.
        ///   </description></item>
        ///   <item><description>
        ///     The app is only available on a testing track that the user doesn't have access to.
        ///   </description></item>
        /// </list>
        /// </para>
        /// </summary>
        AppUnavailable = 1,

        /// <summary>
        /// The requested asset pack is unavailable.
        /// </summary>
        BundleUnavailable = 2,

        /// <summary>
        /// Network error. Unable to obtain asset pack details.
        /// </summary>
        NetworkError = 3,

        /// <summary>
        /// Download not permitted under current device circumstances, for example the app is running in the background.
        /// </summary>
        AccessDenied = 4,

        /// <summary>
        /// Asset pack installation failed due to insufficient storage.
        /// </summary>
        InsufficientStorage = 7,

        /// <summary>
        /// AssetBundle failed to load.
        /// </summary>
        AssetBundleLoadingError = 8,

        /// <summary>
        /// The download was cancelled, likely by the user cancelling the download notification.
        /// </summary>
        Canceled = 9,

        /// <summary>
        /// Unknown error retrieving asset pack.
        /// </summary>
        InternalError = 10,

        /// <summary>
        /// The Play Store app is either not installed or not the official version.
        /// </summary>
        PlayStoreNotFound = 11,

        /// <summary>
        /// Returned if <see cref="PlayAssetDelivery.ShowCellularDataConfirmation"/> is called but
        /// no asset packs are waiting for Wi-Fi.
        /// </summary>
        NetworkUnrestricted = 12,

        /// <summary>
        /// The app isn't owned by any user on this device. An app is "owned" if it has been
        /// acquired from the Play Store.
        /// </summary>
        AppNotOwned = 13,
    }
}