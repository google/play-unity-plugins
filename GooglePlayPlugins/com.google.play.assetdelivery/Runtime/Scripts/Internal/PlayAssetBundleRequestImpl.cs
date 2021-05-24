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
using System.Collections;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    internal class PlayAssetBundleRequestImpl : PlayAssetBundleRequest
    {
        internal readonly PlayAssetPackRequestImpl PackRequest;

        private readonly AssetDeliveryUpdateHandler _updateHandler;
        private AssetDeliveryStatus _loadingStatus;
        private AssetDeliveryErrorCode _loadingError;

        public PlayAssetBundleRequestImpl(PlayAssetPackRequestImpl packRequest,
            AssetDeliveryUpdateHandler updateHandler)
        {
            PackRequest = packRequest;
            _updateHandler = updateHandler;
            _loadingStatus = AssetDeliveryStatus.Pending;
            _loadingError = AssetDeliveryErrorCode.NoError;
            PackRequest.Completed += OnPackRequestCompleted;
        }

        public override event Action<PlayAssetBundleRequest> Completed
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

        public override float DownloadProgress
        {
            get { return PackRequest.DownloadProgress; }
        }

        public override AssetDeliveryStatus Status
        {
            get { return PackRequest.Status == AssetDeliveryStatus.Available ? _loadingStatus : PackRequest.Status; }
        }

        public override AssetDeliveryErrorCode Error
        {
            get { return PackRequest.Error == AssetDeliveryErrorCode.NoError ? _loadingError : PackRequest.Error; }
        }

        private void OnLoadingErrorOccurred(AssetDeliveryErrorCode errorCode)
        {
            if (IsDone)
            {
                return;
            }

            IsDone = true;
            _loadingError = errorCode;
            _loadingStatus = AssetDeliveryStatus.Failed;
            AssetBundle = null;
            InvokeCompletedEvent();
        }

        private void OnLoadingFinished(AssetBundle loadedAssetBundle)
        {
            if (IsDone)
            {
                return;
            }

            DownloadProgress = 1f;
            AssetBundle = loadedAssetBundle;
            _loadingStatus = AssetDeliveryStatus.Loaded;
            IsDone = true;
            InvokeCompletedEvent();
        }

        private void OnPackRequestCompleted(PlayAssetPackRequest packRequest)
        {
            packRequest.Completed -= OnPackRequestCompleted;
            if (packRequest.Error == AssetDeliveryErrorCode.NoError)
            {
                StartLoadingAssetBundle();
            }
            else
            {
                IsDone = true;
                InvokeCompletedEvent();
            }
        }

        private void StartLoadingAssetBundle()
        {
            _updateHandler.StartCoroutine(CoLoadAssetBundle());
            DownloadProgress = 1f;
            _loadingStatus = AssetDeliveryStatus.Loading;
        }

        private IEnumerator CoLoadAssetBundle()
        {
            // Assume that the AssetBundle name equals the asset pack name.
            var bundleCreateRequest = PackRequest.LoadAssetBundleAsync(PackRequest.AssetPackName);
            yield return bundleCreateRequest;

            if (bundleCreateRequest.assetBundle == null)
            {
                OnLoadingErrorOccurred(AssetDeliveryErrorCode.AssetBundleLoadingError);
                yield break;
            }

            OnLoadingFinished(bundleCreateRequest.assetBundle);
        }

        public override void AttemptCancel()
        {
            PackRequest.AttemptCancel();
        }
    }
}