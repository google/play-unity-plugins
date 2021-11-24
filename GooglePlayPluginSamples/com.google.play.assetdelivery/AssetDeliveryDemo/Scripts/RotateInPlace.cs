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
    public class RotateInPlace : MonoBehaviour
    {
        public Vector3 Axis;
        public float DegreesPerSecond;

        public void Update()
        {
            transform.Rotate(Axis, Time.deltaTime*DegreesPerSecond);
        }
    }
}