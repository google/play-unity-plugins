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

using UnityEngine;
using UnityEngine.UI;

namespace Google.Play.Common.LoadingScreen
{
    /// <summary>
    /// A UI component that scrolls a tiled texture horizontally.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(RectTransform))]
    public class ScrollingFillAnimator : MonoBehaviour
    {
        /// <summary>
        /// How fast the fill texture will scroll in units per second.
        /// </summary>
        [Tooltip("How fast the fill texture will scroll in units per second.")]
        public float ScrollSpeed = 2.5f;

        private RawImage _image;
        private Vector2 _textureOffset;
        private RectTransform _rectTransform;

        private void Update()
        {
            LazyInit();

            var uvRect = _image.uvRect;
            uvRect = ScrollUvs(uvRect);
            uvRect = ScaleUvs(uvRect);
            _image.uvRect = uvRect;
        }

        // Move the uvs to the left so that the texture appears to scroll to the right.
        private Rect ScrollUvs(Rect uvRect)
        {
            // Don't scroll the uvs in edit mode, because Update is called too intermittently to be a useful preview.
            if (Application.isPlaying)
            {
                uvRect.x -= ScrollSpeed * Time.deltaTime;
            }
            return uvRect;
        }

        // Scale the uv rect so that the texture tiles across its rect instead of scaling with it.
        private Rect ScaleUvs(Rect uvRect)
        {
            if (_image.texture == null)
            {
                return uvRect;
            }

            uvRect.size = new Vector2(_rectTransform.rect.width / _image.texture.width,
                _rectTransform.rect.height / _image.texture.height);
            return uvRect;
        }

        private void LazyInit()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            if (_image == null)
            {
                _image = GetComponent<RawImage>();
            }
        }
    }
}