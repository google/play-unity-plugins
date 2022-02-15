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

using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.Integrity
{
    /// <summary>
    /// Response for <see cref="IntegrityManager.RequestIntegrityToken"/>.
    /// </summary>
    public class IntegrityTokenResponse
    {
        /// <summary>
        /// A token which contains the Base64 encoded response for the integrity related enquiries. The response
        /// consists of a JSON payload that is signed and encrypted as a nested JSON Web Token (JWT).
        /// </summary>
        public string Token { get; private set; }

        internal IntegrityTokenResponse(AndroidJavaObject tokenResponse)
        {
            using (tokenResponse)
            {
                var javaTokenString = tokenResponse.Call<AndroidJavaObject>("token");
                Token = PlayCoreHelper.ConvertJavaString(javaTokenString);
            }
        }
    }
}