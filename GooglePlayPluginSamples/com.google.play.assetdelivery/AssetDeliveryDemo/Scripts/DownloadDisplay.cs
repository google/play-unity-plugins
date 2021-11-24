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
using System.Collections.Generic;
using System.Linq;
using Google.Play.Common.LoadingScreen;
using UnityEngine;
using UnityEngine.UI;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo
{
    /// <summary>
    /// Provides controls and status displays for downloading AssetBundles via Play Asset Delivery
    /// </summary>
    public class DownloadDisplay : MonoBehaviour
    {
        public Text StatusText;
        public Text NameText;
        public Text RetrieveButtonText;
        public Image ColorTint;
        public LoadingBar LoadingBar;
        public ScrollingFillAnimator ScrollingFill;
        public RectTransform ButtonOutline;
        public Color ErrorColor;
        public Color SuccessColor;
        public Color NeutralColor;

        private readonly IDictionary<AssetDeliveryStatus, List<Button>> _buttonsByStatus =
            new Dictionary<AssetDeliveryStatus, List<Button>>();

        private readonly HashSet<Button> _allButtons = new HashSet<Button>();

        private readonly IDictionary<AssetDeliveryStatus, Color> _colorsByStatus =
            new Dictionary<AssetDeliveryStatus, Color>();

        private const float ActiveScrollSpeed = 2.5f;

        /// <summary>
        /// Configure the display so that a button appears when the status changes.
        /// </summary>
        public void BindButton(Button button, AssetDeliveryStatus status)
        {
            List<Button> buttonsForStatus;
            if (!_buttonsByStatus.TryGetValue(status, out buttonsForStatus))
            {
                buttonsForStatus = new List<Button>();
                _buttonsByStatus.Add(status, buttonsForStatus);
            }

            buttonsForStatus.Add(button);
            _allButtons.Add(button);
        }

        /// <summary>
        /// Configure the display so that the specified button is hidden when ShowButton is called with a different
        /// button.
        /// </summary>
        public void BindButton(Button button)
        {
            _allButtons.Add(button);
        }

        /// <summary>
        /// Configure the display so that the loading bar color changes when the status is set to the specified one.
        /// </summary>
        public void BindColor(Color color, AssetDeliveryStatus status)
        {
            _colorsByStatus.Add(status, color);
        }

        public void SetScrolling(bool scrolling)
        {
            ScrollingFill.ScrollSpeed = scrolling ? ActiveScrollSpeed : 0f;
        }

        public void SetInitialStatus(bool isDownloaded)
        {
            ColorTint.color = NeutralColor;
            LoadingBar.SetProgress(0f);
            StatusText.text = isDownloaded
                ? AssetDeliveryStatus.Available.ToString()
                : AssetDeliveryStatus.Pending.ToString();
        }

        public void SetStatus(AssetDeliveryStatus status, AssetDeliveryErrorCode error)
        {
            StatusText.text = status.ToString();

            if (_buttonsByStatus.ContainsKey(status))
            {
                ShowButtons(_buttonsByStatus[status].ToArray());
            }
            else
            {
                HideButtons();
                ColorTint.color = NeutralColor;
            }

            if (_colorsByStatus.ContainsKey(status))
            {
                ColorTint.color = _colorsByStatus[status];
            }

            if (error != AssetDeliveryErrorCode.NoError)
            {
                StatusText.text = string.Format("{0}: {1}", status.ToString(), error.ToString());
                RetrieveButtonText.text = "Try Again";
            }
        }

        public void SetProgress(float progress)
        {
            LoadingBar.Progress = progress;
        }

        public void HideButtons()
        {
            foreach (var button in _allButtons)
            {
                button.gameObject.SetActive(false);
            }

            ButtonOutline.gameObject.SetActive(false);
        }

        public void ShowButtons(params Button[] buttonsToShow)
        {
            HideButtons();
            foreach (var button in buttonsToShow)
            {
                button.gameObject.SetActive(true);
            }

            ButtonOutline.gameObject.SetActive(true);
        }

        public void SetNameText(string nameText)
        {
            NameText.text = nameText;
        }

        public string FormatSize(long numBytes)
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
    }
}