// Copyright 2020 Google LLC
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

namespace Google.Play.AppUpdate
{
    /// <summary>
    /// Availability information for an in-app update.
    /// </summary>
    public enum UpdateAvailability
    {
        /// <summary>
        /// Update availability is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// No updates are available.
        /// </summary>
        UpdateNotAvailable = 1,

        /// <summary>
        /// An update is available.
        /// </summary>
        UpdateAvailable = 2,

        /// <summary>
        /// An update has been triggered by the developer and is in progress.
        /// </summary>
        DeveloperTriggeredUpdateInProgress = 3
    }
}