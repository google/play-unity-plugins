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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Play.Common;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Internal implementation of <see cref="PlayAssetDelivery"/> methods.
    /// </summary>
    internal class PlayAssetDeliveryInternal
    {
        /// <summary>
        /// The subfolder, inside the "assets" directory, into which an asset pack file is stored.
        /// This intermediate folder can be useful for targeting (by texture compression format) at build time.
        /// </summary>
        private const string AssetPackFolderName = "assetpack";

        private readonly AssetPackManager _assetPackManager;
        private readonly AssetPackStateUpdateListener _stateUpdateListener;
        private readonly AssetDeliveryUpdateHandler _updateHandler;

        private readonly PlayAssetBundleRequestRepository _requestRepository = new PlayAssetBundleRequestRepository();

        // A dictionary from AssetBundle name to AssetBundle loading coroutines that keeps track of AssetBundles that
        // are currently being loaded.
        private readonly IDictionary<string, Coroutine> _loadingCoroutinesByName = new Dictionary<string, Coroutine>();

        internal PlayAssetDeliveryInternal()
        {
            _assetPackManager = new AssetPackManager();
            _updateHandler = AssetDeliveryUpdateHandler.CreateInScene(_assetPackManager, _requestRepository);
            _updateHandler.OnStateUpdateEvent += ProcessPackStateUpdate;
            PlayCoreEventHandler.CreateInScene();
        }

        internal bool IsDownloaded(string assetBundleName)
        {
            return GetAssetLocation(assetBundleName) != null;
        }

        internal PlayAssetBundleRequest RetrieveAssetBundleAsyncInternal(string assetBundleName)
        {
            if (_requestRepository.ContainsRequest(assetBundleName))
            {
                throw new ArgumentException(string.Format("There is already an active request for AssetBundle: {0}",
                    assetBundleName));
            }

            var request = CreateAssetBundleRequest(assetBundleName);

            if (IsDownloaded(assetBundleName))
            {
                StartLoadingAssetBundle(request);
                request.OnLoadingStarted();
            }
            else
            {
                var fetchTask = _assetPackManager.Fetch(assetBundleName);
                fetchTask.RegisterOnSuccessCallback(javaPackStates =>
                {
                    request.OnInitializedInPlayCore();
                    fetchTask.Dispose();
                });
                fetchTask.RegisterOnFailureCallback((reason, errorCode) =>
                {
                    Debug.LogErrorFormat("Failed to retrieve AssetBundle {0}: ", reason);
                    request.OnErrorOccured(PlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                    fetchTask.Dispose();
                });
            }

            return request;
        }

        internal PlayAsyncOperation<ConfirmationDialogResult, AssetDeliveryErrorCode>
            ShowCellularDataConfirmationInternal()
        {
            var requests = _requestRepository.GetRequestsWithStatus(AssetDeliveryStatus.WaitingForWifi);
            if (requests.Count == 0)
            {
                throw new Exception("There are no active requests waiting for wifi.");
            }

            var task = _assetPackManager.ShowCellularDataConfirmation();
            var operation = new AssetDeliveryAsyncOperation<ConfirmationDialogResult>();
            task.RegisterOnSuccessCallback(resultCode =>
            {
                operation.SetResult(ConvertToConfirmationDialogResult(resultCode));
                task.Dispose();
            });
            task.RegisterOnFailureCallback((message, errorCode) =>
            {
                operation.SetError(PlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                task.Dispose();
            });
            return operation;
        }

        internal PlayAsyncOperation<long, AssetDeliveryErrorCode> GetDownloadSizeInternal(string assetBundleName)
        {
            var operation = new AssetDeliveryAsyncOperation<long>();

            if (IsInstallTimeAssetBundle(assetBundleName))
            {
                operation.SetResult(0L);
                return operation;
            }

            var task = _assetPackManager.GetPackStates(assetBundleName);
            task.RegisterOnSuccessCallback(javaPackState =>
            {
                var assetPacks = new AssetPackStates(javaPackState);
                operation.SetResult(assetPacks.TotalBytes);
                task.Dispose();
            });
            task.RegisterOnFailureCallback((message, errorCode) =>
            {
                operation.SetError(PlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                task.Dispose();
            });
            return operation;
        }

        internal PlayAsyncOperation<VoidResult, AssetDeliveryErrorCode> RemoveAssetPackInternal(
            string assetBundleName)
        {
            var operation = new AssetDeliveryAsyncOperation<VoidResult>();
            var task = _assetPackManager.RemovePack(assetBundleName);
            task.RegisterOnSuccessCallback(javaPackState =>
            {
                operation.SetResult(new VoidResult());
                task.Dispose();
            });
            task.RegisterOnFailureCallback((message, errorCode) =>
            {
                operation.SetError(AssetDeliveryErrorCode.InternalError);
                task.Dispose();
            });
            return operation;
        }

        private AssetLocation GetAssetLocation(string assetBundleName)
        {
            var assetPath = Path.Combine(AssetPackFolderName, assetBundleName);
            return _assetPackManager.GetAssetLocation(assetBundleName, assetPath);
        }

        private PlayAssetBundleRequestImpl CreateAssetBundleRequest(string assetBundleName)
        {
            var request = new PlayAssetBundleRequestImpl(assetBundleName, _assetPackManager, _requestRepository);
            _requestRepository.AddRequest(request);
            request.Completed += (req) => _requestRepository.RemoveRequest(assetBundleName);
            return request;
        }

        private void ProcessPackStateUpdate(AssetPackState newState)
        {
            PlayAssetBundleRequestImpl request;
            if (!_requestRepository.TryGetRequest(newState.Name, out request))
            {
                Debug.LogWarningFormat(
                    "Received state update \"{0}\", that is not associated with an active request.",
                    newState.Name);
                return;
            }

            UpdateRequest(request, newState, newState.ErrorCode);
        }

        private void UpdateRequest(
            PlayAssetBundleRequestImpl request,
            AssetPackState newState,
            int errorCode)
        {
            if (request.IsDone)
            {
                // Ignore pack state updates associated with completed requests.
                return;
            }

            var assetDeliveryErrorCode = PlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode);
            if (assetDeliveryErrorCode != AssetDeliveryErrorCode.NoError)
            {
                request.OnErrorOccured(assetDeliveryErrorCode);
                return;
            }

            if (newState.Status == PlayCoreTranslator.AssetPackStatus.Canceled)
            {
                request.OnErrorOccured(AssetDeliveryErrorCode.Canceled);
                return;
            }

            if (newState.Status == PlayCoreTranslator.AssetPackStatus.Completed)
            {
                var isRequestLoadingAssetBundle = _loadingCoroutinesByName.ContainsKey(request.MainAssetBundleName);
                if (!isRequestLoadingAssetBundle)
                {
                    var startedSuccessfully = StartLoadingAssetBundle(request);
                    if (!startedSuccessfully)
                    {
                        request.OnErrorOccured(AssetDeliveryErrorCode.AssetBundleLoadingError);
                        return;
                    }
                }

                request.OnLoadingStarted();
                return;
            }

            request.UpdateState(PlayCoreTranslator.TranslatePlayCorePackStatus(newState.Status),
                newState.BytesDownloaded,
                newState.TotalBytesToDownload);
        }

        private bool StartLoadingAssetBundle(PlayAssetBundleRequestImpl request)
        {
            var assetLocation = GetAssetLocation(request.MainAssetBundleName);
            if (assetLocation == null)
            {
                return false;
            }

            var loadingCo =
                _updateHandler.StartCoroutine(CoLoadAssetBundle(
                    request.MainAssetBundleName,
                    assetLocation.Path,
                    assetLocation.Offset));

            _loadingCoroutinesByName.Add(request.MainAssetBundleName, loadingCo);
            return true;
        }

        private IEnumerator CoLoadAssetBundle(string assetBundleName, string assetBundlePath, uint offset)
        {
            try
            {
                var bundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundlePath, /* crc= */ 0, offset);
                yield return bundleCreateRequest;
                NotifyRequestLoadingComplete(assetBundleName, bundleCreateRequest.assetBundle);
            }
            finally
            {
                _loadingCoroutinesByName.Remove(assetBundleName);
            }
        }

        private void NotifyRequestLoadingComplete(string assetBundleName, AssetBundle loadedBundle)
        {
            PlayAssetBundleRequestImpl request;
            if (!_requestRepository.TryGetRequest(assetBundleName, out request))
            {
                if (loadedBundle == null)
                {
                    Debug.LogError("AssetBundle failed to load, and the associated " +
                                   "PlayAssetBundleRequest could not be found.");
                }
                else
                {
                    Debug.LogErrorFormat("AssetBundle \"{0}\" finished loading, but the associated " +
                                         "PlayAssetBundleRequest could not be found.", loadedBundle.name);
                }

                return;
            }

            if (loadedBundle == null)
            {
                request.OnErrorOccured(AssetDeliveryErrorCode.AssetBundleLoadingError);
                return;
            }

            request.OnLoadingFinished(loadedBundle);
        }

        private bool IsInstallTimeAssetBundle(string assetBundleName)
        {
            var packLocation = _assetPackManager.GetPackLocation(assetBundleName);
            return packLocation != null && packLocation.PackStorageMethod == AssetPackStorageMethod.ApkAssets;
        }

        private ConfirmationDialogResult ConvertToConfirmationDialogResult(int resultCode)
        {
            ConfirmationDialogResult dialogResult;
            switch (resultCode)
            {
                case ActivityResult.ResultOk:
                    dialogResult = ConfirmationDialogResult.Accepted;
                    break;
                case ActivityResult.ResultCancelled:
                    dialogResult = ConfirmationDialogResult.Denied;
                    break;
                default:
                    throw new NotImplementedException("Unexpected activity result: " + resultCode);
            }

            return dialogResult;
        }
    }
}
