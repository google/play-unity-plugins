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

using System.Xml.Linq;
using Google.Android.AppBundle.Editor.Internal;
using Google.Android.AppBundle.Editor.Internal.AndroidManifest;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using UnityEditor;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal.AndroidManifest
{
    /// <summary>
    /// Configures an asset pack AndroidManifest to be used by an instant app.
    /// </summary>
    [InitializeOnLoad]
    public class PlayInstantAssetPackManifestTransformer : IAssetPackManifestTransformer
    {
        private PlayInstantBuildHelper _buildHelper;
        private bool _initialized;
        private bool _isInstant;

        static PlayInstantAssetPackManifestTransformer()
        {
            AssetPackManifestTransformerRegistry.Registry.RegisterConstructor(
                () => new PlayInstantAssetPackManifestTransformer());
        }

        public PlayInstantAssetPackManifestTransformer()
        {
            _buildHelper = new PlayInstantBuildHelper(false);
        }

        public bool Initialize(BuildToolLogger buildToolLogger)
        {
            _initialized = true;
            // Cache IsInstantBuildType because IsInstantBuildType can only be called on the main thread.
            _isInstant = PlayInstantBuildSettings.IsInstantBuildType();
            return _buildHelper.Initialize(buildToolLogger);
        }

        public string Transform(XDocument document)
        {
            if (!_initialized)
            {
                throw new BuildToolNotInitializedException(this);
            }

            if (!_isInstant)
            {
                return null;
            }

            if (PlayInstantBuildConfig.PlayGamesEnabled)
            {
                return
                    "\n\nAsset packs aren't compatible with full Instant play games. As a workaround, deselect the " +
                    "\"Full Instant play game\" checkbox and contact the Instant play team via " +
                    "http://g.co/play/instant";
            }

            var error = AndroidManifestHelper.ConvertAssetPackManifestToInstant(document);
            if (!string.IsNullOrEmpty(error))
            {
                return "Asset Module AndroidManifest could not be converted to instant: " + error;
            }

            return null;
        }
    }
}