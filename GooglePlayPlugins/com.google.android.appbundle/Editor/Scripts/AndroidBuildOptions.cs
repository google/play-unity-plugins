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

using System;
using UnityEditor;

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Options that configure the behavior of an Android build.
    /// </summary>
    public class AndroidBuildOptions
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="buildPlayerOptions">BuildPlayerOptions required for any build.</param>
        /// <exception cref="ArgumentException">Thrown if the BuildPlayerOptions build target isn't Android.</exception>
        public AndroidBuildOptions(BuildPlayerOptions buildPlayerOptions)
        {
            if (buildPlayerOptions.target != BuildTarget.Android)
            {
                throw new ArgumentException("Unexpected non-Android BuildTarget: " + buildPlayerOptions.target);
            }

            if (buildPlayerOptions.targetGroup != BuildTargetGroup.Android)
            {
                throw new ArgumentException(
                    "Unexpected non-Android BuildTargetGroup: " + buildPlayerOptions.targetGroup);
            }

            BuildPlayerOptions = buildPlayerOptions;
            CompressionOptions = new CompressionOptions();
        }

        /// <summary>
        /// The build options passed to BuildPipeline.BuildPlayer(), including scenes and output location.
        /// </summary>
        public BuildPlayerOptions BuildPlayerOptions { get; private set; }

        /// <summary>
        /// Returns the AssetPackConfig to use for the build, or null.
        /// </summary>
        public AssetPackConfig AssetPackConfig { get; set; }

        /// <summary>
        /// Options for overriding the default file compression strategy.
        /// </summary>
        public CompressionOptions CompressionOptions { get; set; }

        /// <summary>
        /// If true, forces the entire build to run on the main thread, potentially freezing the Editor UI during some
        /// build steps. This setting doesn't affect batch mode builds, which always run on the main thread.
        /// </summary>
        public bool ForceSingleThreadedBuild { get; set; }
    }
}