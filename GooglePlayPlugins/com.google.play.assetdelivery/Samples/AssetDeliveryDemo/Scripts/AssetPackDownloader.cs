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
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo
{
    /// <summary>
    /// Provides controls and status displays for downloading asset packs via Play Asset Delivery
    /// </summary>
    public class AssetPackDownloader : MonoBehaviour
    {
        public string AssetPackName;
        public string AssetBundlePath;
        public string TextAssetPath;
        public Button RetrieveAssetBundleButton;
        public DownloadDisplay Display;
        public Button LoadSceneButton;
        public Button ShowCellularDialogButton;
        public Button CancelDownloadButton;
        public Button RemoveButton;
        public Button LoadAssetBundleButton;
        public Button DisplayTextAssetButton;
        public Text DisplayTextBox;
        public List<AssetPackDownloader> Dependencies;
        public bool ShowSize;

        private AssetBundle _assetBundle;
        private PlayAssetPackRequest _request;
        private bool _requestInProgress;

        public bool IsInitialized { get; private set; }

        public void Start()
        {
            RetrieveAssetBundleButton.onClick.AddListener(ButtonEventRetrieveAssetPack);
            LoadSceneButton.onClick.AddListener(ButtonEventLoadSceneFromAssetBundle);
            ShowCellularDialogButton.onClick.AddListener(ButtonEventShowCellularDialog);
            CancelDownloadButton.onClick.AddListener(ButtonEventCancelDownload);
            RemoveButton.onClick.AddListener(ButtonEventRemoveAssetPack);
            LoadAssetBundleButton.onClick.AddListener(ButtonEventLoadAssetBundle);
            DisplayTextAssetButton.onClick.AddListener(ButtonEventDisplayTextAsset);

            Display.BindButton(CancelDownloadButton, AssetDeliveryStatus.Pending);
            Display.BindButton(CancelDownloadButton, AssetDeliveryStatus.Retrieving);
            Display.BindButton(ShowCellularDialogButton, AssetDeliveryStatus.WaitingForWifi);
            Display.BindButton(RemoveButton, AssetDeliveryStatus.Available);
            Display.BindButton(RetrieveAssetBundleButton, AssetDeliveryStatus.Failed);
            Display.BindButton(LoadSceneButton);

            if (!string.IsNullOrEmpty(AssetBundlePath))
            {
                Display.BindButton(LoadAssetBundleButton, AssetDeliveryStatus.Available);
            }

            if (!string.IsNullOrEmpty(TextAssetPath))
            {
                Display.BindButton(DisplayTextAssetButton, AssetDeliveryStatus.Available);
            }

            Display.BindColor(Display.SuccessColor, AssetDeliveryStatus.Available);
            Display.BindColor(Display.ErrorColor, AssetDeliveryStatus.Failed);

            Display.SetNameText(AssetPackName);
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

        /// <summary>
        /// Start retrieving the asset pack specified by <see cref="AssetPackName"/>.
        /// </summary>
        public void ButtonEventRetrieveAssetPack()
        {
            Display.HideButtons();
            StartCoroutine(DownloadAssetPackCo());
        }

        /// <summary>
        /// Display the contents of the asset located at <see cref="TextAssetPath"/> as text.
        /// </summary>
        public void ButtonEventDisplayTextAsset()
        {
            if (_request == null || _requestInProgress)
            {
                return;
            }

            var assetLocation = _request.GetAssetLocation(TextAssetPath);
            var assetFileStream = File.OpenRead(assetLocation.Path);
            var buffer = new byte[assetLocation.Size];
            assetFileStream.Seek((long) assetLocation.Offset, SeekOrigin.Begin);
            assetFileStream.Read(buffer, /* offset= */ 0, buffer.Length);
            var fileContents = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            DisplayTextBox.text = string.Format("Contents of file {0}: {1}", TextAssetPath, fileContents);
        }

        /// <summary>
        /// Loads the first scene present in the AssetBundle located at <see cref="AssetBundlePath"/>.
        /// </summary>
        public void ButtonEventLoadSceneFromAssetBundle()
        {
            var sceneToLoad = _assetBundle.GetAllScenePaths()[0];
            SceneManager.LoadScene(sceneToLoad);
        }

        /// <summary>
        /// Loads the AssetBundle located at <see cref="AssetBundlePath"/> into memory.
        /// </summary>
        public void ButtonEventLoadAssetBundle()
        {
            // Defensive check. In practice the LoadAssetButton should be hidden when either of these cases are true.
            if (_request == null || _requestInProgress)
            {
                return;
            }

            StartCoroutine(LoadAssetBundleFromRequest());
        }

        /// <summary>
        /// Cancels the current asset pack download.
        /// </summary>
        public void ButtonEventCancelDownload()
        {
            // Defensive check. In practice the CancelDownloadButton should be hidden if the request is null. 
            if (_request == null)
            {
                return;
            }

            _request.AttemptCancel();
        }

        /// <summary>
        /// Removes the current asset pack from disk.
        /// </summary>
        public void ButtonEventRemoveAssetPack()
        {
            var removeOperation = PlayAssetDelivery.RemoveAssetPack(AssetPackName);
            removeOperation.Completed += (operation) =>
            {
                if (operation.Error != AssetDeliveryErrorCode.NoError)
                {
                    Debug.LogErrorFormat("Error removing AssetBundle {0}: {1}", AssetPackName, operation.Error);
                    return;
                }

                SetInitialStatus();
                UnloadAssetBundle();
            };
        }

        /// <summary>
        /// Displays a dialog prompting the user to download the asset pack over cellular data.
        /// </summary>
        public void ButtonEventShowCellularDialog()
        {
            PlayAssetDelivery.ShowCellularDataConfirmation();
        }

        public void Update()
        {
            var dependenciesLoaded = Dependencies.Count == 0 || Dependencies.TrueForAll(dep => dep.IsLoaded());
            LoadSceneButton.interactable = IsLoaded() && dependenciesLoaded;
            Display.SetScrolling(_requestInProgress);
        }

        private void SetInitialStatus()
        {
            Display.SetInitialStatus(PlayAssetDelivery.IsDownloaded(AssetPackName));
            Display.ShowButtons(RetrieveAssetBundleButton);
        }

        private IEnumerator DownloadAssetPackCo()
        {
            _request = PlayAssetDelivery.RetrieveAssetPackAsync(AssetPackName);
            _requestInProgress = true;

            while (!_request.IsDone)
            {
                if (_request.Status == AssetDeliveryStatus.WaitingForWifi)
                {
                    // Wait until user has confirmed or cancelled the dialog.
                    var asyncOperation = PlayAssetDelivery.ShowCellularDataConfirmation();
                    yield return asyncOperation;

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

            Display.SetProgress(_request.DownloadProgress);
            Display.SetStatus(_request.Status, _request.Error);

            if (_request.Error != AssetDeliveryErrorCode.NoError)
            {
                Debug.LogErrorFormat("Couldn't load asset pack: {0}", _request.Error);
                _request = null;
            }

            _requestInProgress = false;
        }

        private IEnumerator LoadAssetBundleFromRequest()
        {
            var assetBundleCreateRequest = _request.LoadAssetBundleAsync(AssetBundlePath);
            yield return assetBundleCreateRequest;

            if (assetBundleCreateRequest.assetBundle == null)
            {
                Debug.LogError("Failed to load AssetBundle from request.");
                yield break;
            }

            _assetBundle = assetBundleCreateRequest.assetBundle;
            Display.ShowButtons(LoadSceneButton, RemoveButton);
        }

        private void DisplayDownloadSize()
        {
            var getSizeOperation = PlayAssetDelivery.GetDownloadSize(AssetPackName);
            getSizeOperation.Completed += (operation) =>
            {
                if (operation.Error != AssetDeliveryErrorCode.NoError)
                {
                    Debug.LogErrorFormat("Error getting download size for {0}: {1}",
                        AssetPackName, operation.Error);
                    return;
                }

                IsInitialized = true;
                var nameWithSize = string.Format("{0} : {1}", AssetPackName, Display.FormatSize(operation.GetResult()));
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