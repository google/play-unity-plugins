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
using System.IO;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    internal class PlayAssetPackRequestImpl : PlayAssetPackRequest
    {
        public readonly string AssetPackName;

        /// <summary>
        /// The name of the folder within an asset pack containing all the pack's assets.
        /// Asset packs packaged using the Unity packaging API will place all their assets under this folder.
        /// </summary>
        private const string AssetPackFolderName = "assetpack";

        private readonly AssetPackManager _assetPackManager;
        private bool _initializedInPlayCore;
        private readonly PlayRequestRepository _requestRepository;
        private Action _onInitializedInPlayCore = delegate { };

        public PlayAssetPackRequestImpl(string assetPackName, AssetPackManager assetPackManager,
            PlayRequestRepository requestRepository)
        {
            AssetPackName = assetPackName;
            _assetPackManager = assetPackManager;
            _requestRepository = requestRepository;
        }

        public override event Action<PlayAssetPackRequest> Completed
        {
            add
            {
                if (IsDone)
                {
                    value.Invoke(this);
                    return;
                }

                base.Completed += value;
            }
            remove { base.Completed -= value; }
        }


        public void UpdateState(AssetDeliveryStatus status, long bytesDownloaded, long totalBytesToDownload)
        {
            if (totalBytesToDownload == 0L)
            {
                bool finishedDownloading = status == AssetDeliveryStatus.Available
                                           || status == AssetDeliveryStatus.Loading
                                           || status == AssetDeliveryStatus.Loaded;
                DownloadProgress = finishedDownloading ? 1f : 0f;
            }
            else
            {
                DownloadProgress = bytesDownloaded / (float) totalBytesToDownload;
            }

            Status = status;
            if (Status == AssetDeliveryStatus.Available)
            {
                OnPackAvailable();
            }
        }

        public void OnInitializedInPlayCore()
        {
            // Execute on the main thread in case _onInitializedInPlayCore needs to check _requestRepository.
            PlayCoreEventHandler.HandleEvent(() =>
            {
                _initializedInPlayCore = true;
                _onInitializedInPlayCore();
            });
        }

        public void OnPackAvailable()
        {
            Status = AssetDeliveryStatus.Available;
            DownloadProgress = 1f;
            if (!IsDone)
            {
                IsDone = true;
                InvokeCompletedEvent();
            }
        }

        public void OnErrorOccured(AssetDeliveryErrorCode errorCode)
        {
            if (IsDone)
            {
                return;
            }

            IsDone = true;
            Error = errorCode;
            Status = AssetDeliveryStatus.Failed;
            InvokeCompletedEvent();
        }

        public override AssetBundleCreateRequest LoadAssetBundleAsync(string assetBundlePath)
        {
            var assetLocation = GetAssetLocation(assetBundlePath);
            if (assetLocation == null)
            {
                // Check if the assetBundlePath was relative to "assets" path rather than to the "assets/assetpack"
                // path. This may be a common error, so we check for it here and warn the developer.
                if (AssetPackFolderName.Equals(assetBundlePath.Split('/')[0]))
                {
                    Debug.LogError("Failed to find the asset at path {0} within the asset pack, possibly because the" +
                                   " path begins with \"assetpack\".");
                }

                return null;
            }

            return AssetBundle.LoadFromFileAsync(assetLocation.Path, /* crc= */ 0, assetLocation.Offset);
        }

        public override AssetLocation GetAssetLocation(string assetPath)
        {
            var fullAssetPath = Path.Combine(AssetPackFolderName, assetPath);
            return _assetPackManager.GetAssetLocation(AssetPackName, fullAssetPath);
        }

        public override void AttemptCancel()
        {
            if (IsDone || Status == AssetDeliveryStatus.Loaded)
            {
                return;
            }

            // If play core is aware of the associated pack, then we tell play core to cancel the pack download.
            // Otherwise, we notify play core after the fetch task succeeds.
            if (_initializedInPlayCore)
            {
                CancelPlayCore();
            }
            else
            {
                DeferCancelPlayCore();
            }

            OnErrorOccured(AssetDeliveryErrorCode.Canceled);
        }

        private void CancelPlayCore()
        {
            _assetPackManager.Cancel(AssetPackName).Dispose();
        }

        private void DeferCancelPlayCore()
        {
            _onInitializedInPlayCore = () =>
            {
                // Only cancel if a new request hasn't been started, to avoid cancelling the new request.
                if (_requestRepository.ContainsRequest(AssetPackName))
                {
                    Debug.LogFormat("Skip canceling {0}", AssetPackName);
                }
                else
                {
                    CancelPlayCore();
                }
            };
        }
    }
}