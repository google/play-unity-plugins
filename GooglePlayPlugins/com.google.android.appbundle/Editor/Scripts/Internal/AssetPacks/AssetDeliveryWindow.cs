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
using System.Linq;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.AssetPacks
{
    public class AssetDeliveryWindow : EditorWindow
    {
        /// <summary>
        /// Label for the split base APK checkbox. Visible for referencing from error messages.
        /// </summary>
        public const string SeparateAssetsLabel = "Separate Base APK Assets";

        private const string RefreshButtonText = "Refresh";
        private const int WindowMinWidth = 610;
        private const int WindowMinHeight = 300;
        private const int ButtonWidth = 150;
        private const int FieldWidth = 100;

        private AssetDeliveryConfig _assetDeliveryConfig;
        private Vector2 _scrollPosition;

        /// <summary>
        /// The names of asset packs that are shown collapsed.
        /// </summary>
        private HashSet<string> _collapsedAssetPacks;

        /// <summary>
        /// Displays this window, creating it if necessary.
        /// </summary>
        public static void ShowWindow()
        {
            var window = GetWindow(typeof(AssetDeliveryWindow), false, "Asset Delivery");
            window.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
        }

        private void OnGUI()
        {
            if (_assetDeliveryConfig == null)
            {
                _assetDeliveryConfig = AssetDeliveryConfigSerializer.LoadConfig();
                _assetDeliveryConfig.Refresh();
            }

            if (_collapsedAssetPacks == null)
            {
                _collapsedAssetPacks = new HashSet<string>();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Base APK Asset Delivery", EditorStyles.boldLabel);
            RenderDescription(
                "When building an Android App Bundle (AAB), automatically move assets that would normally be " +
                "delivered in the base APK into an install-time asset pack. This option reduces base APK size " +
                "similarly to the \"Split Application Binary\" publishing setting, but it uses Play Asset Delivery " +
                "instead of APK Expansion (OBB) files, so it's compatible with AABs. This option is recommended for " +
                "apps with a large base APK, e.g. over 150 MB.");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(SeparateAssetsLabel, GUILayout.Width(145));
            var pendingSplitBaseModuleAssets = EditorGUILayout.Toggle(_assetDeliveryConfig.SplitBaseModuleAssets);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Asset Pack Configuration", EditorStyles.boldLabel);
            RenderDescription(
                "Add folders that directly contain AssetBundle files, for example the output folder from Unity's " +
                "AssetBundle Browser. All AssetBundles directly contained in the specified folders will be displayed " +
                "below. Update the \"Delivery Mode\" to include an AssetBundle in AAB builds.");
            EditorGUILayout.Space();

            // TODO: Remove this #if once TCF support is available in Play, to make TCF targeting discoverable.
#if SHOW_TCF_IN_UI
            if (_assetDeliveryConfig.Folders.Count != 0 && !_assetDeliveryConfig.HasTextureCompressionFormatTargeting())
            {
                // TODO: Add link to public documentation when available.
                EditorGUILayout.HelpBox(
                    "These AssetBundles don't specify texture compression format targeting. Generate additional " +
                    "AssetBundles into folders ending with #tcf_xxx (e.g. AssetBundles#tcf_astc), and the " +
                    "AssetBundles will be delivered to devices supporting that format.", MessageType.Info);
                EditorGUILayout.Space();
            }
#endif

            EditorGUILayout.BeginHorizontal();

            // Store the changes and defer the actual update at the end to avoid modifications during rendering.
            var refreshAssetDeliveryConfig = false;
            var foldersToRemove = new List<string>();
            var foldersToAdd = new List<string>();

            if (GUILayout.Button("Add Folder...", GUILayout.Width(ButtonWidth)))
            {
                var folderPath = EditorUtility.OpenFolderPanel("Add Folder Containing AssetBundles", null, null);
                if (string.IsNullOrEmpty(folderPath))
                {
                    // Assume cancelled.
                    return;
                }

                if (_assetDeliveryConfig.Folders.ContainsKey(folderPath))
                {
                    var errorMessage =
                        string.Format(
                            "Cannot add a folder that has already been added. Use \"{0}\" to analyze existing folders.",
                            RefreshButtonText);
                    EditorUtility.DisplayDialog("Add Folder Error", errorMessage, WindowUtils.OkButtonText);
                }
                else
                {
                    foldersToAdd.Add(folderPath);
                }
            }

            if (GUILayout.Button(RefreshButtonText, GUILayout.Width(ButtonWidth)))
            {
                refreshAssetDeliveryConfig = true;
            }

            if (GUILayout.Button("App Bundle Summary...", GUILayout.Width(ButtonWidth)))
            {
                _assetDeliveryConfig.Refresh();
                EditorUtility.DisplayDialog("App Bundle Summary", GetPackagingSummary(), WindowUtils.OkButtonText);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // Hide this setting for projects targeting SDK 21+ since it only affects install-time asset pack
            // installation on older devices.
            if (_assetDeliveryConfig.HasTextureCompressionFormatTargeting() &&
                TextureTargetingTools.IsSdkVersionPreLollipop(PlayerSettings.Android.minSdkVersion))
            {
                EditorGUILayout.LabelField("Texture Compression Configuration", EditorStyles.boldLabel);
                EditorGUILayout.Space();

                bool targetingUpdated;
                RenderTextureCompressionFormatTargetingConfiguration(_assetDeliveryConfig, out targetingUpdated);
                refreshAssetDeliveryConfig |= targetingUpdated;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("AssetBundle Folders", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            foreach (var folderPair in _assetDeliveryConfig.Folders)
            {
                var folderPath = folderPair.Key;
                bool removeFolder;
                RenderFolder(folderPath, folderPair.Value, out removeFolder);
                if (removeFolder)
                {
                    foldersToRemove.Add(folderPath);
                }
            }

            if (_assetDeliveryConfig.Folders.Count == 0)
            {
                EditorGUILayout.HelpBox("Add folders that directly contain AssetBundles.", MessageType.None);
                EditorGUILayout.Space();
            }
            else
            {
                EditorGUILayout.LabelField("AssetBundle Configuration", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                foreach (var assetBundlePack in _assetDeliveryConfig.AssetBundlePacks.Values)
                {
                    bool refreshFolder;
                    RenderAssetBundlePack(assetBundlePack, out refreshFolder);
                    refreshAssetDeliveryConfig |= refreshFolder;
                }
            }

            EditorGUILayout.EndScrollView();

            // Defer folder refresh, addition and removal to avoid modification of the collection while iterating.
            var pendingConfigChanges =
                _assetDeliveryConfig.UpdateAndRefreshFolders(refreshAssetDeliveryConfig, foldersToAdd, foldersToRemove);
            pendingConfigChanges |= pendingSplitBaseModuleAssets != _assetDeliveryConfig.SplitBaseModuleAssets;
            if (pendingConfigChanges)
            {
                _assetDeliveryConfig.SplitBaseModuleAssets = pendingSplitBaseModuleAssets;
                AssetDeliveryConfigSerializer.SaveConfig(_assetDeliveryConfig);
            }
        }

        private void RenderAssetBundlePack(AssetBundlePack assetBundlePack, out bool refreshFolder)
        {
            refreshFolder = false;

#if UNITY_2019_1_OR_NEWER
            var showGroupExpanded = !_collapsedAssetPacks.Contains(assetBundlePack.Name);
            var groupExpanded =
                EditorGUILayout.BeginFoldoutHeaderGroup(showGroupExpanded, assetBundlePack.Name);
            if (groupExpanded)
            {
                _collapsedAssetPacks.Remove(assetBundlePack.Name);
                RenderAssetBundlePackContent(assetBundlePack, out refreshFolder);
            }
            else
            {
                _collapsedAssetPacks.Add(assetBundlePack.Name);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
#else
            // Old Unity versions don't support foldable headers. Use a simple label instead.
            EditorGUILayout.LabelField(assetBundlePack.Name, EditorStyles.boldLabel);
            RenderAssetBundlePackContent(assetBundlePack, out refreshFolder);
            EditorGUILayout.Separator();
#endif
        }

        private static void RenderAssetBundlePackContent(AssetBundlePack assetBundlePack, out bool refreshFolder)
        {
            refreshFolder = false;

            EditorGUI.BeginChangeCheck();
            var newDeliveryMode =
                (AssetPackDeliveryMode) EditorGUILayout.EnumPopup("Delivery Mode", assetBundlePack.DeliveryMode,
                    GUILayout.Width(300));
            if (EditorGUI.EndChangeCheck())
            {
                assetBundlePack.DeliveryMode = newDeliveryMode;

                // Don't use a short-circuit evaluation one-liner to ensure EndChangeCheck() is called above.
                refreshFolder = true;
            }

            // Display the direct dependencies of all the contained asset packs.
            var directDependencies = assetBundlePack.DirectDependencies;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Dependencies");
            GUILayout.TextArea(
                directDependencies.Count > 0 ? string.Join(", ", directDependencies.ToArray()) : "None");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("AssetBundles");
            EditorGUI.indentLevel++;
            foreach (var variant in assetBundlePack.Variants)
            {
                RenderVariant(variant.Value, variant.Key);
            }

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
        }

        private static void RenderVariant(AssetBundleVariant variant, TextureCompressionFormat targeting)
        {
            EditorGUILayout.BeginHorizontal();

            // TODO: revisit TextureCompressionFormat enum extension methods.
            var targetingDescription = targeting == TextureCompressionFormat.Default
                ? targeting.ToString()
                : targeting.ToString().ToUpper();
            EditorGUILayout.PrefixLabel(targetingDescription);

            EditorGUILayout.LabelField(variant.FileSizeText, GUILayout.ExpandWidth(false));

            var errors = variant.Errors;
            if (errors.Count == 0)
            {
                EditorGUILayout.LabelField("No errors", GUILayout.ExpandWidth(true));
            }
            else
            {
                if (GUILayout.Button(variant.ErrorSummary + "...", GUILayout.ExpandWidth(true)))
                {
                    var errorDialogTitle = "AssetBundle Error" + (errors.Count == 1 ? string.Empty : "s");
                    var errorDialogMessage = string.Join("\n\n",
                        errors.Select(e => NameAndDescriptionAttribute.GetAttribute(e).Description).ToArray());
                    EditorUtility.DisplayDialog(errorDialogTitle, errorDialogMessage, WindowUtils.OkButtonText);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void RenderFolder(string folderPath, AssetBundleFolder assetBundleFolder, out bool removeFolder)
        {
            // Folder path and associated management buttons.
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(folderPath, EditorStyles.wordWrappedLabel);
            removeFolder = GUILayout.Button("Remove", GUILayout.Width(FieldWidth));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // Display the folder state.
            if (assetBundleFolder.State == AssetPackFolderState.Ok)
            {
                EditorGUILayout.HelpBox(
                    string.Format("Found {0} AssetBundle(s) in this folder", assetBundleFolder.AssetBundleCount),
                    MessageType.None);
            }
            else
            {
                var errorMessage = NameAndDescriptionAttribute.GetAttribute(assetBundleFolder.State).Description;
                EditorGUILayout.HelpBox(errorMessage, MessageType.Warning);
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        private static void RenderTextureCompressionFormatTargetingConfiguration(
            AssetDeliveryConfig assetDeliveryConfig, out bool targetingUpdated)
        {
            targetingUpdated = false;
            const string label = "Format for pre-L devices";
            var width = GUILayout.Width(300);

            EditorGUI.BeginChangeCheck();
#if UNITY_2018_3_OR_NEWER
            var validTextureCompressionFormats = assetDeliveryConfig.GetAllTextureCompressionFormats();
            var newDefaultTextureCompressionFormat =
                (TextureCompressionFormat) EditorGUILayout.EnumPopup(new GUIContent(label),
                    assetDeliveryConfig.DefaultTextureCompressionFormat,
                    textureCompressionFormat =>
                        validTextureCompressionFormats.Contains((TextureCompressionFormat) textureCompressionFormat),
                    false, width);
#else
            var newDefaultTextureCompressionFormat =
                (TextureCompressionFormat) EditorGUILayout.EnumPopup(label,
                    assetDeliveryConfig.DefaultTextureCompressionFormat, width);
#endif
            if (EditorGUI.EndChangeCheck())
            {
                assetDeliveryConfig.DefaultTextureCompressionFormat = newDefaultTextureCompressionFormat;
                targetingUpdated = true;
            }

            EditorGUILayout.HelpBox(
                "This compression format will be used for devices running Android 4.4.4 (SDK 20) or older. " +
                "AssetBundles marked as install-time will be included in the APK generated for these devices. " +
                "Android 5.0 (SDK 21) and newer devices aren't affected by this setting.",
                MessageType.None);
        }

        private static void RenderDescription(string label)
        {
            var descriptionTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Italic,
                wordWrap = true
            };

            EditorGUILayout.BeginVertical("textfield"); // Adds a light grey background.
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(label, descriptionTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private string GetPackagingSummary()
        {
            int numAssetPacksToDeliver;
            var errorMessages = _assetDeliveryConfig.GetPackagingErrorMessages(out numAssetPacksToDeliver);

            if (numAssetPacksToDeliver == 0)
            {
                return _assetDeliveryConfig.SplitBaseModuleAssets
                    ? "The Base APK's assets will be packaged in an asset pack."
                    : "There are no AssetBundles marked for packaging.";
            }

            if (errorMessages.Count > 0)
            {
                const string separator = "\n\n- ";
                return "The following error(s) will occur when building an Android App Bundle:" + separator +
                       string.Join(separator, errorMessages.ToArray());
            }

            var description = numAssetPacksToDeliver == 1
                ? "There is 1 AssetBundle marked for packaging."
                : string.Format("There are {0} AssetBundles marked for packaging.", numAssetPacksToDeliver);
            return _assetDeliveryConfig.SplitBaseModuleAssets
                ? description + " Also, the Base APK's assets will be packaged in an asset pack."
                : description;
        }
    }
}