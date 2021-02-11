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

using UnityEngine;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo
{
    [RequireComponent(typeof(Camera))]
    public class LookAround : MonoBehaviour
    {
        private Vector3 _currentLocalPoint;
        private Vector3 _previousLocalPoint;
        private Camera _camera;

        private const float BigNumber = 1000000f;

        private void Start()
        {
            _camera = GetComponent<Camera>();
        }

        public void Update()
        {
            HandleInput();

            var to = transform.position - _previousLocalPoint;
            var from = transform.position - _currentLocalPoint;
            var rotation = Quaternion.FromToRotation(from, to);
            transform.rotation *= rotation;
        }

        // Handles both touchscreen and mouse controls.
        private void HandleInput()
        {
            if (!IsDragging()) return;

            _previousLocalPoint = _currentLocalPoint;
            var screenPoint = Input.mousePosition + Vector3.forward * BigNumber;
            _currentLocalPoint = transform.worldToLocalMatrix * _camera.ScreenToWorldPoint(screenPoint);

            if (DidDraggingBegin())
            {
                _previousLocalPoint = _currentLocalPoint;
            }
        }

        private bool IsDragging()
        {
            return Input.GetMouseButton(0) || Input.touchCount > 0;
        }

        private bool DidDraggingBegin()
        {
            var didTouchBegin = Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began;
            return Input.GetMouseButtonDown(0) || didTouchBegin;
        }
    }
}