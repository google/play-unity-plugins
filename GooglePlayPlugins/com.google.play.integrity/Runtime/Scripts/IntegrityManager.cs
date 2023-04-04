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

using Google.Play.Common;
using Google.Play.Core.Internal;
using Google.Play.Integrity.Internal;
using UnityEngine;

namespace Google.Play.Integrity
{
    /// <summary>
    /// Manages requests for integrity information.
    /// </summary>
    public class IntegrityManager
    {
        private readonly PlayCoreIntegrityManager _playCoreIntegrityManager;

        private const string IntegrityTokenRequestClassName =
            PlayCoreConstants.IntegrityPackagePrefix + "IntegrityTokenRequest";

        /// <summary>
        /// Constructor.
        /// </summary>
        public IntegrityManager()
        {
            _playCoreIntegrityManager = new PlayCoreIntegrityManager();
        }

        /// <summary>
        /// Starts a PlayAsyncOperation to generate a token for integrity-related enquiries, and provides the token as
        /// it's result.
        ///
        /// <p>The JSON payload is signed and encrypted as a nested JSON Web Token (JWT), that is
        /// <a href="https://tools.ietf.org/html/rfc7516">JWE</a> of
        /// <a href="https://tools.ietf.org/html/rfc7515">JWS</a>.
        ///
        /// <p>JWE uses <a href="https://tools.ietf.org/html/rfc7518#section-4.4">A256KW</a> as a key wrapping
        /// algorithm and <a href="https://tools.ietf.org/html/rfc7518#section-5.3">A256GCM</a> as a content encryption
        /// algorithm. JWS uses <a href="https://tools.ietf.org/html/rfc7518#section-3.4">ES256</a> as a signing
        /// algorithm.
        ///
        /// <p>All decryption and verification should be done within a secure server environment. Do not decrypt or
        /// verify the received token from within the client app. In particular, never expose any decryption keys to the
        /// client app.
        ///
        /// <p>See https://developer.android.com/google/play/integrity/verdict#token-format.
        /// </summary>
        /// <returns>
        /// A <see cref="PlayAsyncOperation{IntegrityTokenResponse, IntegrityErrorCode}"/> that returns
        /// <see cref="IntegrityTokenResponse"/> on successful callback or <see cref="IntegrityErrorCode"/> on failure
        /// callback.
        /// </returns>
        public PlayAsyncOperation<IntegrityTokenResponse, IntegrityErrorCode> RequestIntegrityToken(
            IntegrityTokenRequest integrityTokenRequest)
        {
            var operation = new IntegrityAsyncOperation<IntegrityTokenResponse>();

            using (var integrityTokenRequestClass = new AndroidJavaClass(IntegrityTokenRequestClassName))
            using (var integrityTokenRequestBuilder =
                   integrityTokenRequestClass.CallStatic<AndroidJavaObject>("builder"))
            {
                integrityTokenRequestBuilder.Call<AndroidJavaObject>("setNonce", integrityTokenRequest.Nonce);

                if (integrityTokenRequest.CloudProjectNumber.HasValue)
                {
                    integrityTokenRequestBuilder.Call<AndroidJavaObject>(
                        "setCloudProjectNumber", integrityTokenRequest.CloudProjectNumber.Value);
                }

                var javaIntegrityTokenRequest = integrityTokenRequestBuilder.Call<AndroidJavaObject>("build");

                var requestIntegrityTokenTask =
                    _playCoreIntegrityManager.RequestIntegrityToken(javaIntegrityTokenRequest);

                requestIntegrityTokenTask.RegisterOnSuccessCallback(tokenResponse =>
                {
                    operation.SetResult(new IntegrityTokenResponse(tokenResponse));
                    requestIntegrityTokenTask.Dispose();
                    javaIntegrityTokenRequest.Dispose();
                });
                requestIntegrityTokenTask.RegisterOnFailureCallback((reason, errorCode) =>
                {
                    operation.SetError(PlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                    requestIntegrityTokenTask.Dispose();
                    javaIntegrityTokenRequest.Dispose();
                });
            }

            return operation;
        }
    }
}