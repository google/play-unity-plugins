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
    /// <summary>
    /// References a TextAsset. By placing this in a scene, we effectively include the TextAsset in the scene.
    /// This allows us to arbitrarily increase the size of a scene AssetBundle by increasing the size of the TextAsset.
    /// </summary>
    public class TextAssetHolder : MonoBehaviour
    {
        public TextAsset HeldTextAsset;
    }
}