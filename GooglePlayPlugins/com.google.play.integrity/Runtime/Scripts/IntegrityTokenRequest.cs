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
        ///
        /// <p>It must be base64 encoded in web-safe no-wrap form.
        ///
        /// <p>See https://developer.android.com/google/play/integrity/verdict#nonce for details about
        /// the nonce requirements and recommendations.
        /// </summary>
        public string Nonce { get; private set; }

        /// <summary>
        /// A cloud project number to link to the integrity token.
        /// <p>This field is required for <a
        /// href="https://developer.android.com/google/play/integrity/setup#apps-exclusively-distributed-outside-google-play">
        /// apps exclusively distributed outside of Google Play</a> and <a
        /// href="https://developer.android.com/google/play/integrity/setup#sdks">SDKs</a>. For apps
        /// distributed on Google Play, the cloud project number is configured in the Play Console and
        /// need not be set on the request.
        ///
        /// <p>Cloud project number can be found in Project info in your Google Cloud Console for the
        /// cloud project where Play Integrity API is enabled.
        ///
        /// <p>Calls to <a
        /// href="https://developer.android.com/google/play/integrity/verdict#decrypt-verify-google-servers">
        /// decrypt the token on Google's server</a> must be authenticated using the cloud account that
        /// was linked to the token in this request.
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
