// Copyright 2021 Google LLC
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
using Google.Play.Common;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.Integrity.Internal
{
    /// <summary>
    /// Internal implementation of IntegrityManager that wraps Play Core's IntegrityManager.
    /// </summary>
    internal class PlayCoreIntegrityManager : IDisposable
    {
        private readonly AndroidJavaObject _javaIntegrityManager;

        internal PlayCoreIntegrityManager()
        {
            const string factoryClassName =
                PlayCoreConstants.PlayCorePackagePrefix + "integrity.IntegrityManagerFactory";

            using (var activity = UnityPlayerHelper.GetCurrentActivity())
            using (var integrityManagerFactory = new AndroidJavaClass(factoryClassName))
            {
                _javaIntegrityManager = integrityManagerFactory.CallStatic<AndroidJavaObject>("create", activity);
            }

            if (_javaIntegrityManager == null)
            {
                throw new NullReferenceException("Play Core returned null IntegrityManager");
            }

            PlayCoreEventHandler.CreateInScene();
        }

        /// <summary>
        /// Returns a <see cref="PlayServicesTask{TAndroidJava}" /> which returns an IntegrityTokenResponse
        /// AndroidJavaObject on the registered on success callback.
        /// </summary>
        /// <param name="integrityTokenRequest">The IntegrityTokenRequest AndroidJavaObject.</param>
        public PlayServicesTask<AndroidJavaObject> RequestIntegrityToken(AndroidJavaObject integrityTokenRequest)
        {
            var javaTask =
                _javaIntegrityManager.Call<AndroidJavaObject>("requestIntegrityToken", integrityTokenRequest);
            return new PlayServicesTask<AndroidJavaObject>(javaTask);
        }

        public void Dispose()
        {
            _javaIntegrityManager.Dispose();
        }
    }
}
