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

namespace Google.Play.Integrity
{
    /// <summary>
    /// Request for <see cref="StandardIntegrityManager.PrepareIntegrityToken"/>.
    /// </summary>
    public class PrepareIntegrityTokenRequest
    {
        /// <summary>
        /// A cloud project number to link to the integrity token.
        /// 
        /// <para>This field is required.</para>
        /// 
        /// <para>Cloud project number can be found in Project info in your Google Cloud Console for the cloud project where
        /// Play Integrity API is enabled.</para>
        /// 
        /// <para>Calls to <a href="https://developer.android.com/google/play/integrity/verdict#decrypt-verify-google-servers">
        /// decrypt the token on Google's server</a> must be authenticated using the cloud account that was linked to
        /// the token in this request.</para> 
        /// </summary>
        public long CloudProjectNumber { get; private set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="cloudProjectNumber">A cloud project number to link to the integrity token.</param>
        public PrepareIntegrityTokenRequest(long cloudProjectNumber)
        {
            CloudProjectNumber = cloudProjectNumber;
        }
    }
}