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
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo
{
    /// <summary>
    /// Provides functionality for the test infrastructure to control the Asset Delivery Demo via key presses.
    /// </summary>
    [RequireComponent(typeof(DemoPager))]
    public class DemoController : MonoBehaviour
    {
        private interface IDownloaderInputMapper
        {
            Button GetButton(KeyCode keyCode, GameObject gameObject);
            List<KeyCode> GetKeyCodes();
        }

        private class DownloaderInputMapper<T> : IDownloaderInputMapper where T : MonoBehaviour
        {
            public Dictionary<KeyCode, ButtonFromDownloader<T>> KeyMapping;

            /// <summary>
            /// Gets the button mapped to the specified keyCode.
            /// Returns null if the mapping isn't found or if the specified gameObject doesn't have a component of type
            /// T.
            /// </summary>
            public Button GetButton(KeyCode keyCode, GameObject gameObject)
            {
                var downloader = gameObject.GetComponent<T>();
                if (downloader == null)
                {
                    return null;
                }

                ButtonFromDownloader<T> action;
                if (!KeyMapping.TryGetValue(keyCode, out action))
                {
                    return null;
                }

                return action.Invoke(downloader);
            }

            public List<KeyCode> GetKeyCodes()
            {
                return KeyMapping.Keys.ToList();
            }
        }

        // These downloaders take some time to initialize so we monitor them and notify the test infrastructure when they
        // are finished initializing.
        public List<AssetBundleDownloader> InitializedDownloaders;
        public Image SelectionBox;

        private GameObject _selectedDownloaderObj;
        private Text _selectedOutputText;

        private readonly DownloaderInputMapper<AssetBundleDownloader> _assetBundleKeyMapping =
            new DownloaderInputMapper<AssetBundleDownloader>();

        private readonly DownloaderInputMapper<AssetPackDownloader> _assetPackKeyMapping =
            new DownloaderInputMapper<AssetPackDownloader>();

        private readonly DownloaderInputMapper<AssetPackBatchDownloader> _assetPackBatchKeyMapping =
            new DownloaderInputMapper<AssetPackBatchDownloader>();

        private Dictionary<KeyCode, Action> _navigationKeyMappings;
        private List<KeyCode> _keyCodesMappedToCommands;

        private List<IDownloaderInputMapper> _downloaderInputMappers;
        private DemoPager _demoPager;

        private delegate Button ButtonFromDownloader<in T>(T display);

        private IEnumerator Start()
        {
            _demoPager = GetComponent<DemoPager>();

            // Provide an interface for the test infrastructure to call functions via key presses.
            // Create a mapping from a given numerical key code to a page index and downloader index.
            // e.g. Pressing 5 selects the first (index 0) downloader on the third page (index 2).
            _navigationKeyMappings = new Dictionary<KeyCode, Action>()
            {
                {KeyCode.Alpha0, () => SelectDownloader(0, 0)},
                {KeyCode.Alpha1, () => SelectDownloader(0, 1)},
                {KeyCode.Alpha2, () => SelectDownloader(0, 2)},
                {KeyCode.Alpha3, () => SelectDownloader(1, 0)},
                {KeyCode.Alpha4, () => SelectDownloader(1, 1)},
                {KeyCode.Alpha5, () => SelectDownloader(2, 0)},
                {KeyCode.Q, QueryStatusText},
                {KeyCode.T, QueryOutputText}
            };

            _assetBundleKeyMapping.KeyMapping = new Dictionary<KeyCode, ButtonFromDownloader<AssetBundleDownloader>>()
            {
                {KeyCode.R, downloader => downloader.RetrieveAssetBundleButton},
                {KeyCode.L, downloader => downloader.LoadSceneButton},
                {KeyCode.C, downloader => downloader.CancelDownloadButton},
                {KeyCode.X, downloader => downloader.RemoveButton},
            };

            _assetPackKeyMapping.KeyMapping = new Dictionary<KeyCode, ButtonFromDownloader<AssetPackDownloader>>()
            {
                {KeyCode.R, downloader => downloader.RetrieveAssetBundleButton},
                {KeyCode.L, downloader => downloader.LoadSceneButton},
                {KeyCode.C, downloader => downloader.CancelDownloadButton},
                {KeyCode.X, downloader => downloader.RemoveButton},
                {KeyCode.O, downloader => downloader.LoadAssetBundleButton},
            };

            _assetPackBatchKeyMapping.KeyMapping =
                new Dictionary<KeyCode, ButtonFromDownloader<AssetPackBatchDownloader>>()
                {
                    {KeyCode.R, downloader => downloader.RetrieveAssetPackBatchButton},
                    {KeyCode.X, downloader => downloader.RemovePacksButton},
                    {KeyCode.I, downloader => downloader.Assets[0].DisplayContentsButton},
                    {KeyCode.J, downloader => downloader.Assets[1].DisplayContentsButton},
                    {KeyCode.K, downloader => downloader.Assets[2].DisplayContentsButton},
                };

            _downloaderInputMappers = new List<IDownloaderInputMapper>()
            {
                _assetBundleKeyMapping,
                _assetPackKeyMapping,
                _assetPackBatchKeyMapping
            };

            _keyCodesMappedToCommands =
                _downloaderInputMappers.SelectMany(controller => controller.GetKeyCodes()).Distinct().ToList();

            Debug.Log("Initialized key mappings");

            while (!AllDisplaysInitialized())
            {
                yield return null;
            }

            Debug.Log("Initialized download displays");
        }

        private void Update()
        {
            if (!Input.anyKeyDown)
            {
                return;
            }

            foreach (var keyMapping in _navigationKeyMappings)
            {
                if (Input.GetKeyDown(keyMapping.Key))
                {
                    keyMapping.Value.Invoke();
                }
            }

            foreach (var keyCode in _keyCodesMappedToCommands)
            {
                if (Input.GetKeyDown(keyCode))
                {
                    ProcessDownloaderCommand(keyCode);
                }
            }
        }

        private void ProcessDownloaderCommand(KeyCode keyCode)
        {
            if (_selectedDownloaderObj == null)
            {
                Debug.LogErrorFormat("Cannot click \"{0}\" key. No downloader selected.", keyCode);
                return;
            }

            var matchingButtons = _downloaderInputMappers
                .Select(mapping => mapping.GetButton(keyCode, _selectedDownloaderObj))
                .Where(mapping => mapping != null);

            foreach (var button in matchingButtons)
            {
                ClickButton(button, button.name);
            }
        }

        private void SelectDownloader(int pageIndex, int downloaderIndex)
        {
            _demoPager.SetPage(pageIndex);
            var currentPage = _demoPager.GetCurrentPage();
            _selectedDownloaderObj = currentPage.DownloaderObjects[downloaderIndex];
            _selectedOutputText = currentPage.OutputText;

            SelectionBox.transform.position = _selectedDownloaderObj.transform.position;
            SelectionBox.gameObject.SetActive(true);
            Debug.Log("Selected " + _selectedDownloaderObj.name);
        }

        private void ClickButton(Button button, string buttonName)
        {
            if (!button.isActiveAndEnabled)
            {
                Debug.LogErrorFormat("The \"{0}\" button is currently disabled.", buttonName);
                return;
            }

            button.onClick.Invoke();
        }

        private void QueryStatusText()
        {
            if (_selectedDownloaderObj == null)
            {
                Debug.LogError("Cannot query status because there is no downloader selected.");
                return;
            }

            var display = _selectedDownloaderObj.GetComponent<DownloadDisplay>();
            if (display == null)
            {
                Debug.LogError("Cannot query status because the selected object has no DownloadDisplay.");
                return;
            }

            Debug.Log(display.StatusText.text);
        }

        private void QueryOutputText()
        {
            if (_selectedOutputText == null)
            {
                Debug.LogError("Cannot query output text box because there is no selected text box");
                return;
            }

            Debug.Log(_selectedOutputText.text);
        }

        private bool AllDisplaysInitialized()
        {
            return InitializedDownloaders.TrueForAll((display) => display.IsInitialized);
        }
    }
}