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

namespace Google.Play.Integrity
{
    /// <summary>
    /// Request for <see cref="StandardIntegrityTokenProvider.Request"/>.
    /// </summary>
    public class StandardIntegrityTokenRequest
    {
        /// <summary>
        /// The request hash provided to the API.
        /// <para>It is a recommended and not required field.</para>
        /// </summary>
        public String RequestHash { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="requestHash">A request hash to bind the integrity token to.</param>
        public StandardIntegrityTokenRequest(String requestHash = null)
        {
            RequestHash = requestHash;
        }
    }
}