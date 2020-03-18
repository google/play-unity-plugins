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
    /// Describes various possible error states of an AssetPack.
    /// </summary>
    public enum AssetPackError
    {
        [NameAndDescription("Duplicate Name",
            "AssetBundles with the same name and texture format exist in different folders. An " +
            "AssetBundle's name and parent folder texture format must be unique.")]
        DuplicateName,

        [NameAndDescription("Invalid Name",
            "A Play-delivered AssetBundle's name must start with an English letter and can only contain letters, " +
            "numbers, and underscores. Please regenerate the AssetBundle with a new name.")]
        InvalidName,

        [NameAndDescription("Missing File", "An AssetBundle file with this name is missing from the folder.")]
        FileMissing,

        [NameAndDescription("Dependency Error", "One or more of this AssetBundle's dependencies has an error.")]
        DependencyError,

        [NameAndDescription("Missing Dependency", "One or more of this AssetBundle's dependencies is missing.")]
        DependencyMissing,

        [NameAndDescription("Dependency Not Packaged",
            "This AssetBundle is marked for delivery, but one or more of its dependencies is not.")]
        DependencyNotPackaged,

        [NameAndDescription("Incompatible Dependency",
            "This AssetBundle is marked to be delivered earlier than one or more of its dependencies.")]
        DependencyIncompatibleDelivery,

        [NameAndDescription("Instant Incompatible",
            "Install-time asset packs aren't supported for instant apps.")]
        InstallTimeAndInstant,

        [NameAndDescription("Instant Incompatible",
            "Fast-follow asset packs aren't supported for instant apps.")]
        FastFollowAndInstant,
    }
}
