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
    public class AssetBundleDownloadDisplay : MonoBehaviour
    {
        public string AssetBundleName;
        public Text StatusText;
        public Text NameText;
        public Text RetrieveAssetBundleButtonText;
        public Image ColorTint;
        public LoadingBar LoadingBar;
        public ScrollingFillAnimator ScrollingFill;
        public Button RetrieveAssetBundleButton;
        public Button LoadSceneButton;
        public Button ShowCellularDialogButton;
        public Button CancelDownloadButton;
        public Button RemoveButton;
        public RectTransform ButtonOutline;
        public Color ErrorColor;
        public Color SuccessColor;
        public Color NeutralColor;
        public List<AssetBundleDownloadDisplay> Dependencies;
        public bool ShowSize;

        private AssetBundle _assetBundle;
        private PlayAssetBundleRequest _request;
        private List<Button> _buttons;

        private const float ActiveScrollSpeed = 2.5f;

        public bool IsInitialized { get; private set; }

        public void Start()
        {
            RetrieveAssetBundleButton.onClick.AddListener(ButtonEventRetrieveAssetBundle);
            LoadSceneButton.onClick.AddListener(ButtonEventLoadSceneFromAssetBundle);
            ShowCellularDialogButton.onClick.AddListener(ButtonEventShowCellularDialog);
            CancelDownloadButton.onClick.AddListener(ButtonEventCancelDownload);
            RemoveButton.onClick.AddListener(ButtonEventRemoveAssetBundle);

            _buttons = new List<Button>
            {
                RetrieveAssetBundleButton, LoadSceneButton, ShowCellularDialogButton, CancelDownloadButton, RemoveButton
            };

            NameText.text = AssetBundleName;
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
            HideButtons();
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
            ScrollingFill.ScrollSpeed = _request == null ? 0 : ActiveScrollSpeed;
        }

        private void SetInitialStatus()
        {
            ColorTint.color = NeutralColor;
            LoadingBar.SetProgress(0f);
            ShowButtons(RetrieveAssetBundleButton);
            StatusText.text = PlayAssetDelivery.IsDownloaded(AssetBundleName)
                ? AssetDeliveryStatus.Available.ToString()
                : AssetDeliveryStatus.Pending.ToString();
        }

        private void SetStatus(AssetDeliveryStatus status, AssetDeliveryErrorCode error)
        {
            StatusText.text = status.ToString();

            switch (status)
            {
                case AssetDeliveryStatus.Pending:
                case AssetDeliveryStatus.Retrieving:
                    ShowButtons(CancelDownloadButton);
                    ColorTint.color = NeutralColor;
                    break;
                case AssetDeliveryStatus.WaitingForWifi:
                    ShowButtons(ShowCellularDialogButton);
                    ColorTint.color = NeutralColor;
                    break;
                case AssetDeliveryStatus.Loading:
                    HideButtons();
                    ColorTint.color = NeutralColor;
                    break;
                case AssetDeliveryStatus.Loaded:
                    ShowButtons(LoadSceneButton, RemoveButton);
                    ColorTint.color = SuccessColor;
                    break;
                case AssetDeliveryStatus.Failed:
                    ShowButtons(RetrieveAssetBundleButton);
                    ColorTint.color = ErrorColor;
                    break;
                default:
                    HideButtons();
                    ColorTint.color = NeutralColor;
                    break;
            }

            if (error != AssetDeliveryErrorCode.NoError)
            {
                StatusText.text = string.Format("{0}: {1}", status.ToString(), error.ToString());
                RetrieveAssetBundleButtonText.text = "Try Again";
            }
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
                        SetStatus(_request.Status, _request.Error);
                    }

                    yield return new WaitUntil(() => _request.Status != AssetDeliveryStatus.WaitingForWifi);
                }

                LoadingBar.SetProgress(_request.DownloadProgress);
                SetStatus(_request.Status, _request.Error);

                yield return null;
            }

            SetStatus(_request.Status, _request.Error);

            if (_request.Error != AssetDeliveryErrorCode.NoError)
            {
                Debug.LogErrorFormat("Couldn't load AssetBundles: {0}", _request.Error);
                _request = null;
                yield break;
            }

            _assetBundle = _request.AssetBundle;
            _request = null;
        }

        private void HideButtons()
        {
            foreach (var button in _buttons)
            {
                button.gameObject.SetActive(false);
            }

            ButtonOutline.gameObject.SetActive(false);
        }

        private void ShowButtons(params Button[] buttonsToShow)
        {
            HideButtons();
            foreach (var button in buttonsToShow)
            {
                button.gameObject.SetActive(true);
            }

            ButtonOutline.gameObject.SetActive(true);
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
                NameText.text = string.Format("{0} : {1}", AssetBundleName, FormatSize(operation.GetResult()));
            };
        }

        private string FormatSize(long numBytes)
        {
            if (numBytes < 2)
            {
                return numBytes + " B";
            }

            string[] units = {"B", "KB", "MB", "GB", "TB", "PB", "EB"};
            var unitIndex = (int) Math.Floor(Math.Log10(numBytes)) / 3;
            double shiftedBytes = numBytes / Math.Pow(1000, unitIndex);
            return string.Format("{0:0.##} {1}", shiftedBytes, units[unitIndex]);
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