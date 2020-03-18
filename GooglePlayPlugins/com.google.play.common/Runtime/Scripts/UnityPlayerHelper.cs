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

namespace Google.Play.Common
{
    /// <summary>
    /// Helper methods related to the UnityPlayer Java class.
    /// </summary>
    public static class UnityPlayerHelper
    {
        /// <summary>
        /// Gets the current activity running in Unity. This object should be disposed after use.
        /// </summary>
        /// <returns>A wrapped activity object. The AndroidJavaObject should be disposed.</returns>
        public static AndroidJavaObject GetCurrentActivity()
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
    }
}