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
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo
{
    /// <summary>
    /// Provides controls for downloading a batch of asset packs
    /// </summary>
    public class AssetPackBatchDownloader : MonoBehaviour
    {
        [Serializable]
        public class AssetWithinPack
        {
            public string AssetPackName;
            public string AssetPath;
            public Button DisplayContentsButton;
        }

        public List<AssetWithinPack> Assets;
        public Button RetrieveAssetPackBatchButton;
        public Button RemovePacksButton;
        public DownloadDisplay Display;
        public Text DisplayTextBox;

        private PlayAssetPackBatchRequest _batchRequest;
        private bool _requestInProgress;

        public void Start()
        {
            RetrieveAssetPackBatchButton.onClick.AddListener(ButtonEventRetrieveAssetPackBatch);
            RemovePacksButton.onClick.AddListener(ButtonEventRemovePacks);

            Display.BindButton(RetrieveAssetPackBatchButton, AssetDeliveryStatus.Failed);
            Display.BindButton(RemovePacksButton, AssetDeliveryStatus.Available);

            foreach (var assetData in Assets)
            {
                var assetDataInClosure = assetData;
                assetData.DisplayContentsButton.onClick.AddListener(
                    () => ButtonEventDisplayTextAsset(assetDataInClosure));
                Display.BindButton(assetData.DisplayContentsButton, AssetDeliveryStatus.Available);
            }

            Display.BindColor(Display.SuccessColor, AssetDeliveryStatus.Available);
            Display.BindColor(Display.ErrorColor, AssetDeliveryStatus.Failed);

            SetInitialStatus();
        }

        /// <summary>
        /// Start retrieving the asset packs specified in <see cref="Assets"/>.
        /// </summary>
        public void ButtonEventRetrieveAssetPackBatch()
        {
            StartCoroutine(CoLoadAssetPackBatch());
        }

        /// <summary>
        /// Remove all asset packs specified in <see cref="Assets"/>.
        /// </summary>
        public void ButtonEventRemovePacks()
        {
            // Defensive check. In practice this should never occur because the remove button is only shown once the
            // batch request is initialized.
            if (_batchRequest == null)
            {
                return;
            }

            foreach (var asset in Assets)
            {
                PlayAssetDelivery.RemoveAssetPack(asset.AssetPackName);
            }

            SetInitialStatus();
        }

        /// <summary>
        /// Display the contents of the specified asset as text.
        /// </summary>
        /// <param name="asset">The AssetWithinPack object describing where to find the asset.</param>
        public void ButtonEventDisplayTextAsset(AssetWithinPack asset)
        {
            // Defensive check. In practice this should never occur because the display buttons are hidden until the
            // batch request is complete.
            if (_batchRequest == null)
            {
                return;
            }

            var request = _batchRequest.Requests[asset.AssetPackName];
            var assetLocation = request.GetAssetLocation(asset.AssetPath);
            if (assetLocation == null)
            {
                Debug.LogError("Asset cannot be found");
                return;
            }

            var assetFileStream = File.OpenRead(assetLocation.Path);
            var buffer = new byte[assetLocation.Size];
            assetFileStream.Seek((long) assetLocation.Offset, SeekOrigin.Begin);
            assetFileStream.Read(buffer, /* offset= */ 0, buffer.Length);
            var fileContents = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            DisplayTextBox.text = string.Format("Contents of file {0} in asset pack {1}: {2}",
                asset.AssetPath, asset.AssetPackName, fileContents);
        }

        private void Update()
        {
            Display.SetScrolling(_requestInProgress);
        }

        private void SetInitialStatus()
        {
            Display.ShowButtons(RetrieveAssetPackBatchButton);

            var allDownloaded = true;
            foreach (var asset in Assets)
            {
                if (!PlayAssetDelivery.IsDownloaded(asset.AssetPackName))
                {
                    allDownloaded = false;
                    break;
                }
            }

            Display.SetInitialStatus(allDownloaded);
        }

        private IEnumerator CoLoadAssetPackBatch()
        {
            var assetPackNames = Assets.Select(a => a.AssetPackName).Distinct().ToList();
            _batchRequest = PlayAssetDelivery.RetrieveAssetPackBatchAsync(assetPackNames);
            _requestInProgress = true;

            while (!_batchRequest.IsDone)
            {
                var totalProgress = 0f;
                foreach (var request in _batchRequest.Requests.Values)
                {
                    totalProgress += request.DownloadProgress;
                }

                Display.SetProgress(totalProgress / _batchRequest.Requests.Count);
                Display.SetStatus(AssetDeliveryStatus.Retrieving, AssetDeliveryErrorCode.NoError);
                yield return null;
            }

            var allSucceeded = true;
            foreach (var request in _batchRequest.Requests.Values)
            {
                if (request.Error != AssetDeliveryErrorCode.NoError)
                {
                    Display.SetStatus(request.Status, request.Error);
                    allSucceeded = false;
                    break;
                }
            }

            if (allSucceeded)
            {
                Display.SetProgress(1f);
                Display.SetStatus(AssetDeliveryStatus.Available, AssetDeliveryErrorCode.NoError);
            }

            _requestInProgress = false;
        }
    }
}