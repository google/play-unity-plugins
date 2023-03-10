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

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Build Options for building asset-only app bundles.
    /// </summary>
    public class AssetOnlyOptions
    {
        /// <summary>
        /// A list of app versions that are eligible to update to this asset-only app bundle.
        /// </summary>
        public IList<long> AppVersions { get; set; }

        /// <summary>
        /// A string uniquely identifying this asset-only app bundle.
        /// </summary>
        public string AssetVersionTag { get; set; }
    }
}