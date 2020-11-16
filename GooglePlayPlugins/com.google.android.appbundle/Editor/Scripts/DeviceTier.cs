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

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// The device tier labels that can be recognized in an asset pack folder name.
    /// </summary>
    public enum DeviceTier
    {
        /// <summary>
        /// No/Unspecified device tier.
        /// </summary>
        None,

        /// <summary>
        /// High device tier.
        /// </summary>
        High,

        /// <summary>
        /// Medium device tier.
        /// </summary>
        Medium,

        /// <summary>
        /// Low device tier.
        /// </summary>
        Low
    }
}