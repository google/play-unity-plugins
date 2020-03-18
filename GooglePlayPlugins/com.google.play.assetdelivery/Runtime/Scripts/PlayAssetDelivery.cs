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

using Google.Play.AssetDelivery.Internal;
using Google.Play.Common;

namespace Google.Play.AssetDelivery
{
    /// <summary>
    /// Provides methods for retrieving asset packs via the Play Asset Delivery system.
    /// Manages in-progress <see cref="PlayAssetBundleRequest"/>s.
    /// </summary>
    public static class PlayAssetDelivery
    {
        private static PlayAssetDeliveryInternal _instance;

        private static PlayAssetDeliveryInternal Instance
        {
            get { return _instance ?? (_instance = new PlayAssetDeliveryInternal()); }
        }

        /// <summary>
        /// Returns whether or not the the latest version of an asset pack is available on disk.
        /// </summary>
        /// <param name="assetPackName">The name of the desired asset pack.</param>
        public static bool IsDownloaded(string assetPackName)
        {
            return Instance.IsDownloaded(assetPackName);
        }

        /// <summary>
        /// Starts a <see cref="PlayAssetBundleRequest"/> to retrieve an asset pack containing an AssetBundle.
        /// Downloads the asset pack if the latest version isn't already available on disk.
        /// After download, the contained AssetBundle is loaded into memory.
        /// </summary>
        /// <param name="assetBundleName">The name of the requested AssetBundle.</param>
        /// <returns>A request object used to monitor the asynchronous AssetBundle retrieval.</returns>
        public static PlayAssetBundleRequest RetrieveAssetBundleAsync(string assetBundleName)
        {
            return Instance.RetrieveAssetBundleAsyncInternal(assetBundleName);
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
        /// If the specified asset pack is currently being retrieved or transferring, this method will not cancel the
        /// process. If the specified asset pack contains an AssetBundle that is already loaded into memory, it will
        /// not be unloaded.
        /// </summary>
        /// <returns>
        /// If the files are deleted successfully, or if the files don't exist, the returned operation will
        /// complete successfully.
        /// Otherwise, the operation will complete with an error code.
        /// </returns>
        public static PlayAsyncOperation<VoidResult, AssetDeliveryErrorCode> RemoveAssetPack(string assetBundleName)
        {
            return Instance.RemoveAssetPackInternal(assetBundleName);
        }

        /// <summary>
        /// Shows a confirmation dialog for all currently downloading asset packs that are
        /// <see cref="AssetDeliveryStatus.WaitingForWifi"/>. If the user accepts the dialog, then those
        /// Asset packs are downloaded over cellular data.
        ///
        /// A <see cref="PlayAssetBundleRequest"/> is set to <see cref="AssetDeliveryStatus.WaitingForWifi"/> if
        /// the user is currently not on a wifi connection and the AssetBundle is large or the user has set
        /// their download preference in the Play Store to only download apps over wifi. By showing this
        /// dialog, the app can ask the user if they accept downloading the asset packs over cellular data
        /// instead of waiting for wifi.
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