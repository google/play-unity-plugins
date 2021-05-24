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
using UnityEditor.Build.Reporting;

namespace Google.Android.AppBundle.Editor
{
    // Note: this class exists (versus using BuildReport directly) so that additional build info can be added later.
    /// <summary>
    /// Report containing the results of an Android build.
    /// </summary>
    public class AndroidBuildReport
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buildReport">A BuildPlayer() BuildReport.</param>
        internal AndroidBuildReport(BuildReport buildReport)
        {
            Report = buildReport;
        }

        /// <summary>
        /// The <see cref="BuildReport"/> generated during the creation of the Android Player. Note that since
        /// additional processing occurs after the Player is built, some information in this report may not be useful.
        /// For example, the BuildSummary.outputPath will contain an intermediate output file path, not the final
        /// AAB output file path.
        /// </summary>
        public BuildReport Report { get; }
    }
}
#endif