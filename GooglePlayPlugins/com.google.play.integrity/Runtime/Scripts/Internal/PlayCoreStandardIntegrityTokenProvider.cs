// Copyright 2023 Google LLC
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

namespace Google.Play.Integrity.Internal
{
    /// <summary>
    /// Internal implementation of StandardIntegrityTokenProvider that wraps Play Core's StandardIntegrityTokenProvider.
    /// </summary>
    internal class PlayCoreStandardIntegrityTokenProvider : IDisposable
    {
        private readonly AndroidJavaObject _javaStandardIntegrityTokenProvider;

        internal PlayCoreStandardIntegrityTokenProvider(AndroidJavaObject javaTokenProvider)
        {
            _javaStandardIntegrityTokenProvider = javaTokenProvider;
        }

        internal PlayServicesTask<AndroidJavaObject> Request(AndroidJavaObject standardIntegrityTokenRequest)
        {
            var javaTask =
                _javaStandardIntegrityTokenProvider.Call<AndroidJavaObject>("request",
                    standardIntegrityTokenRequest);
            return new PlayServicesTask<AndroidJavaObject>(javaTask);
        }

        public void Dispose()
        {
            _javaStandardIntegrityTokenProvider.Dispose();
        }
    }
}