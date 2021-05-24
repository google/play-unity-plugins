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
    /// In-app update API status.
    /// </summary>
    public enum AppUpdateStatus
    {
        /// <summary>
        /// The update status is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Download of an update is pending and will be processed soon.
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Download of an update is in progress.
        /// </summary>
        Downloading = 2,

        /// <summary>
        /// An update has been fully downloaded.
        /// </summary>
        Downloaded = 3,

        /// <summary>
        /// An update is being installed.
        /// </summary>
        Installing = 4,

        /// <summary>
        /// An update has been successfully installed.
        /// </summary>
        Installed = 5,

        /// <summary>
        /// An update has failed.
        /// </summary>
        Failed = 6,

        /// <summary>
        /// An update has been canceled.
        /// </summary>
        Canceled = 7
    }
}