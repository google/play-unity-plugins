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

using System.Collections.Generic;

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Provides methods for validating whether an <see cref="AssetBundlePack"/> can be included in an app bundle.
    /// </summary>
    public interface IAssetPackValidator
    {
        /// <summary>
        /// Returns a list of errors that would prevent the specified asset pack from being included in an app bundle.
        /// </summary>
        IList<AssetPackError> CheckBuildErrors(AssetBundlePack assetBundlePack);
    }
}