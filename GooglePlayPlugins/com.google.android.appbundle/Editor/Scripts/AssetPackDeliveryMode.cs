// Copyright 2019 Google LLC
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

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Indicates whether an asset pack will be included in an Android App Bundle, and if so how it is delivered.
    /// </summary>
    public enum AssetPackDeliveryMode
    {
        /// <summary>
        /// Do not include the corresponding asset pack in the Android App Bundle.
        /// </summary>
        DoNotPackage = 0,

        /// <summary>
        /// Deliver the corresponding asset pack when the app is installed.
        /// </summary>
        InstallTime = 1,

        /// <summary>
        /// Deliver the corresponding asset pack immediately after the app is installed and potentially before it is
        /// opened.
        /// </summary>
        FastFollow = 2,

        /// <summary>
        /// Only deliver the corresponding asset pack when it is explicitly requested by the app.
        /// </summary>
        OnDemand = 3
    }
}