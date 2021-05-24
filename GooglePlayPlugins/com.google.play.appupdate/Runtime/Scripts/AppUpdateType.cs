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
    /// Methods for performing the in-app update flow.
    /// Note: regardless of the method, the app needs to be restarted to install an update.
    /// </summary>
    public enum AppUpdateType
    {
        /// <summary>
        /// Flexible update flow, where the user can still use the app while the update is downloaded.
        /// </summary>
        Flexible = 0,

        /// <summary>
        /// Immediate update flow, where the user is unable to interact with the app
        /// while the update is downloaded and installed.
        /// </summary>
        Immediate = 1
    }
}