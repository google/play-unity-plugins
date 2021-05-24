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

namespace Google.Play.Core.Internal
{
    /// <summary>
    /// A collection of constants used for interfacing with the Play Core library.
    /// </summary>
    public static class PlayCoreConstants
    {
        public const string PlayCorePackagePrefix = "com.google.android.play.core.";
        public const string AssetPackPackagePrefix = PlayCorePackagePrefix + "assetpacks.";
        public const int InternalErrorCode = -100;
    }
}