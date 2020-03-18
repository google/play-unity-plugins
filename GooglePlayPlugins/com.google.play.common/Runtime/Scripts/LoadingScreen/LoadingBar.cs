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

using System.Collections;
using UnityEngine;

namespace Google.Play.Common.LoadingScreen
{
    /// <summary>
    /// A loading bar that can be used to display download progress to the user.
    /// </summary>
    [ExecuteInEditMode]
    public class LoadingBar : MonoBehaviour
    {
        /// <summary>
        /// The outline width.
        /// </summary>
        public float OutlineWidth = 6f;

        /// <summary>
        /// The inner border width.
        /// </summary>
        public float InnerBorderWidth = 6f;

        /// <summary>
        /// If true, the <see cref="Outline"/> and <see cref="Background"/> RectTransforms will update to match the
        /// outline and border width.
        /// </summary>
        [Tooltip(
            "If true, the Outline and Background RectTransforms will update to match the outline and border width.")]
        public bool ResizeAutomatically = true;

        /// <summary>
        /// AssetBundle download and install progress. The value set in the Editor is ignored at runtime.
        /// </summary>
        [Tooltip("AssetBundle download and install progress. The value set in the Editor is ignored at runtime.")]
        [Range(0f, 1f)]
        public float Progress = 0.25f;

        /// <summary>
        /// The loading bar background.
        /// </summary>
        public RectTransform Background;

        /// <summary>
        /// The loading bar outline.
        /// </summary>
        public RectTransform Outline;

        /// <summary>
        /// The loading bar progress holder.
        /// </summary>
        public RectTransform ProgressHolder;

        /// <summary>
        /// The loading bar progress fill.
        /// </summary>
        public RectTransform ProgressFill;

        /// <summary>
        /// Proportion of the loading bar allocated to AssetBundle download progress.
        /// The remainder of the loading bar is allocated to install progress.
        /// </summary>
        [Tooltip("Proportion of the loading bar allocated to AssetBundle download progress. " +
                 "The remainder of the loading bar is allocated to install progress.")]
        [Range(0f, 1f)]
        public float AssetBundleDownloadToInstallRatio = 0.8f;

        private void Update()
        {
            if (ResizeAutomatically)
            {
                ApplyBorderWidth();
                SetProgress(Progress);
            }
        }

        /// <summary>
        /// Applies the border width to the loading bar.
        /// </summary>
        public void ApplyBorderWidth()
        {
            Outline.anchorMin = Vector3.zero;
            Outline.anchorMax = Vector3.one;
            Outline.sizeDelta = Vector2.one * (OutlineWidth + InnerBorderWidth);

            Background.anchorMin = Vector3.zero;
            Background.anchorMax = Vector3.one;
            Background.sizeDelta = Vector2.one * (InnerBorderWidth);
        }

        /// <summary>
        /// Sets the loading bar progress to the specified value in the range [0,1].
        /// </summary>
        /// <param name="proportionOfLoadingBar">The loading bar's current progress in the range [0,1].</param>
        public void SetProgress(float proportionOfLoadingBar)
        {
            Progress = proportionOfLoadingBar;
            if (ProgressFill != null)
            {
                ProgressFill.anchorMax = new Vector2(proportionOfLoadingBar, ProgressFill.anchorMax.y);
            }
        }

        /// <summary>
        /// Updates a loading bar by the progress made by an asynchronous operation. The bar will interpolate between
        /// <see cref="startingFillProportion"/> and <see cref="endingFillProportion"/> as the operation progresses.
        /// </summary>
        /// <param name="operation">The <see cref="AsyncOperation"/> indicating progress.</param>
        /// <param name="startingFillProportion">The starting position from which to fill.</param>
        /// <param name="endingFillProportion">The ending position to fill to.</param>
        /// <param name="skipFinalUpdate">
        /// If true, the bar will only fill before the operation has finished.
        /// This is useful in cases where an async operation will set its progress to 1, even when it has failed.
        /// </param>
        public IEnumerator FillUntilDone(AsyncOperation operation, float startingFillProportion,
            float endingFillProportion, bool skipFinalUpdate)
        {
            var previousFillProportion = startingFillProportion;
            var isDone = false;
            while (!isDone)
            {
                if (operation.isDone)
                {
                    isDone = true;
                }
                else
                {
                    var fillProportion = Mathf.Lerp(startingFillProportion, endingFillProportion, operation.progress);
                    fillProportion = Mathf.Max(previousFillProportion, fillProportion); // Progress can only increase.
                    SetProgress(fillProportion);
                    previousFillProportion = fillProportion;
                }

                yield return null;
            }

            if (skipFinalUpdate)
            {
                yield break;
            }

            var finalFillProportion = Mathf.Lerp(startingFillProportion, endingFillProportion, operation.progress);
            finalFillProportion = Mathf.Max(previousFillProportion, finalFillProportion);
            SetProgress(finalFillProportion);
        }
    }
}