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

namespace Google.Play.AssetDelivery
{
    /// <summary>
    /// Enum indicating the availability of a newer version of a particular asset pack.
    /// </summary>
    public enum AssetPackUpdateAvailability
    {
        /// <summary>
        /// The asset pack update availability is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The asset pack is already at the latest version; there's no update available.
        /// </summary>
        UpdateNotAvailable = 1,

        /// <summary>
        /// The asset pack can be updated to a newer version.
        /// </summary>
        UpdateAvailable = 2
    }
}