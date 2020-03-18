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

namespace Google.Play.Core.Internal
{
    /// <summary>
    /// Mirror of a subset of Activity constants of
    /// <a href="https://developer.android.com/reference/android/app/Activity.html#constants_2"></a> and
    /// <a href="https://developer.android.com/reference/com/google/android/play/core/install/model/ActivityResult"></a>.
    /// </summary>
    public static class ActivityResult
    {
        /// <summary>
        /// Operation succeeded.
        /// </summary>
        public const int ResultOk = -1;

        /// <summary>
        /// Operation cancelled.
        /// </summary>
        public const int ResultCancelled = 0;

        /// <summary>
        /// Operation for In-app update failed.
        /// </summary>
        public const int ResultFailed = 1;
    }
}