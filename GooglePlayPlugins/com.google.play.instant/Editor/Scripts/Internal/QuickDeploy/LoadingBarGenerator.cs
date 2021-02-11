// Copyright 2018 Google LLC
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

using Google.Play.Common.LoadingScreen;
using UnityEngine;
using UnityEngine.UI;

namespace Google.Play.Instant.Editor.Internal.QuickDeploy
{
    /// <summary>
    /// Class that encapsulates the creation of the LoadingBar component
    /// </summary>
    public static class LoadingBarGenerator
    {
        // Loading bar size in pixels.
        private const float LoadingBarWidth = 672.0f;
        private const float LoadingBarHeight = 54.0f;

        // Loading bar placement as a proportion of the screen size relative to the bottom left corner. Adjust if needed.
        private const float LoadingBarPositionX = 0.5f;
        private const float LoadingBarPositionY = 0.3984375f; // Corresponds to a position of 765 on 1080x1920 devices.

        // Names for the gameobject components.
        private const string ContainerName = "Loading Elements";
        private const string BarName = "Loading Bar";
        private const string OutlineName = "Outline";
        private const string BackgroundName = "Background";
        private const string FillName = "Fill";
        private const string ScrollingFillName = "Scrolling Fill";
        private const string ProgressName = "Progress";

        // Color of the inner fill and outline of the loading bar.
        private static readonly Color DarkGrey = new Color32(74, 74, 74, 255);

        public static RectTransform GenerateLoadingContainer()
        {
            var containerObject = GenerateUiObject(ContainerName);

            var containerRectTransform = containerObject.GetComponent<RectTransform>();
            containerRectTransform.anchorMin = new Vector2(LoadingBarPositionX, LoadingBarPositionY);
            containerRectTransform.anchorMax = new Vector2(LoadingBarPositionX, LoadingBarPositionY);
            containerRectTransform.sizeDelta = new Vector2(LoadingBarWidth, LoadingBarHeight);

            return containerRectTransform;
        }

        public static LoadingBar GenerateLoadingBar()
        {
            var progressHolderObject = GenerateUiObject(ProgressName);

            var loadingBarObject = GenerateUiObject(BarName);

            var loadingBar = loadingBarObject.AddComponent<LoadingBar>();
            loadingBar.Outline = GenerateImage(loadingBarObject, OutlineName, DarkGrey);
            loadingBar.Background = GenerateImage(loadingBarObject, BackgroundName, Color.white);
            loadingBar.ProgressFill = GenerateImage(progressHolderObject, FillName, DarkGrey);

            loadingBar.ProgressHolder = progressHolderObject.GetComponent<RectTransform>();
            loadingBar.ProgressHolder.transform.SetParent(loadingBar.transform, false);
            SetAnchorsToScaleWithParent(loadingBar.ProgressHolder);

            var scrollingFillRectTransform = GenerateScrollingFill();
            scrollingFillRectTransform.transform.SetParent(loadingBar.ProgressHolder, false);
            scrollingFillRectTransform.SetAsFirstSibling(); // The scrolling fill should be behind the progress fill.

            var loadingBarRectTransform = loadingBarObject.GetComponent<RectTransform>();
            SetAnchorsToScaleWithParent(loadingBarRectTransform);

            return loadingBar;
        }

        private static RectTransform GenerateScrollingFill()
        {
            var scrollingFillObject = GenerateUiObject(ScrollingFillName);
            var scrollingFillImage = scrollingFillObject.AddComponent<RawImage>();
            scrollingFillImage.texture = FindLoadingTileTexture();

            scrollingFillObject.AddComponent<ScrollingFillAnimator>();

            return scrollingFillImage.GetComponent<RectTransform>();
        }

        private static RectTransform GenerateImage(GameObject parent, string name, Color color)
        {
            var imageObject = GenerateUiObject(name);
            imageObject.transform.SetParent(parent.transform, false);

            var image = imageObject.AddComponent<Image>();
            image.color = color;

            return imageObject.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Create a new GameObject with the specified name and a RectTransform instead of a normal Transform.
        /// </summary>
        private static GameObject GenerateUiObject(string name)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rectTransform = gameObject.GetComponent<RectTransform>();
            SetAnchorsToScaleWithParent(rectTransform);

            return gameObject;
        }

        private static void SetAnchorsToScaleWithParent(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
        }

        // Visible for testing.
        public static Texture2D FindLoadingTileTexture()
        {
            return LoadingScreenGenerator.FindAssetByFilter<Texture2D>("GooglePlayInstantLoadingTile t:texture2d");
        }
    }
}