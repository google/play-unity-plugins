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

using System.Collections.Generic;

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Options that override the default Android App Bundle (AAB) compression settings for base APKs and install-time
    /// asset pack APKs. The asset files in on-demand and fast-follow asset packs are always stored uncompressed.
    ///
    /// The default values for these options lead to APKs containing uncompressed files. This generally results in
    /// faster APK downloads, smaller update patches, and straightforward asset loading. These settings should
    /// generally only be changed in advanced use cases when the top priority is reducing on-disk APK size.
    /// </summary>
    public class CompressionOptions
    {
        /// <summary>
        /// A list of file globs for matching files that should be kept uncompressed within the base APK or
        /// install-time asset pack APKs. Matching occurs against the path of files within a generated APK using
        /// forward slash ('/') as a separator, for example "assets/**/*.txt".
        /// </summary>
        public List<string> UncompressedGlobs { get; set; }

        /// <summary>
        /// Indicates the compression strategy for asset files in install-time asset packs. By default (when false),
        /// asset files are stored uncompressed in the generated install-time asset pack APKs. If this setting is
        /// enabled and a file matches one of the <see cref="UncompressedGlobs"/>, the <see cref="UncompressedGlobs"/>
        /// setting takes precedence and the file is stored uncompressed in the generated APK.
        /// </summary>
        public bool CompressInstallTimeAssetPacks { get; set; }

        /// <summary>
        /// Indicates the compression strategy for asset files added to an AAB from the
        /// <a href="https://docs.unity3d.com/Manual/StreamingAssets.html">StreamingAssets folder</a>.
        /// By default (when false), the asset files are stored uncompressed in both the generated base APK and any
        /// install-time asset pack APKs. If this setting is enabled and a file matches one of the
        /// <see cref="UncompressedGlobs"/>, the <see cref="UncompressedGlobs"/> takes
        /// precedence and the file is stored uncompressed in the generated APK.
        /// </summary>
        public bool CompressStreamingAssets { get; set; }
    }
}