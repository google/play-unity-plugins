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
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AppUpdate.Internal
{
    /// <summary>
    /// Proxy for Play Core's InstallStateUpdatedListener class.
    /// Used to receive callbacks from Play Core's AppUpdateManager.
    /// </summary>
    internal class AppUpdateStateListener : AndroidJavaProxy
    {
        public event Action<AppUpdateState> OnStateUpdateEvent = delegate { };

        public AppUpdateStateListener() :
            base(PlayCoreConstants.PlayCorePackagePrefix + "install.InstallStateUpdatedListener")
        {
        }

        // Proxied java calls. Method names are camelCase to match the corresponding java methods.
        public void onStateUpdate(AndroidJavaObject installState)
        {
            var updateState = new AppUpdateState(installState);
            PlayCoreEventHandler.HandleEvent(() => OnStateUpdateEvent.Invoke(updateState));
        }
    }
}