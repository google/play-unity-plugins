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

namespace Google.Play.AssetDelivery
{
    /// <summary>
    /// Download information about a particular asset pack.
    /// </summary>
    public abstract class PlayAssetPackDownloadInfo
    {
        /// <summary>
        /// The asset pack's download size in bytes.
        /// If the asset pack's delivery mode is install-time, then the download size will always be 0.
        /// </summary>
        public long DownloadSize { get; protected set; }

        /// <summary>
        /// An enum describing whether or not a newer version of the asset pack is available for download.
        /// </summary>
        public AssetPackUpdateAvailability UpdateAvailability { get; protected set; }

        /// <summary>
        /// The version tag of the latest available version of the asset pack. This field might match
        /// <see cref="InstalledVersionTag"/>, in which case no update is available.
        ///
        /// <para>
        /// The tag is set by the developer at upload-time and can be used to identify an asset pack version.
        /// </para>
        ///
        /// <para>Empty string if the asset pack does not exist or no information is available.</para>
        /// </summary>
        public string AvailableVersionTag { get; protected set; }

        /// <summary>
        /// Returns the version tag of the currently installed asset pack. If no version tag was set for this
        /// pack version then the app version code will be returned instead.
        ///
        /// <para>
        /// The tag is set by the developer at upload-time and can be used to identify an asset pack version.
        /// </para>
        ///
        /// <para>
        /// Empty string if no version of this pack is currently installed.
        /// </para>
        /// </summary>
        public string InstalledVersionTag { get; protected set; }
    }
}