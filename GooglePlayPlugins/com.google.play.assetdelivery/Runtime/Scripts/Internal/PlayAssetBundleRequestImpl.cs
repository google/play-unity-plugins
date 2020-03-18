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
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    internal class PlayAssetBundleRequestImpl : PlayAssetBundleRequest
    {
        public readonly string MainAssetBundleName;

        private AssetPackManager _assetPackManager;
        private bool _initializedInPlayCore;
        private PlayAssetBundleRequestRepository _requestRepository;
        private Action _onInitializedInPlayCore = delegate { };

        public PlayAssetBundleRequestImpl(string mainAssetBundleName, AssetPackManager assetPackManager,
            PlayAssetBundleRequestRepository requestRepository)
        {
            MainAssetBundleName = mainAssetBundleName;
            _assetPackManager = assetPackManager;
            _requestRepository = requestRepository;
        }

        public void UpdateState(AssetDeliveryStatus status, long bytesDownloaded, long totalBytesToDownload)
        {
            if (totalBytesToDownload == 0L)
            {
                bool finishedDownloading = status == AssetDeliveryStatus.Loading
                                           || status == AssetDeliveryStatus.Loaded;
                DownloadProgress = finishedDownloading ? 1f : 0f;
            }
            else
            {
                DownloadProgress = bytesDownloaded / (float) totalBytesToDownload;
            }

            Status = status;
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

        public void OnErrorOccured(AssetDeliveryErrorCode errorCode)
        {
            if (IsDone)
            {
                return;
            }

            IsDone = true;
            Error = errorCode;
            Status = AssetDeliveryStatus.Failed;
            AssetBundle = null;
            InvokeCompletedEvent();
        }

        public void OnLoadingStarted()
        {
            DownloadProgress = 1f;
            Status = AssetDeliveryStatus.Loading;
        }

        public void OnLoadingFinished(AssetBundle loadedAssetBundle)
        {
            if (IsDone)
            {
                return;
            }

            DownloadProgress = 1f;
            AssetBundle = loadedAssetBundle;
            Status = AssetDeliveryStatus.Loaded;
            IsDone = true;
            InvokeCompletedEvent();
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
            _assetPackManager.Cancel(MainAssetBundleName).Dispose();
        }

        private void DeferCancelPlayCore()
        {
            _onInitializedInPlayCore = () =>
            {
                // Only cancel if a new request hasn't been started, to avoid cancelling the new request.
                if (_requestRepository.ContainsRequest(MainAssetBundleName))
                {
                    Debug.LogFormat("Skip canceling {0}", MainAssetBundleName);
                }
                else
                {
                    CancelPlayCore();
                }
            };
        }
    }
}