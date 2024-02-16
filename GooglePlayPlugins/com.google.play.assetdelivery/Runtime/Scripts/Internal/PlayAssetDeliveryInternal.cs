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
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly AssetDeliveryUpdateHandler _updateHandler;

        private readonly PlayRequestRepository _requestRepository = new PlayRequestRepository();

        internal PlayAssetDeliveryInternal()
        {
            _assetPackManager = new AssetPackManager();
            _updateHandler = AssetDeliveryUpdateHandler.CreateInScene(_assetPackManager, _requestRepository);
            _updateHandler.OnStateUpdateEvent += ProcessPackStateUpdate;
            PlayCoreEventHandler.CreateInScene();
        }

        internal bool IsDownloaded(string assetBundleName)
        {
            return _assetPackManager.GetPackLocation(assetBundleName) != null;
        }

        internal PlayAssetBundleRequest RetrieveAssetBundleAsyncInternal(string assetBundleName)
        {
            var request = InitializeAssetBundleRequest(assetBundleName);
            InitiateRequest(request.PackRequest);

            return request;
        }

        internal PlayAssetBundleRequest RetrieveAssetBundleAsyncInternal(string assetBundleName, bool updateIfAvailable)
        {
            var request = InitializeAssetBundleRequest(assetBundleName);
            InitiateRequest(request.PackRequest, updateIfAvailable);
            return request;
        }

        private PlayAssetBundleRequestImpl InitializeAssetBundleRequest(string assetBundleName)
        {
            if (_requestRepository.ContainsRequest(assetBundleName))
            {
                throw new ArgumentException(string.Format("There is already an active request for AssetBundle: {0}",
                    assetBundleName));
            }

            var request = CreateAssetBundleRequest(assetBundleName);
            _requestRepository.AddAssetBundleRequest(request);
            request.Completed += req => _requestRepository.RemoveRequest(assetBundleName);

            return request;
        }

        internal PlayAssetPackRequest RetrieveAssetPackAsyncInternal(string assetPackName)
        {
            var request = GetExistingAssetPackRequest(assetPackName);
            if (request != null)
            {
                return request;
            }

            request = InitializeAssetPackRequest(assetPackName);
            InitiateRequest(request);

            return request;
        }

        internal PlayAssetPackRequest RetrieveAssetPackAsyncInternal(string assetPackName, bool updateIfAvailable)
        {
            var request = GetExistingAssetPackRequest(assetPackName);
            if (request != null)
            {
                return request;
            }

            request = InitializeAssetPackRequest(assetPackName);
            InitiateRequest(request, updateIfAvailable);

            return request;
        }

        internal PlayAssetPackBatchRequest RetrieveAssetPackBatchAsyncInternal(IList<string> assetPackNames)
        {
            return CreateAndInitiateBatchRequest(assetPackNames, IsDownloaded);
        }

        internal PlayAssetPackBatchRequest RetrieveAssetPackBatchAsyncInternal(IList<string> assetPackNames,
            bool updateIfAvailable)
        {
            Func<string, bool> useAvailablePack = packName => IsCurrentVersionGoodEnough(packName, updateIfAvailable);
            return CreateAndInitiateBatchRequest(assetPackNames, useAvailablePack);
        }

        private PlayAssetPackBatchRequest CreateAndInitiateBatchRequest(IList<string> assetPackNames,
            Func<string, bool> isPackAvailable)
        {
            if (assetPackNames.Count != assetPackNames.Distinct().Count())
            {
                throw new ArgumentException("assetPackNames contains duplicate entries");
            }

            var activeAssetBundleRequestNames = assetPackNames
                .Where(name => _requestRepository.ContainsAssetBundleRequest(name))
                .ToArray();

            if (activeAssetBundleRequestNames.Length != 0)
            {
                throw new ArgumentException("Cannot create a new PlayAssetPackBatchRequests because there are " +
                                            "already PlayAssetBundleRequests for AssetBundles: {0}",
                    string.Join(", ", activeAssetBundleRequestNames));
            }

            // A subset of assetPackNames of packs that are not available on disk.
            var unavailableAssetPackNames = new List<string>();
            var requests = new List<PlayAssetPackRequestImpl>();
            foreach (var assetPackName in assetPackNames)
            {
                PlayAssetPackRequestImpl request;
                if (_requestRepository.TryGetRequest(assetPackName, out request))
                {
                    requests.Add(request);
                    continue;
                }

                request = CreateAssetPackRequest(assetPackName);
                _requestRepository.AddRequest(request);
                request.Completed += req => _requestRepository.RemoveRequest(request.AssetPackName);
                requests.Add(request);

                if (isPackAvailable.Invoke(assetPackName))
                {
                    request.OnPackAvailable();
                }
                else
                {
                    unavailableAssetPackNames.Add(assetPackName);
                }
            }

            var batchRequest = new PlayAssetPackBatchRequestImpl(requests);
            var fetchTask = _assetPackManager.Fetch(unavailableAssetPackNames.ToArray());
            fetchTask.RegisterOnSuccessCallback(javaPackStates =>
            {
                batchRequest.OnInitializedInPlayCore();
                fetchTask.Dispose();
            });
            fetchTask.RegisterOnFailureCallback((reason, errorCode) =>
            {
                Debug.LogError("Failed to retrieve asset pack batch: " + reason);
                batchRequest.OnInitializationErrorOccurred(PlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                fetchTask.Dispose();
            });

            return batchRequest;
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

        internal PlayAsyncOperation<ConfirmationDialogResult, AssetDeliveryErrorCode>
            ShowConfirmationDialogInternal()
        {
            var requestsAwaitingWifi = _requestRepository.GetRequestsWithStatus(AssetDeliveryStatus.WaitingForWifi);
            var requestsAwaitingConsent = _requestRepository.GetRequestsWithStatus(AssetDeliveryStatus.RequiresUserConfirmation);
            if (requestsAwaitingWifi.Count == 0 && requestsAwaitingConsent.Count == 0)
            {
                throw new Exception("There are no active requests waiting for confirmation.");
            }

            var task = _assetPackManager.ShowConfirmationDialog();
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

        internal PlayAsyncOperation<IDictionary<string, PlayAssetPackDownloadInfo>, AssetDeliveryErrorCode>
            GetDownloadInfoInternal(IList<string> assetPackNames)
        {
            var operation = new AssetDeliveryAsyncOperation<IDictionary<string, PlayAssetPackDownloadInfo>>();

            var task = _assetPackManager.GetPackStates(assetPackNames.ToArray());
            task.RegisterOnSuccessCallback(javaPackStates =>
            {
                operation.SetResult(PlayAssetPackDownloadInfoImpl.FromAssetPackStates(javaPackStates));
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

        private PlayAssetPackRequestImpl GetExistingAssetPackRequest(string assetPackName)
        {
            if (_requestRepository.ContainsAssetBundleRequest(assetPackName))
            {
                throw new ArgumentException(string.Format(
                    "Cannot create a new PlayAssetPackRequest because there is already an active " +
                    "PlayAssetBundleRequest for asset bundle: {0}",
                    assetPackName));
            }

            PlayAssetPackRequestImpl request;
            if (_requestRepository.TryGetRequest(assetPackName, out request))
            {
                Debug.Log("Returning existing active request for " + assetPackName);
            }

            return request;
        }

        private PlayAssetPackRequestImpl InitializeAssetPackRequest(string assetPackName)
        {
            var request = CreateAssetPackRequest(assetPackName);
            _requestRepository.AddRequest(request);
            request.Completed += req => _requestRepository.RemoveRequest(assetPackName);

            return request;
        }


        private PlayAssetPackRequestImpl CreateAssetPackRequest(string assetPackName)
        {
            return new PlayAssetPackRequestImpl(assetPackName, _assetPackManager, _requestRepository);
        }

        private PlayAssetBundleRequestImpl CreateAssetBundleRequest(string assetBundleName)
        {
            var packRequest = CreateAssetPackRequest(assetBundleName);
            return new PlayAssetBundleRequestImpl(packRequest, _updateHandler);
        }

        private void InitiateRequest(PlayAssetPackRequestImpl request)
        {
            StartRequest(request, IsDownloaded(request.AssetPackName));
        }

        private void InitiateRequest(PlayAssetPackRequestImpl request, bool updateIfAvailable)
        {
            StartRequest(request, IsCurrentVersionGoodEnough(request.AssetPackName, updateIfAvailable));
        }

        private bool IsCurrentVersionGoodEnough(string assetPackName, bool wantNewerVersion)
        {
            var packLocation = _assetPackManager.GetPackLocation(assetPackName);
            bool isCurrentVersionGoodEnough;
            if (packLocation == null)
            {
                isCurrentVersionGoodEnough= false;
            }
            else if (packLocation.PackStorageMethod == AssetPackStorageMethod.ApkAssets)
            {
                // If the pack exists as an APK, then it is an install-time pack which cannot be updated.
                isCurrentVersionGoodEnough = true;
            }
            else
            {
                // When wantNewerVersion is true,
                // we can't use the version on disk without checking if a newer version exists.
                isCurrentVersionGoodEnough = !wantNewerVersion;
            }

            return isCurrentVersionGoodEnough;
        }

        private void StartRequest(PlayAssetPackRequestImpl request, bool isAvailable)
        {
            if (isAvailable)
            {
                request.OnPackAvailable();
            }
            else
            {
                var fetchTask = _assetPackManager.Fetch(request.AssetPackName);
                fetchTask.RegisterOnSuccessCallback(javaPackStates =>
                {
                    request.OnInitializedInPlayCore();
                    fetchTask.Dispose();
                });
                fetchTask.RegisterOnFailureCallback((reason, errorCode) =>
                {
                    Debug.LogErrorFormat("Failed to retrieve asset pack: {0}", reason);
                    request.OnErrorOccured(PlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                    fetchTask.Dispose();
                });
            }
        }

        private void ProcessPackStateUpdate(AssetPackState newState)
        {
            PlayAssetPackRequestImpl request;
            if (!_requestRepository.TryGetRequest(newState.Name, out request))
            {
                Debug.LogWarningFormat(
                    "Received state update \"{0}\", that is not associated with an active request.",
                    newState.Name);
                return;
            }

            UpdateRequest(request, newState, newState.ErrorCode);
        }

        private void UpdateRequest(PlayAssetPackRequestImpl request, AssetPackState newState, int errorCode)
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

            request.UpdateState(PlayCoreTranslator.TranslatePlayCorePackStatus(newState.Status),
                newState.BytesDownloaded,
                newState.TotalBytesToDownload);
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