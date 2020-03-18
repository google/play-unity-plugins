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
using System.Text.RegularExpressions;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    /// <summary>
    /// Tools for adding texture compression format targeting to folders of an Android App Bundle.
    /// </summary>
    public static class TextureTargetingTools
    {
        /// <summary>
        /// The key used by bundletool to recognize a folder targeted for a specific texture format
        /// (for example, "tcf" in "textures#tcf_astc").
        /// </summary>
        private const string TextureCompressionFormatTargetingKey = "tcf";

        /// <summary>
        /// Returns the string used to designate a texture compression format in bundletool.
        /// </summary>
        public static string GetBundleToolTextureCompressionFormatName(TextureCompressionFormat format)
        {
            if (format == TextureCompressionFormat.Default)
            {
                return "";
            }

            var name = Enum.GetName(typeof(TextureCompressionFormat), format);
            return name == null ? format.ToString() : name.ToLower();
        }

        /// <summary>
        /// Get the targeting suffix for a given texture compression format, to
        /// be appended to a folder name containing AssetBundles.
        /// </summary>
        public static string GetTargetingSuffix(TextureCompressionFormat format)
        {
            if (format == TextureCompressionFormat.Default)
            {
                return "";
            }

            return string.Format("#{0}_{1}", TextureCompressionFormatTargetingKey,
                GetBundleToolTextureCompressionFormatName(format));
        }

        /// <summary>
        /// Parse the folder name for a texture compression format, if any, and return it as
        /// well as the name of the folder without the targeting.
        /// </summary>
        public static void GetTextureCompressionFormatAndStripSuffix(string folderName,
            out TextureCompressionFormat targeting,
            out string tcfStrippedFolderName)
        {
            targeting = TextureCompressionFormat.Default;
            tcfStrippedFolderName = folderName;

            // Parse the texture compression format used, if any, using the same convention
            // as bundletool for folder suffixes.
            Regex regex = new Regex(@"(?<base>.+?)#tcf_(?<value>.+)");
            GroupCollection matchedGroups = regex.Match(folderName).Groups;

            if (!matchedGroups["base"].Success || !matchedGroups["value"].Success)
            {
                // No texture compression format targeting found.
                return;
            }

            string tcfValue = matchedGroups["value"].Captures[0].Value;
            try
            {
                targeting = (TextureCompressionFormat) Enum.Parse(typeof(TextureCompressionFormat), tcfValue, true);
                tcfStrippedFolderName = matchedGroups["base"].Captures[0].Value;
            }
            catch (ArgumentException)
            {
                Debug.LogWarningFormat(
                    "Ignoring unrecognized texture format \"{0}\" for folder: {1}",
                    tcfValue, folderName);
            }
        }
    }
}