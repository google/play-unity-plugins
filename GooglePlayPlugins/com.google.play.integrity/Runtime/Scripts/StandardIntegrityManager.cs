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

using Google.Play.Common;
using Google.Play.Core.Internal;
using Google.Play.Integrity.Internal;
using UnityEngine;

namespace Google.Play.Integrity
{
    /// <summary>
    /// Manages requests for integrity information.
    /// </summary>
    public class StandardIntegrityManager
    {
        private readonly PlayCoreStandardIntegrityManager _playCoreStandardIntegrityManager;

        private const string PrepareIntegrityTokenRequestClassName =
            PlayCoreConstants.IntegrityPackagePrefix + "StandardIntegrityManager$PrepareIntegrityTokenRequest";

        /// <summary>
        ///  Constructor.
        /// </summary>
        public StandardIntegrityManager()
        {
            _playCoreStandardIntegrityManager = new PlayCoreStandardIntegrityManager();
        }

        /// <summary>
        /// Prepares the integrity token and makes it available for requesting via
        /// <see cref="StandardIntegrityTokenProvider"/>.
        ///
        /// <para>You can call this method from time to time in order to refresh the resulting
        /// <see cref="StandardIntegrityTokenProvider"/>.</para>
        ///
        /// <para>The API makes a call to Google servers and hence requires a network connection.</para>
        ///
        /// <para>Note that the API is in beta mode.</para>
        /// </summary>
        /// <param name="request">the object to prepare the integrity token with.</param>
        /// <returns>
        /// A <see cref="PlayAsyncOperation{StandardIntegrityTokenProvider, StandardIntegrityErrorCode}"/> that returns
        /// <see cref="StandardIntegrityTokenProvider"/> on successful callback or
        /// <see cref="StandardIntegrityErrorCode"/> on failure callback.
        /// </returns>
        public PlayAsyncOperation<StandardIntegrityTokenProvider, StandardIntegrityErrorCode> PrepareIntegrityToken(
            PrepareIntegrityTokenRequest request)
        {
            var operation = new StandardIntegrityAsyncOperation<StandardIntegrityTokenProvider>();

            using (var prepareIntegrityTokenRequestClass = new AndroidJavaClass(PrepareIntegrityTokenRequestClassName))
            using (var prepareIntegrityTokenRequestBuilder =
                   prepareIntegrityTokenRequestClass.CallStatic<AndroidJavaObject>("builder"))
            {
                prepareIntegrityTokenRequestBuilder.Call<AndroidJavaObject>("setCloudProjectNumber",
                    request.CloudProjectNumber);
                var javaPrepareIntegrityTokenRequest =
                    prepareIntegrityTokenRequestBuilder.Call<AndroidJavaObject>("build");

                var prepareIntegrityTokenTask =
                    _playCoreStandardIntegrityManager.PrepareIntegrityToken(javaPrepareIntegrityTokenRequest);

                prepareIntegrityTokenTask.RegisterOnSuccessCallback(tokenProvider =>
                {
                    operation.SetResult(
                        new StandardIntegrityTokenProvider(new PlayCoreStandardIntegrityTokenProvider(tokenProvider)));
                    prepareIntegrityTokenTask.Dispose();
                    javaPrepareIntegrityTokenRequest.Dispose();
                });
                prepareIntegrityTokenTask.RegisterOnFailureCallback((reason, errorCode) =>
                {
                    operation.SetError(PlayCoreTranslator.TranslatePlayCoreStandardIntegrityErrorCode(errorCode));
                    prepareIntegrityTokenTask.Dispose();
                    javaPrepareIntegrityTokenRequest.Dispose();
                });
            }

            return operation;
        }
    }
}