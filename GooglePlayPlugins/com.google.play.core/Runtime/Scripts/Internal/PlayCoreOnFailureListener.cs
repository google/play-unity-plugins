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
using UnityEngine;

namespace Google.Play.Core.Internal
{
    /// <summary>
    /// Proxy for Play Core's OnFailureListener class.
    /// Allows C# classes to be alerted when a Play Core Task throws an exception.
    /// </summary>
    public class PlayCoreOnFailureListener : PlayCoreJavaProxy
    {
        public event Action<string, int> OnTaskFailed = delegate { };

        public PlayCoreOnFailureListener() : base(PlayCoreConstants.PlayCorePackagePrefix + "tasks.OnFailureListener")
        {
        }

        // Proxied java calls. Method names are camelCase to match the corresponding java methods.
        public void onFailure(AndroidJavaObject exception)
        {
            var message = exception.Call<string>("getMessage");

            int errorCode;
            try
            {
                // If exception is not a TaskException, this call will throw an AndroidJavaException.
                errorCode = exception.Call<int>("getErrorCode");
            }
            catch (AndroidJavaException)
            {
                errorCode = PlayCoreConstants.InternalErrorCode;
            }

            PlayCoreEventHandler.HandleEvent(() => OnTaskFailed.Invoke(message, errorCode));
        }
    }
}