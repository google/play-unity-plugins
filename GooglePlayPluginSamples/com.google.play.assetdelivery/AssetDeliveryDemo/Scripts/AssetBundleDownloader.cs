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
using System.Collections.Generic;
using Google.Play.Common.LoadingScreen;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo
{
    /// <summary>
    /// Provides controls and status displays for downloading AssetBundles via Play Asset Delivery
    /// </summary>
    public class AssetBundleDownloader : MonoBehaviour
    {
        public string AssetBundleName;
        public DownloadDisplay Display;
        public Button RetrieveAssetBundleButton;
        public Button LoadSceneButton;
        public Button ShowCellularDialogButton;
        public Button CancelDownloadButton;
        public Button RemoveButton;
        public List<AssetBundleDownloader> Dependencies;
        public bool ShowSize;

        private AssetBundle _assetBundle;
        private PlayAssetBundleRequest _request;

        public bool IsInitialized { get; private set; }

        public void Start()
        {
            RetrieveAssetBundleButton.onClick.AddListener(ButtonEventRetrieveAssetBundle);
            LoadSceneButton.onClick.AddListener(ButtonEventLoadSceneFromAssetBundle);
            ShowCellularDialogButton.onClick.AddListener(ButtonEventShowCellularDialog);
            CancelDownloadButton.onClick.AddListener(ButtonEventCancelDownload);
            RemoveButton.onClick.AddListener(ButtonEventRemoveAssetBundle);

            Display.BindButton(CancelDownloadButton, AssetDeliveryStatus.Pending);
            Display.BindButton(CancelDownloadButton, AssetDeliveryStatus.Retrieving);
            Display.BindButton(ShowCellularDialogButton, AssetDeliveryStatus.WaitingForWifi);
            Display.BindButton(RemoveButton, AssetDeliveryStatus.Loaded);
            Display.BindButton(LoadSceneButton, AssetDeliveryStatus.Loaded);
            Display.BindButton(RetrieveAssetBundleButton, AssetDeliveryStatus.Failed);

            Display.BindColor(Display.SuccessColor, AssetDeliveryStatus.Loaded);
            Display.BindColor(Display.ErrorColor, AssetDeliveryStatus.Failed);

            Display.SetNameText(AssetBundleName);
            SetInitialStatus();

            if (ShowSize)
            {
                DisplayDownloadSize();
            }
            else
            {
                IsInitialized = true;
            }
        }

        public void ButtonEventRetrieveAssetBundle()
        {
            Display.HideButtons();
            StartCoroutine(DownloadAssetBundleCo());
        }

        public void ButtonEventLoadSceneFromAssetBundle()
        {
            var sceneToLoad = _assetBundle.GetAllScenePaths()[0];
            SceneManager.LoadScene(sceneToLoad);
        }

        public void ButtonEventCancelDownload()
        {
            if (_request == null)
            {
                return;
            }

            _request.AttemptCancel();
        }

        public void ButtonEventRemoveAssetBundle()
        {
            var removeOperation = PlayAssetDelivery.RemoveAssetPack(AssetBundleName);
            removeOperation.Completed += (operation) =>
            {
                if (operation.Error != AssetDeliveryErrorCode.NoError)
                {
                    Debug.LogErrorFormat("Error removing AssetBundle {0}: {1}", AssetBundleName, operation.Error);
                    return;
                }

                SetInitialStatus();
                UnloadAssetBundle();
            };
        }

        public void ButtonEventShowCellularDialog()
        {
            PlayAssetDelivery.ShowCellularDataConfirmation();
        }

        public void Update()
        {
            var dependenciesLoaded = Dependencies.Count == 0 || Dependencies.TrueForAll(dep => dep.IsLoaded());
            LoadSceneButton.interactable = IsLoaded() && dependenciesLoaded;
            Display.SetScrolling(_request == null);
        }

        private void SetInitialStatus()
        {
            Display.ShowButtons(RetrieveAssetBundleButton);
            Display.SetInitialStatus(PlayAssetDelivery.IsDownloaded(AssetBundleName));
        }

        private IEnumerator DownloadAssetBundleCo()
        {
            _request = PlayAssetDelivery.RetrieveAssetBundleAsync(AssetBundleName);

            while (!_request.IsDone)
            {
                if (_request.Status == AssetDeliveryStatus.WaitingForWifi)
                {
                    var asyncOperation = PlayAssetDelivery.ShowCellularDataConfirmation();
                    yield return asyncOperation; // Wait until user has confirmed or cancelled the dialog.
                    if (asyncOperation.Error != AssetDeliveryErrorCode.NoError
                        || asyncOperation.GetResult() != ConfirmationDialogResult.Accepted)
                    {
                        // Provide a button to re-show the dialog in case user changes their mind.
                        Display.SetStatus(_request.Status, _request.Error);
                    }

                    yield return new WaitUntil(() => _request.Status != AssetDeliveryStatus.WaitingForWifi);
                }

                Display.SetProgress(_request.DownloadProgress);
                Display.SetStatus(_request.Status, _request.Error);

                yield return null;
            }

            Display.SetStatus(_request.Status, _request.Error);

            if (_request.Error != AssetDeliveryErrorCode.NoError)
            {
                Debug.LogErrorFormat("Couldn't load AssetBundles: {0}", _request.Error);
                _request = null;
                yield break;
            }

            _assetBundle = _request.AssetBundle;
            _request = null;
        }

        private void DisplayDownloadSize()
        {
            var getSizeOperation = PlayAssetDelivery.GetDownloadSize(AssetBundleName);
            getSizeOperation.Completed += (operation) =>
            {
                if (operation.Error != AssetDeliveryErrorCode.NoError)
                {
                    Debug.LogErrorFormat("Error getting download size for {0}: {1}",
                        AssetBundleName, operation.Error);
                    return;
                }

                IsInitialized = true;
                string nameWithSize =
                    string.Format("{0} : {1}", AssetBundleName, Display.FormatSize(operation.GetResult()));
                Display.SetNameText(nameWithSize);
            };
        }

        private bool IsLoaded()
        {
            return _assetBundle != null;
        }

        private void UnloadAssetBundle()
        {
            if (_assetBundle != null)
            {
                _assetBundle.Unload(false);
                _assetBundle = null;
            }
        }

        private void OnDestroy()
        {
            UnloadAssetBundle();
        }
    }
}