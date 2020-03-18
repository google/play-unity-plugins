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
using UnityEngine;
using UnityEngine.UI;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo
{
    /// <summary>
    /// Provides functionality for the test infrastructure to control the Asset Delivery Demo via key presses.
    /// </summary>
    public class DemoController : MonoBehaviour
    {
        public List<AssetBundleDownloadDisplay> DownloadDisplays;
        public Image SelectionBox;

        private AssetBundleDownloadDisplay _selectedDownloadDisplay;
        private Dictionary<KeyCode, Action> _keyMappings;

        private delegate Button ButtonFromDisplay(AssetBundleDownloadDisplay display);

        private IEnumerator Start()
        {
            // Provide an interface for the test infrastructure to call functions via key presses.
            _keyMappings = new Dictionary<KeyCode, Action>()
            {
                {KeyCode.Alpha0, () => SelectAssetBundle(0)},
                {KeyCode.Alpha1, () => SelectAssetBundle(1)},
                {KeyCode.Alpha2, () => SelectAssetBundle(2)},
                {KeyCode.Alpha3, () => SelectAssetBundle(3)},
                {KeyCode.R, ClickRetrieveAssetBundleButton},
                {KeyCode.L, ClickLoadSceneButton},
                {KeyCode.C, ClickCancelDownloadButton},
                {KeyCode.X, ClickRemoveAssetBundleButton},
                {KeyCode.Q, QueryStatusText}
            };
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

            foreach (var keyMapping in _keyMappings)
            {
                if (Input.GetKeyDown(keyMapping.Key))
                {
                    keyMapping.Value.Invoke();
                }
            }
        }

        private void SelectAssetBundle(int index)
        {
            _selectedDownloadDisplay = DownloadDisplays[index];
            SelectionBox.transform.position = _selectedDownloadDisplay.transform.position;
            SelectionBox.gameObject.SetActive(true);
            Debug.LogFormat("Selected {0}", _selectedDownloadDisplay.AssetBundleName);
        }

        private void ClickRetrieveAssetBundleButton()
        {
            ClickButton(display => display.RetrieveAssetBundleButton, "Retrieve AssetBundle");
        }

        private void ClickLoadSceneButton()
        {
            ClickButton(display => display.LoadSceneButton, "Load Scene");
        }

        private void ClickCancelDownloadButton()
        {
            ClickButton(display => display.CancelDownloadButton, "Cancel Download");
        }

        private void ClickRemoveAssetBundleButton()
        {
            ClickButton(display => display.RemoveButton, "Remove AssetBundle");
        }

        private void ClickButton(ButtonFromDisplay getButtonAction, string buttonName)
        {
            if (_selectedDownloadDisplay == null)
            {
                Debug.LogErrorFormat("Cannot click \"{0}\" button. No AssetBundle selected.", buttonName);
                return;
            }

            var button = getButtonAction(_selectedDownloadDisplay);
            if (!button.isActiveAndEnabled)
            {
                Debug.LogErrorFormat("The \"{0}\" button is currently disabled.", buttonName);
                return;
            }

            button.onClick.Invoke();
        }

        private void QueryStatusText()
        {
            Debug.Log(_selectedDownloadDisplay.StatusText.text);
        }

        private bool AllDisplaysInitialized()
        {
            return DownloadDisplays.TrueForAll((display) => display.IsInitialized);
        }
    }
}