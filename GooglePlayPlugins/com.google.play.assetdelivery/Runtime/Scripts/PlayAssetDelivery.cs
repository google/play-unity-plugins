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

using System;
using System.Collections.Generic;
using Google.Play.AssetDelivery.Internal;
using Google.Play.Common;

namespace Google.Play.AssetDelivery
{
    /// <summary>
    /// Provides methods for retrieving asset packs via the Play Asset Delivery system.
    /// </summary>
    public static class PlayAssetDelivery
    {
        private static PlayAssetDeliveryInternal _instance;

        private static PlayAssetDeliveryInternal Instance
        {
            get { return _instance ?? (_instance = new PlayAssetDeliveryInternal()); }
        }

        /// <summary>
        /// Returns whether or not the specified asset pack is available on disk.
        /// </summary>
        /// <param name="assetPackName">The name of the desired asset pack.</param>
        /// <returns>True if the asset pack is available on disk and false if not.</returns>
        public static bool IsDownloaded(string assetPackName)
        {
            return Instance.IsDownloaded(assetPackName);
        }

        /// <summary>
        /// Starts a <see cref="PlayAssetBundleRequest"/> to retrieve an asset pack containing only
        /// the specified AssetBundle.
        /// Both the AssetBundle and asset pack must share the same name. Downloads the asset pack if
        /// it isn't already available on disk.
        ///
        /// After download, the contained AssetBundle is loaded into memory before the request completes.
        /// </summary>
        /// <param name="assetBundleName">The name of the requested AssetBundle.</param>
        /// <returns>A request object used to monitor the asynchronous AssetBundle retrieval.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if there is already an active request with the specified name.
        /// </exception>
        public static PlayAssetBundleRequest RetrieveAssetBundleAsync(string assetBundleName)
        {
            return Instance.RetrieveAssetBundleAsyncInternal(assetBundleName);
        }


        /// <summary>
        /// Starts a <see cref="PlayAssetPackRequest"/> to retrieve the specified asset pack.
        /// Downloads the asset pack if it isn't already available on disk.
        ///
        /// After download, the assets and/or AssetBundles contained in the asset pack are <b>not</b>
        /// loaded into memory. To load them see <see cref="PlayAssetPackRequest.GetAssetLocation"/>
        /// or <see cref="PlayAssetPackRequest.LoadAssetBundleAsync"/>.
        /// </summary>
        /// <param name="assetPackName">The name of the requested asset pack.</param>
        /// <returns>A request object used to monitor the asynchronous asset pack retrieval.</returns>
        public static PlayAssetPackRequest RetrieveAssetPackAsync(string assetPackName)
        {
            return Instance.RetrieveAssetPackAsyncInternal(assetPackName);
        }


        /// <summary>
        /// Starts a <see cref="PlayAssetPackBatchRequest"/> to retrieve the specified asset packs.
        /// Downloads the asset packs if they aren't already available on disk.
        ///
        /// After download, the assets and/or AssetBundles contained in the asset pack are <b>not</b>
        /// loaded into memory. To load them use <see cref="PlayAssetPackRequest.GetAssetLocation"/>
        /// or <see cref="PlayAssetPackRequest.LoadAssetBundleAsync"/> on the values of the
        /// <see cref="PlayAssetPackBatchRequest.Requests"/> dictionary.
        /// </summary>
        /// <param name="assetPackNames">A list of requested asset packs.</param>
        /// <returns>A request object used to monitor the asynchronous asset pack batch retrieval.</returns>
        /// <exception cref="ArgumentException">Throws if assetPackNames contains duplicate entries.</exception>
        public static PlayAssetPackBatchRequest RetrieveAssetPackBatchAsync(IList<string> assetPackNames)
        {
            return Instance.RetrieveAssetPackBatchAsyncInternal(assetPackNames);
        }


        /// <summary>
        /// Starts a PlayAsyncOperation to determine the download size in bytes of the specified asset pack.
        /// If the specified asset pack's delivery mode is install-time, then the download size will always be 0.
        /// </summary>
        public static PlayAsyncOperation<long, AssetDeliveryErrorCode> GetDownloadSize(string assetPackName)
        {
            return Instance.GetDownloadSizeInternal(assetPackName);
        }


        /// <summary>
        /// Starts a PlayAsyncOperation to delete the specified asset pack from internal storage.
        /// If the specified asset pack is currently being retrieved, this method will not cancel the
        /// retrieval. If the specified asset pack contains any AssetBundles that are already loaded into
        /// memory, the AssetBundles will <b>not</b> be unloaded.
        /// </summary>
        /// <returns>
        /// An async operation object used to monitor the asset pack removal. If the files are deleted
        /// successfully or if the files don't exist, the returned operation will complete successfully.
        /// Otherwise, the operation will complete with an error code.
        /// </returns>
        public static PlayAsyncOperation<VoidResult, AssetDeliveryErrorCode> RemoveAssetPack(string assetPackName)
        {
            return Instance.RemoveAssetPackInternal(assetPackName);
        }

        /// <summary>
        /// Shows a confirmation dialog for all currently downloading asset packs that are
        /// <see cref="AssetDeliveryStatus.WaitingForWifi"/>.
        /// If the user accepts the dialog, then those asset packs are downloaded over cellular data.
        ///
        /// A <see cref="PlayAssetBundleRequest"/> is set to <see cref="AssetDeliveryStatus.WaitingForWifi"/> if
        /// the user is currently not on a Wi-Fi connection and the AssetBundle is large or the user has set
        /// their download preference in the Play Store to only download apps over Wi-Fi. By showing this
        /// dialog, the app can ask the user if they accept downloading the asset packs over cellular data
        /// instead of waiting for Wi-Fi.
        /// </summary>
        /// <returns>
        /// A <see cref="PlayAsyncOperation{ConfirmationDialogResult, AssetDeliveryErrorCode}"/> that completes
        /// once the dialog has been accepted, denied, or closed.
        /// </returns>
        public static PlayAsyncOperation<ConfirmationDialogResult, AssetDeliveryErrorCode>
            ShowCellularDataConfirmation()
        {
            return Instance.ShowCellularDataConfirmationInternal();
        }
    }
}