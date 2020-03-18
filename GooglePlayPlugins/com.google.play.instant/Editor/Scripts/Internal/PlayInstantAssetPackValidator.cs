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
using Google.Android.AppBundle.Editor;
using Google.Android.AppBundle.Editor.Internal.AssetPacks;
using UnityEditor;

namespace Google.Play.Instant.Editor.Internal
{
    [InitializeOnLoad]
    public class PlayInstantAssetPackValidator : IAssetPackValidator
    {
        static PlayInstantAssetPackValidator()
        {
            AssetPackValidatorRegistry.Registry.RegisterConstructor(() => new PlayInstantAssetPackValidator());
        }

        public IList<AssetPackError> CheckBuildErrors(AssetBundlePack assetBundlePack)
        {
            var isInstantBuildType = PlayInstantBuildSettings.IsInstantBuildType();
            if (isInstantBuildType && assetBundlePack.DeliveryMode == AssetPackDeliveryMode.InstallTime)
            {
                return new List<AssetPackError> {AssetPackError.InstallTimeAndInstant};
            }

            if (isInstantBuildType && assetBundlePack.DeliveryMode == AssetPackDeliveryMode.FastFollow)
            {
                return new List<AssetPackError> {AssetPackError.FastFollowAndInstant};
            }

            return new List<AssetPackError>();
        }
    }
}