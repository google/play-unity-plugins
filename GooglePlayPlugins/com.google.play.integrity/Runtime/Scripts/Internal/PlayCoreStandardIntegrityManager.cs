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
using Google.Play.Common;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.Integrity.Internal
{
    /// <summary>
    /// Internal implementation of StandardIntegrityManager that wraps Play Core's StandardIntegrityManager.
    /// </summary>
    internal class PlayCoreStandardIntegrityManager : IDisposable
    {
        private readonly AndroidJavaObject _javaStandardIntegrityManager;

        internal PlayCoreStandardIntegrityManager()
        {
            const string factoryClassName =
                PlayCoreConstants.IntegrityPackagePrefix + "IntegrityManagerFactory";

            using (var activity = UnityPlayerHelper.GetCurrentActivity())
            using (var integrityManagerFactory = new AndroidJavaClass(factoryClassName))
            {
                _javaStandardIntegrityManager =
                    integrityManagerFactory.CallStatic<AndroidJavaObject>("createStandard", activity);
            }

            if (_javaStandardIntegrityManager == null)
            {
                throw new NullReferenceException("Play Core returned null StandardIntegrityManager");
            }

            PlayCoreEventHandler.CreateInScene();
        }

        /// <summary>
        /// Returns a <see cref="PlayServicesTask{TAndroidJava}" /> which returns an IntegrityTokenResponse
        /// AndroidJavaObject on the registered on success callback.
        /// </summary>
        /// <param name="prepareIntegrityTokenRequest">The PrepareIntegrityTokenRequest AndroidJavaObject.</param>
        internal PlayServicesTask<AndroidJavaObject> PrepareIntegrityToken(
            AndroidJavaObject prepareIntegrityTokenRequest)
        {
            var javaTask =
                _javaStandardIntegrityManager.Call<AndroidJavaObject>("prepareIntegrityToken",
                    prepareIntegrityTokenRequest);
            return new PlayServicesTask<AndroidJavaObject>(javaTask);
        }


        public void Dispose()
        {
            _javaStandardIntegrityManager.Dispose();
        }
    }
}