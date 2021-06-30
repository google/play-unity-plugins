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

using Google.Android.AppBundle.Editor.Internal;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using UnityEditor;
#if UNITY_2018_4_OR_NEWER && !NET_LEGACY
using System.Threading.Tasks;

#endif

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Helper to build <a href="https://developer.android.com/platform/technology/app-bundle/">Android App Bundle</a>
    /// files suitable for publishing on <a href="https://play.google.com/console/">Google Play Console</a>.
    /// </summary>
    public static class Bundletool
    {
#if UNITY_2018_4_OR_NEWER && !NET_LEGACY
        /// <summary>
        /// Builds an Android App Bundle using the specified build options.
        ///
        /// Before calling this method use PlayInstantBuildSettings.SetInstantBuildType() to decide whether to build an
        /// instant app bundle or a regular app bundle for installed apps.
        ///
        /// This method returns an async Task because it partially runs on a background thread (except in Batch Mode).
        /// As with other methods that return async tasks, the result can be awaited on or accessed via ContinueWith().
        /// </summary>
        /// <param name="androidBuildOptions">Configuration options for the build.</param>
        /// <returns>An async task that provides an AndroidBuildReport.</returns>
        /// <exception cref="AndroidBuildException">
        /// Thrown in case of certain build failures. Includes an AndroidBuildReport when thrown.
        /// </exception>
        public static async Task<AndroidBuildReport> BuildBundle(AndroidBuildOptions androidBuildOptions)
        {
            return await AppBundlePublisher.BuildTask(androidBuildOptions);
        }

#endif

        /// <summary>
        /// Builds an Android App Bundle using the specified build options.
        ///
        /// Before calling this method use PlayInstantBuildSettings.SetInstantBuildType() to decide whether to build an
        /// instant app bundle or a regular app bundle for installed apps.
        ///
        /// If this method is invoked in Batch Mode, for example from a command line build, then the entire build will
        /// run on the main thread. In this case the <see cref="forceSynchronousBuild"/> parameter has no effect.
        ///
        /// If this method is invoked by a script running from an interactive Editor UI, some of the build will run on
        /// a background thread and this method will return before the full build is complete. This behavior can be
        /// overridden by setting the <see cref="forceSynchronousBuild"/> parameter to true; in this case the entire
        /// build will run on the main thread. Note that this freezes the Editor UI.
        ///
        /// The asynchronous BuildBundle() method method should be preferred when building on Unity 2018.4 or higher.
        /// </summary>
        /// <param name="buildPlayerOptions">A Unity BuildPlayerOptions including the output file path.</param>
        /// <param name="assetPackConfig">The asset packs to include in the Android App Bundle, if any.</param>
        /// <param name="forceSynchronousBuild">If true, the build should only run on the main thread.</param>
        /// <returns>
        /// True if the build succeeded or began running in the background, false if it failed or was cancelled.
        /// </returns>
        public static bool BuildBundle(
            BuildPlayerOptions buildPlayerOptions,
            AssetPackConfig assetPackConfig = null,
            bool forceSynchronousBuild = false)
        {
            return AppBundlePublisher.Build(buildPlayerOptions, assetPackConfig, forceSynchronousBuild);
        }

        /// <summary>
        /// Builds an APK Set (.apks) file from the specified Android App Bundle file (.aab).
        /// </summary>
        /// <param name="aabFilePath">An .aab file.</param>
        /// <param name="apksFilePath">An .apks output ZIP file containing APK files.</param>
        /// <param name="buildMode">The type of APKs to build from the Android App Bundle.</param>
        /// <param name="enableLocalTesting">
        /// If true, enables a testing mode where fast-follow and on-demand asset packs are fetched from local storage
        /// rather than downloaded.
        /// </param>
        /// <returns>An error message if there was a problem running bundletool, or null if successful.</returns>
        public static string BuildApks(
            string aabFilePath,
            string apksFilePath,
            BundletoolBuildMode buildMode = BundletoolBuildMode.Default,
            bool enableLocalTesting = false)
        {
            var bundletoolHelper = new BundletoolHelper(new JavaUtils());
            if (bundletoolHelper.Initialize(new BuildToolLogger()))
            {
                return bundletoolHelper.BuildApkSet(aabFilePath, apksFilePath, buildMode, enableLocalTesting);
            }

            return "Failed to initialize bundletool.";
        }
    }
}