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
using System.IO;
using System.Linq;
using Google.Android.AppBundle.Editor;
using Google.Android.AppBundle.Editor.AssetPacks;
using Google.Android.AppBundle.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace Google.Play.AssetDelivery.Samples.AssetDeliveryDemo.Editor
{
    /// <summary>
    /// Provides methods to build AssetDeliveryDemo and its AssetBundles.
    /// </summary>
    public static class DemoBuilder
    {
        public const string ControllerExampleAssetBundleName = "controllerexample";
        public const string GalaxyExampleAssetBundleName = "galaxyexample";
        public const string GamepadAssetBundleName = "gamepad";
        public const string AssetFilesPackName = "assetfiles";
        public const string UndeliveredFileName = "undelivered";

        // Guid for "AssetDeliveryDemo.unity"
        public const string DemoSceneGuid = "5e362fc8a1a6e47bdaa08f1a9401d7b1";

        // Guid for "ControllerExample.unity"
        private const string ControllerExampleSceneGuid = "197703ab7b0e34bd095d324eb5fbcce9";

        // Guid for "GalaxyExample.unity"
        private const string GalaxyExampleSceneGuid = "98beed8a25aaf486e81e0fd972eb3812";

        // Guid for "Gamepad.fbx"
        private const string GamepadModelGuid = "8b8cddea4cd84443786ee8ff9edac140";

        // Guid for "UndeliveredFile.txt"
        private const string UndeliveredFileGuid = "118d88092faf7406695edfc430dd9631";

        private const string AssetBundleDirectory = "AssetBundles/AssetDeliveryDemo/";
        private const string AabFilePath = "AssetDeliveryDemo.aab";

        // TODO: Reference AppBundleEditorMenu once we're ready to introduce a new major version.
        private const string MenuDirectory = GoogleEditorMenu.MainMenuName + "/Android App Bundle/Asset Delivery Demo";
        private const int Priority = GoogleEditorMenu.AndroidAppBundlePriority + 3;

        private static readonly Dictionary<string, string> _assetBundleNameToAssetGuid = new Dictionary<string, string>
        {
            {ControllerExampleAssetBundleName, ControllerExampleSceneGuid},
            {GalaxyExampleAssetBundleName, GalaxyExampleSceneGuid},
            {GamepadAssetBundleName, GamepadModelGuid},
            {UndeliveredFileName, UndeliveredFileGuid}
        };

        [MenuItem(MenuDirectory + "/Configure", false, Priority)]
        public static void Configure()
        {
            if (!DisplayDeletionWarning("Configure Asset Delivery Demo"))
            {
                return;
            }

            AssetPackConfigSerializer.SaveConfig(CreateAssetPackConfig());
            Debug.Log(
                "Configured Asset Delivery settings for sample app. " +
                "Select the AssetDeliveryDemo scene in build settings and run \"Google -> Build & Run\" to test the " +
                "Asset Delivery demo.");
        }

        [MenuItem(MenuDirectory + "/Build", false, Priority + 1)]
        public static void Build()
        {
            if (!DisplayDeletionWarning("Build Asset Delivery Demo"))
            {
                return;
            }

            var assetPackConfig = CreateAssetPackConfig();
            var buildPlayerOptions = AndroidBuildHelper.CreateBuildPlayerOptions(AabFilePath);
            buildPlayerOptions.scenes = new[] {AssetDatabase.GUIDToAssetPath(DemoSceneGuid)};
            if (!Bundletool.BuildBundle(buildPlayerOptions, assetPackConfig))
            {
                throw new Exception("Asset Delivery Demo build failed");
            }
        }

        public static AssetPackConfig CreateAssetPackConfig()
        {
            var assetPackConfig = BuildAssetBundles();
            AddAssetFilesPackToConfig(assetPackConfig);
            AddAssetFilesExampleToConfig(assetPackConfig, "batchExample1", "pack contents #1");
            AddAssetFilesExampleToConfig(assetPackConfig, "batchExample2", "pack contents #2");
            AddAssetFilesExampleToConfig(assetPackConfig, "batchExample3", "pack contents #3");
            return assetPackConfig;
        }

        private static bool DisplayDeletionWarning(string dialogTitle)
        {
            var message = string.Format(
                "This operation will:\n(1) Overwrite the Asset Delivery Settings with the settings for the Asset" +
                " Delivery Demo\n(2) Overwrite the contents of the directory {0}\n\nWould you like to proceed with" +
                " this operation?",
                AssetBundleDirectory);
            return EditorUtility.DisplayDialog(dialogTitle, message, "Yes", "No");
        }

        private static AssetPackConfig BuildAssetBundles()
        {
            AssetDatabase.Refresh();

            var builds = _assetBundleNameToAssetGuid.Select(bundleAndGuid => new AssetBundleBuild
            {
                assetBundleName = bundleAndGuid.Key,
                assetNames = new[] {AssetDatabase.GUIDToAssetPath(bundleAndGuid.Value)}
            }).ToArray();

            return AssetBundleBuilder.BuildAssetBundles(AssetBundleDirectory, builds, AssetPackDeliveryMode.OnDemand,
                null, BuildAssetBundleOptions.UncompressedAssetBundle, true);
        }

        private static void AddAssetFilesPackToConfig(AssetPackConfig assetPackConfig)
        {
            var assetsDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            assetsDirectory.Create();
            var subdirectory = assetsDirectory.CreateSubdirectory("subdirectory");

            File.WriteAllLines(Path.Combine(assetsDirectory.FullName, "file.txt"), new[] {"root file"});
            File.WriteAllLines(Path.Combine(subdirectory.FullName, "file.txt"), new[] {"subdirectory file"});

            assetPackConfig.AddAssetsFolder(
                AssetFilesPackName, assetsDirectory.FullName, AssetPackDeliveryMode.OnDemand);
        }

        private static void AddAssetFilesExampleToConfig(AssetPackConfig assetPackConfig, string packName,
            string fileContents)
        {
            var assetsDirectory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            assetsDirectory.Create();

            File.WriteAllLines(Path.Combine(assetsDirectory.FullName, packName + ".txt"), new[] {fileContents});

            assetPackConfig.AddAssetsFolder(
                packName, assetsDirectory.FullName, AssetPackDeliveryMode.OnDemand);
        }
    }
}
