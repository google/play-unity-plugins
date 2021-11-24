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
using UnityEngine;
using UnityEngine.UI;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo
{
    /// <summary>
    /// Provides functionality to page through a list of GameObjects.
    /// </summary>
    public class DemoPager : MonoBehaviour
    {
        [Serializable]
        public class Page
        {
            public GameObject PageRoot;
            public List<GameObject> DownloaderObjects;
            public Text OutputText;
        }

        public Page[] Pages;
        public Button NextButton;
        public Button PreviousButton;
        private int _currentIndex;
        private readonly Vector3 _visiblePosition = Vector3.zero;

        // Arbitrarily far away position;
        private readonly Vector3 _offscreenPosition = Vector3.right * 3000f;

        public void Start()
        {
            NextButton.onClick.AddListener(ButtonEventNext);
            PreviousButton.onClick.AddListener(ButtonEventPrevious);

            foreach (var page in Pages)
            {
                page.PageRoot.SetActive(true);
                page.PageRoot.transform.localPosition = _offscreenPosition;
            }

            SetPage(_currentIndex);
        }

        public void SetPage(int pageIndex)
        {
            Pages[_currentIndex].PageRoot.transform.localPosition = _offscreenPosition;

            _currentIndex = pageIndex;
            Pages[_currentIndex].PageRoot.transform.localPosition = _visiblePosition;
        }

        public Page GetCurrentPage()
        {
            return Pages[_currentIndex];
        }

        private void ButtonEventNext()
        {
            SetPage((_currentIndex + 1) % Pages.Length);
        }

        private void ButtonEventPrevious()
        {
            SetPage((Pages.Length + _currentIndex - 1) % Pages.Length);
        }
    }
}