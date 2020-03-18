// Copyright 2018 Google LLC
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

namespace Google.Play.Instant
{
    /// <summary>
    /// Methods and constants that are specific to Google Play Instant and/or the plugin.
    /// </summary>
    public static class GooglePlayInstantUtils
    {
        /// <summary>
        /// Return true if this is an instant app build, false if an installed app build.
        /// This is an alternative to checking "#if PLAY_INSTANT" directly.
        /// Note: Do not call this in an Editor script. Prefer PlayBuildConfiguration.IsInstantBuildType() instead.
        /// </summary>
        public static bool IsInstantApp()
        {
#if PLAY_INSTANT
            return true;
#else
            return false;
#endif
        }
    }
}