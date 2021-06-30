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

namespace Google.Android.AppBundle.Editor.Internal
{
    // Options related to creating Android App Bundles.
    public class CreateBundleOptions
    {
        /// <summary>
        /// Specifies the file path for the AAB.
        /// </summary>
        public string AabFilePath{ get; set; }

        /// <summary>
        /// Returns the AssetPackConfig to use for the build, or null.
        /// </summary>
        public AssetPackConfig AssetPackConfig { get; set; }

        /// <summary>
        /// Options for overriding the default file compression strategy.
        /// </summary>
        public CompressionOptions CompressionOptions { get; set; }

    }
}