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

namespace Google.Play.Integrity
{
    /// <summary>
    /// Request for <see cref="IntegrityManager.RequestIntegrityToken"/>.
    /// </summary>
    public class IntegrityTokenRequest
    {
        /// <summary>
        /// A nonce to bind the integrity token to.
        /// <p>A nonce is a unique token which is ideally generated on the application backend and bound to the context
        /// (for example, hash of the user id and timestamp). The provided nonce will be a part of the signed response
        /// token, which will allow you to compare it to the original one and hence prevent replay attacks.
        ///
        /// <p>Nonces should always be generated in a secure server environment. Do not generate a nonce from within the
        /// client app.
        ///
        /// <p>It must be Base64 encoded in web-safe no-wrap form.
        /// </summary>
        public string Nonce { get; private set; }

        /// <summary>
        /// A cloud project number to link to the integrity token.
        /// <p>This is an optional field and is meant to be used only for apps not available on Google Play or by SDKs
        /// that include the Play Integrity API.
        ///
        /// <p>Cloud project number is an automatically generated unique identifier for your Google Cloud project. It
        /// can be found in Project info in your Google Cloud Console for the cloud project where Play Integrity API is
        /// enabled.
        /// </summary>
        public long? CloudProjectNumber { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nonce">The nonce encoded as a Base64 web-safe no-wrap string.</param>
        /// <param name="cloudProjectNumber">An optional cloud project number to link to the integrity token.</param>
        public IntegrityTokenRequest(string nonce, long? cloudProjectNumber = null)
        {
            Nonce = nonce;
            CloudProjectNumber = cloudProjectNumber;
        }
    }
}