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

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Extension methods for bundletool.
    /// </summary>
    public static class BundletoolExtensions
    {
        /// <summary>
        /// Returns the specified <see cref="BundletoolBuildMode"/> as an acceptable value for the "--mode" flag
        /// of "bundletool build-apks".
        /// </summary>
        public static string GetModeFlag(this BundletoolBuildMode buildMode)
        {
            if (buildMode == BundletoolBuildMode.SystemCompressed)
            {
                return "system_compressed";
            }

            return buildMode.ToString().ToLower();
        }
    }
}