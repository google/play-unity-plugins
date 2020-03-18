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
using Google.Android.AppBundle.Editor.Internal.BuildTools;

namespace Google.Android.AppBundle.Editor.Internal.AndroidManifest
{
    /// <summary>
    /// Provides an interface to modify the AndroidManifest xml documents configured for asset packs.
    /// </summary>
    public interface IAssetPackManifestTransformer : IBuildTool
    {
        /// <summary>
        /// Modifies the specified AndroidManifest xml document. Returns an error message if the operation fails and
        /// null otherwise.
        /// </summary>
        string Transform(XDocument document);
    }
}