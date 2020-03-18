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

using System;
using System.Collections.Generic;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Provides a C# mirror of
    /// <a href="https://github.com/google/bundletool/blob/master/src/main/proto/config.proto">BundletoolConfig</a>
    /// for use with JsonUtility.
    /// Inner class fields are camelCase to match json style.
    /// </summary>
    public static class BundletoolConfig
    {
        public const string Abi = "ABI";
        public const string Language = "LANGUAGE";
        public const string ScreenDensity = "SCREEN_DENSITY";
        public const string TextureCompressionFormat = "TEXTURE_COMPRESSION_FORMAT";

        [Serializable]
        public class Config
        {
            public Optimizations optimizations = new Optimizations();
            public Compression compression = new Compression();
        }

        [Serializable]
        public class Optimizations
        {
            public SplitsConfig splitsConfig = new SplitsConfig();
            public UncompressNativeLibraries uncompressNativeLibraries = new UncompressNativeLibraries();
            public UncompressDexFiles uncompressDexFiles = new UncompressDexFiles();
            public StandaloneConfig standaloneConfig = new StandaloneConfig();
        }

        [Serializable]
        public class SplitsConfig
        {
            public List<SplitDimension> splitDimension = new List<SplitDimension>();
        }

        [Serializable]
        public class StandaloneConfig
        {
            public List<SplitDimension> splitDimension = new List<SplitDimension>();
            public bool strip64BitLibraries;
        }

        [Serializable]
        public class SplitDimension
        {
            public string value;
            public bool negate;
            public SuffixStripping suffixStripping = new SuffixStripping();
        }

        [Serializable]
        public class Compression
        {
            public List<string> uncompressedGlob = new List<string>();
        }

        [Serializable]
        public class UncompressNativeLibraries
        {
            public bool enabled;
        }

        [Serializable]
        public class UncompressDexFiles
        {
            public bool enabled;
        }

        [Serializable]
        public class SuffixStripping
        {
            public bool enabled;
            public string defaultSuffix;
        }
    }
}