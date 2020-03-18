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
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Proxy for Play Core's AssetPackStateUpdateListener class.
    /// Used to receive callbacks from Play Core's AssetPackManager.
    /// </summary>
    internal class AssetPackStateUpdateListener : PlayCoreJavaProxy
    {
        public event Action<AssetPackState> OnStateUpdateEvent = delegate { };

        public AssetPackStateUpdateListener() :
            base(PlayCoreConstants.AssetPackPackagePrefix + "AssetPackStateUpdateListener")
        {
        }

        // Proxied java calls. Method names are camelCase to match the corresponding java methods.
        public void onStateUpdate(AndroidJavaObject assetPacksState)
        {
            var packState = new AssetPackState(assetPacksState);
            PlayCoreEventHandler.HandleEvent(() => OnStateUpdateEvent.Invoke(packState));
        }
    }
}
