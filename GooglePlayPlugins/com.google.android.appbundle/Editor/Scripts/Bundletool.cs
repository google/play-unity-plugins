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

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Helper to build <a href="https://developer.android.com/platform/technology/app-bundle/">Android App Bundle</a>
    /// files suitable for publishing on <a href="https://play.google.com/apps/publish/">Google Play Console</a>.
    /// </summary>
    public static class Bundletool
    {
        /// <summary>
        /// Builds an Android App Bundle using the specified build options. Before calling this method use
        /// PlayInstantBuildSettings.SetInstantBuildType() to decide whether to build an instant app bundle or a
        /// regular app bundle for installed apps.
        /// </summary>
        /// <param name="buildPlayerOptions">A Unity BuildPlayerOptions including the output file path.</param>
        /// <param name="assetPackConfig">The asset packs to include in the Android App Bundle, if any.</param>
        /// <returns>True if the build succeeded, false if it failed or was cancelled.</returns>
        public static bool BuildBundle(BuildPlayerOptions buildPlayerOptions, AssetPackConfig assetPackConfig = null)
        {
            return AppBundlePublisher.Build(buildPlayerOptions, assetPackConfig ?? new AssetPackConfig());
        }

        /// <summary>
        /// Builds an APK Set (.apks) file from the specified Android App Bundle file (.aab).
        /// </summary>
        /// <param name="aabFilePath">An .aab file that was created by calling <see cref="BuildBundle"/>.</param>
        /// <param name="apksFilePath">An .apks output ZIP file containing APK files.</param>
        /// <param name="buildMode">The type of APKs to build from the Android App Bundle.</param>
        /// <param name="enableLocalTesting">
        /// If true, enables a testing mode where fast-follow and on-demand asset packs are fetched from storage
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