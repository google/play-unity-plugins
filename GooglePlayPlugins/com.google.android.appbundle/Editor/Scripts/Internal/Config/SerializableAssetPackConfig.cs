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

using System;
using System.Collections.Generic;

namespace Google.Android.AppBundle.Editor.Internal.Config
{
    [Serializable]
    public class SerializableAssetPackConfig
    {
        public List<SerializableAssetPack> assetPacks = new List<SerializableAssetPack>();

        public List<SerializableMultiTargetingAssetPack> targetedAssetPacks =
            new List<SerializableMultiTargetingAssetPack>();

        public List<SerializableMultiTargetingAssetBundle> assetBundles =
            new List<SerializableMultiTargetingAssetBundle>();

        public bool splitBaseModuleAssets;

        public string defaultTextureCompressionFormat = TextureCompressionFormat.Default.ToString();

        public TextureCompressionFormat DefaultTextureCompressionFormat
        {
            get { return SerializationHelper.GetTextureCompressionFormat(defaultTextureCompressionFormat); }
            set { defaultTextureCompressionFormat = value.ToString(); }
        }
    }
}