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
using System.IO;
using System.Linq;
using Google.Android.AppBundle.Editor.Internal.AssetPacks;
using UnityEditor;

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// Provides helper methods for building AssetBundles with various texture compression formats.
    /// </summary>
    public static class AssetBundleBuilder
    {
        /// <summary>
        /// Run one or more AssetBundle builds with the specified texture compression formats.
        /// Notes about the <see cref="outputPath"/> parameter:
        /// - If a relative path is provided, the file paths in the returned AssetPackConfig will be relative paths.
        /// - If an absolute path is provided, the file paths in the returned object will be absolute paths.
        /// - AssetBundle builds for additional texture formats will be created in siblings of this directory. For
        ///   example, for outputDirectory "a/b/c" and texture format ASTC, there will be a directory "a/b/c#tcf_astc".
        /// - If allowClearDirectory is false, this directory and any sibling directories must be empty or not exist,
        ///   otherwise an exception is thrown.
        /// </summary>
        /// <param name="outputPath">The output directory for the ETC1 AssetBundles. See other notes above.</param>
        /// <param name="builds">The main argument to <see cref="BuildPipeline"/>.</param>
        /// <param name="deliveryMode">A delivery mode to apply to every asset pack in the generated config.</param>
        /// <param name="additionalTextureFormats">Texture formats to build for in addition to ETC1.</param>
        /// <param name="assetBundleOptions">Options to pass to <see cref="BuildPipeline"/>.</param>
        /// <param name="allowClearDirectory">Allows this method to clear the contents of the output directory.</param>
        /// <returns>An <see cref="AssetPackConfig"/> containing file paths to all generated AssetBundles.</returns>
        public static AssetPackConfig BuildAssetBundles(
            string outputPath,
            AssetBundleBuild[] builds,
            AssetPackDeliveryMode deliveryMode,
            IEnumerable<MobileTextureSubtarget> additionalTextureFormats = null,
            BuildAssetBundleOptions assetBundleOptions = BuildAssetBundleOptions.UncompressedAssetBundle,
            bool allowClearDirectory = false)
        {
            var nameToTextureFormatToPath = BuildAssetBundles(outputPath, builds, assetBundleOptions,
                MobileTextureSubtarget.ETC, additionalTextureFormats, allowClearDirectory);
            var assetPackConfig = new AssetPackConfig();
            foreach (var compressionToPath in nameToTextureFormatToPath.Values)
            {
                assetPackConfig.AddAssetBundles(compressionToPath, deliveryMode);
            }

            return assetPackConfig;
        }

        /// <summary>
        /// Run one or more AssetBundle builds with the specified texture compression formats.
        /// This variant allows for overriding the base format and provides a different return type.
        /// </summary>
        /// <param name="outputPath">The output directory for base AssetBundles. See the other method for notes.</param>
        /// <param name="builds">The main argument to <see cref="BuildPipeline"/>.</param>
        /// <param name="assetBundleOptions">Options to pass to <see cref="BuildPipeline"/>.</param>
        /// <param name="baseTextureFormat">The default format. Note: ETC1 is supported by all devices.</param>
        /// <param name="additionalTextureFormats">Texture formats to generate for in addition to the base.</param>
        /// <param name="allowClearDirectory">Allows this method to clear the contents of the output directory.</param>
        /// <returns>A dictionary from AssetBundle name to TextureCompressionFormat to file outputPath.</returns>
        public static Dictionary<string, Dictionary<TextureCompressionFormat, string>> BuildAssetBundles(
            string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions,
            MobileTextureSubtarget baseTextureFormat, IEnumerable<MobileTextureSubtarget> additionalTextureFormats,
            bool allowClearDirectory)
        {
            if (builds == null || builds.Length == 0)
            {
                throw new ArgumentException("AssetBundleBuild parameter cannot be null or empty");
            }

            foreach (var build in builds)
            {
                if (!AndroidAppBundle.IsValidModuleName(build.assetBundleName))
                {
                    throw new ArgumentException("Invalid AssetBundle name: " + build.assetBundleName);
                }
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentNullException("outputPath");
            }

            CheckDirectory(outputPath, allowClearDirectory);

            // Make unique and silently remove the base format, if it was present.
            var textureSubtargets = new HashSet<MobileTextureSubtarget>(
                additionalTextureFormats ?? Enumerable.Empty<MobileTextureSubtarget>());
            textureSubtargets.Remove(baseTextureFormat);

            // Note: GetCompressionFormatSuffix() will throw for unsupported formats.
            var paths = textureSubtargets.Select(format => outputPath + GetCompressionFormatSuffix(format));
            foreach (var path in paths)
            {
                CheckDirectory(path, allowClearDirectory);
            }

            // Throws if the base format is invalid.
            GetCompressionFormat(baseTextureFormat);

            return BuildAssetBundlesInternal(
                outputPath, builds, assetBundleOptions, baseTextureFormat, textureSubtargets);
        }

        // The internal method assumes all parameter preconditions have already been checked.
        private static Dictionary<string, Dictionary<TextureCompressionFormat, string>> BuildAssetBundlesInternal(
            string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions,
            MobileTextureSubtarget baseTextureFormat, IEnumerable<MobileTextureSubtarget> textureSubtargets)
        {
            // Use a dictionary to capture the generated AssetBundles' file names.
            var nameToTextureFormatToPath = builds.ToDictionary(
                build => build.assetBundleName, _ => new Dictionary<TextureCompressionFormat, string>());

            var originalAndroidBuildSubtarget = EditorUserBuildSettings.androidBuildSubtarget;
            try
            {
                // First build for the additional texture formats.
                foreach (var textureSubtarget in textureSubtargets)
                {
                    // Build AssetBundles in the the base format's directory.
                    EditorUserBuildSettings.androidBuildSubtarget = textureSubtarget;
                    BuildAssetBundles(outputPath, builds, assetBundleOptions);

                    // Then move the files to a new directory with the compression format suffix.
                    var outputPathWithSuffix = outputPath + GetCompressionFormatSuffix(textureSubtarget);
                    Directory.Move(outputPath, outputPathWithSuffix);

                    var textureCompressionFormat = GetCompressionFormat(textureSubtarget);
                    UpdateDictionary(nameToTextureFormatToPath, outputPathWithSuffix, textureCompressionFormat);
                }

                // Then build for the base format.
                EditorUserBuildSettings.androidBuildSubtarget = baseTextureFormat;
                BuildAssetBundles(outputPath, builds, assetBundleOptions);

                UpdateDictionary(nameToTextureFormatToPath, outputPath, TextureCompressionFormat.Default);
                return nameToTextureFormatToPath;
            }
            finally
            {
                EditorUserBuildSettings.androidBuildSubtarget = originalAndroidBuildSubtarget;
            }
        }

        private static void BuildAssetBundles(
            string outputPath, AssetBundleBuild[] builds, BuildAssetBundleOptions assetBundleOptions)
        {
            // The output directory must exist before calling BuildAssetBundles(), so create it first.
            Directory.CreateDirectory(outputPath);
            var assetPackManifest =
                BuildPipeline.BuildAssetBundles(outputPath, builds, assetBundleOptions, BuildTarget.Android);
            if (assetPackManifest == null)
            {
                throw new InvalidOperationException("Error building AssetBundles");
            }
        }

        private static void UpdateDictionary(
            Dictionary<string, Dictionary<TextureCompressionFormat, string>> nameToTextureFormatToPath,
            string outputPath, TextureCompressionFormat textureCompressionFormat)
        {
            foreach (var entry in nameToTextureFormatToPath)
            {
                var filePath = Path.Combine(outputPath, entry.Key);
                entry.Value[textureCompressionFormat] = filePath;
                if (!File.Exists(filePath))
                {
                    throw new InvalidOperationException(string.Format("Missing AssetBundle file: " + filePath));
                }
            }
        }

        private static string GetCompressionFormatSuffix(MobileTextureSubtarget subtarget)
        {
            var format = GetCompressionFormat(subtarget);
            return TextureTargetingTools.GetTargetingSuffix(format);
        }

        private static TextureCompressionFormat GetCompressionFormat(MobileTextureSubtarget subtarget)
        {
            switch (subtarget)
            {
                case MobileTextureSubtarget.ASTC:
                    return TextureCompressionFormat.Astc;
                case MobileTextureSubtarget.DXT:
                    return TextureCompressionFormat.Dxt1;
                case MobileTextureSubtarget.ETC:
                    return TextureCompressionFormat.Etc1;
                case MobileTextureSubtarget.ETC2:
                    return TextureCompressionFormat.Etc2;
                case MobileTextureSubtarget.PVRTC:
                    return TextureCompressionFormat.Pvrtc;
                default:
                    throw new ArgumentException("Unsupported MobileTextureSubtarget: " + subtarget);
            }
        }

        private static void CheckDirectory(string path, bool allowClearDirectory)
        {
            var directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
            {
                return;
            }

            if (!allowClearDirectory && directoryInfo.GetFileSystemInfos().Any())
            {
                throw new ArgumentException("Directory is not empty: " + path);
            }

            directoryInfo.Delete( /* recursive= */ true);
        }
    }
}