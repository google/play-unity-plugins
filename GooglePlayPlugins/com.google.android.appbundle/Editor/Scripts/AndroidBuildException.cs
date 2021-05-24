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

#if UNITY_2018_4_OR_NEWER && !NET_LEGACY
using System;

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Exception thrown in case of certain Android build failures.
    /// </summary>
    public class AndroidBuildException : Exception
    {
        internal AndroidBuildException(string errorMessage, AndroidBuildReport androidBuildReport) : base(errorMessage)
        {
            AndroidBuildReport = androidBuildReport;
        }

        internal AndroidBuildException(Exception exception, AndroidBuildReport androidBuildReport)
            : base(exception.Message, exception)
        {
            AndroidBuildReport = androidBuildReport;
        }

        /// <summary>
        /// An <see cref="AndroidBuildReport"/> associated with this build failure. Note that the build may fail
        /// after the Android Player is built but before the AAB is ready; in this case the BuildReport will
        /// likely indicate a BuildResult of Succeeded since it's reporting the result of calling BuildPlayer().
        /// </summary>
        public AndroidBuildReport AndroidBuildReport { get; }
    }
}
#endif