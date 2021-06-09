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

using System.Xml.Linq;
using Google.Android.AppBundle.Editor.Internal.AndroidManifest;

namespace Google.Play.Instant.Editor.Internal.AndroidManifest
{
    // TODO: Merge this with ManifestConstants when we increment major version code.
    /// <summary>
    /// String constants used by AndroidManifest.xml files in instant apps.
    /// </summary>
    public static class PlayInstantManifestConstants
    {
        public const string Flavor = "com.google.android.gms.instant.flavor";
        public const string MetaData = "meta-data";

        /// <summary>
        /// Flavor required for an instant app to be featured on the Play Games App.
        /// See: https://developer.android.com/topic/google-play-instant/instant-play-games-checklist
        /// </summary>
        public const int PlayGamesInstantFlavor = 1337;

        public static readonly XName AndroidValueXName = XName.Get("value", ManifestConstants.AndroidNamespaceUrl);
    }
}
