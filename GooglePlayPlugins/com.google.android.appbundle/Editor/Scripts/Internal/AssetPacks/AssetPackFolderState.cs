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

using Google.Android.AppBundle.Editor.Internal.Utils;

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Describes various possible states of a folder containing AssetBundle files.
    /// </summary>
    public enum AssetPackFolderState
    {
        // Does not need NameAndDescription attribute.
        Ok,

        [NameAndDescription("Folder Missing", "This folder does not exist.")]
        FolderMissing,

        [NameAndDescription("Folder Empty", "This folder contains no files.")]
        FolderEmpty,

        [NameAndDescription("Manifest File Missing",
            "This folder is missing an AssetBundle file that matches the folder name.")]
        ManifestFileMissing,

        [NameAndDescription("Manifest File Load Error",
            "There was an error loading the AssetBundle containing the AssetBundleManifest asset.")]
        ManifestFileLoadError,

        [NameAndDescription("AssetBundleManifest Missing",
            "This folder's AssetBundle manifest file is missing the AssetBundleManifest asset.")]
        ManifestAssetMissing,

        [NameAndDescription("AssetBundleManifest Load Error",
            "There was an error loading the AssetBundleManifest asset.")]
        ManifestAssetLoadError,

        [NameAndDescription("AssetBundle Files Missing", "This folder contains no AssetBundle files.")]
        AssetBundlesMissing,
    }
}