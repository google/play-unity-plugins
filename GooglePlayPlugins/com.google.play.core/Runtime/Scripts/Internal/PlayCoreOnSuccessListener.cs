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
    /// Proxy for Play Core's OnSuccessListener class.
    /// Allows C# classes to be alerted when a Play Core Task succeeds, and to get the result of that Task.
    /// </summary>
    /// <typeparam name="TAndroidJava">
    /// The return type of the proxied Play Core Task.
    /// Must be a primitive type (e.g. bool, int, or float) or an AndroidJavaObject.
    /// </typeparam>
    public class PlayCoreOnSuccessListener<TAndroidJava> : AndroidJavaProxy
    {
        /// <summary>
        /// Triggers when the associated Play Core Task succeeds.
        /// </summary>
        public event Action<TAndroidJava> OnTaskSucceeded = delegate { };

        public PlayCoreOnSuccessListener() : base(PlayCoreConstants.PlayCorePackagePrefix + "tasks.OnSuccessListener")
        {
        }

        // Proxied java calls. Method names are camelCase to match the corresponding java methods.
        public void onSuccess(TAndroidJava result)
        {
            PlayCoreEventHandler.HandleEvent(() => OnTaskSucceeded.Invoke(result));
        }
    }
}