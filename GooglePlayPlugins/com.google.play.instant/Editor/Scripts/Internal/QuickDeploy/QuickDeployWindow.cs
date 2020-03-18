// Copyright 2018 Google LLC
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
using UnityEditor;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal.QuickDeploy
{
    public class QuickDeployWindow : EditorWindow
    {
        /// <summary>
        /// Saved configurations from a previous session.
        /// </summary>
        public static readonly QuickDeployConfig Config = new QuickDeployConfig();

        private static readonly string[] ToolbarButtonNames =
        {
            "Overview", "Bundle Creation", "Loading Screen"
        };

        private static int _toolbarSelectedButtonIndex;

        // Keep track of the previous tab to remove focus if user moves to a different tab. (b/112536394)
        private static ToolBarSelectedButton _previousTab;

        public enum ToolBarSelectedButton
        {
            Overview,
            CreateBundle,
            LoadingScreen
        }

        // Style that provides a light box background.
        // Documentation: https://docs.unity3d.com/ScriptReference/GUISkin-textField.html
        private const string UserInputGuiStyle = "textfield";

        private const int WindowMinWidth = 475;
        private const int WindowMinHeight = 400;

        private const int SceneViewDeltaFromTop = 230;

        private const int FieldMinWidth = 100;
        private const int ToolbarHeight = 25;

        // Titles for errors that occur.
        private const string AssetBundleBuildErrorTitle = "AssetBundle Build Error";
        private const string LoadingScreenCreationErrorTitle = "Loading Screen Creation Error";

        private PlayInstantSceneTreeView _playInstantSceneTreeTreeView;

        public static void ShowWindow()
        {
            var window = GetWindow<QuickDeployWindow>(true, "Quick Deploy");
            window.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
        }

        private void OnEnable()
        {
            Config.LoadConfiguration();

            var scenesViewState = Config.AssetBundleScenes ?? new PlayInstantSceneTreeView.State();

            _playInstantSceneTreeTreeView = new PlayInstantSceneTreeView(scenesViewState);
            _playInstantSceneTreeTreeView.OnTreeStateChanged += (treeState) =>
            {
                Config.AssetBundleScenes = treeState;
                Config.SaveConfiguration(true);
            };
        }


        private void Update()
        {
            Config.PollForChanges();
        }

        private void OnGUI()
        {
            _toolbarSelectedButtonIndex = GUILayout.Toolbar(_toolbarSelectedButtonIndex, ToolbarButtonNames,
                GUILayout.MinHeight(ToolbarHeight));
            var currentTab = (ToolBarSelectedButton) _toolbarSelectedButtonIndex;
            UpdateGuiFocus(currentTab);
            switch (currentTab)
            {
                case ToolBarSelectedButton.Overview:
                    OnGuiOverviewSelect();
                    break;
                case ToolBarSelectedButton.CreateBundle:
                    OnGuiCreateBundleSelect();
                    break;
                case ToolBarSelectedButton.LoadingScreen:
                    OnGuiLoadingScreenSelect();
                    break;
            }
        }

        /// <summary>
        /// Unfocus the window if the user has just moved to a different quick deploy tab.
        /// </summary>
        /// <param name="currentTab">A ToolBarSelectedButton instance representing the current quick deploy tab.</param>
        private static void UpdateGuiFocus(ToolBarSelectedButton currentTab)
        {
            if (currentTab != _previousTab)
            {
                _previousTab = currentTab;
                GUI.FocusControl(null);
            }
        }

        private void OnGuiOverviewSelect()
        {
            var descriptionTextStyle = CreateDescriptionTextStyle();
            EditorGUILayout.LabelField("About Quick Deploy", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(UserInputGuiStyle);
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Quick Deploy can significantly reduce the size of a Unity-based instant app " +
                                       "by packaging some assets in an AssetBundle that is retrieved from a server " +
                                       "during app startup.", descriptionTextStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                "Use the \"Bundle Creation\" tab to build an AssetBundle containing the game's " +
                "main scene. Then upload the created AssetBundle to a server or CDN  " +
                "and make the url endpoint public. Finally, use the \"Loading Screen\" tab to select an image to " +
                "display on the loading screen and the URL that points to the uploaded AssetBundle.",
                descriptionTextStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("Use Google Play Instant's \"{0}\" window to customize the " +
                                                     "scenes included in the instant app. Then use the \"{1}\" menu " +
                                                     "option to test the instant app loading the AssetBundle from the " +
                                                     "remote server. Finally, select the \"{2}\" menu option to build " +
                                                     "the app in a manner suitable for publishing on Play Console.",
                "Build Settings", "Build and Run", "Build for Play Console"), descriptionTextStyle);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void OnGuiCreateBundleSelect()
        {
            var descriptionTextStyle = CreateDescriptionTextStyle();

            EditorGUILayout.LabelField("Create AssetBundle", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(UserInputGuiStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Select scenes to be put into an AssetBundle and then build it.",
                descriptionTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(UserInputGuiStyle);
            EditorGUILayout.Space();
            _playInstantSceneTreeTreeView.OnGUI(GUILayoutUtility.GetRect(position.width,
                position.height - SceneViewDeltaFromTop));
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Add Open Scenes"))
            {
                _playInstantSceneTreeTreeView.AddOpenScenes();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Build AssetBundle..."))
            {
                HandleBuildAssetBundleButton();
                HandleDialogExit();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Displays a save dialog, saves the specified path to config, and then creates an AssetBundle at that path.
        /// </summary>
        private void HandleBuildAssetBundleButton()
        {
            string saveFilePath = DialogHelper.SaveFilePanel("Save AssetBundle", Config.AssetBundleFileName, "");
            if (String.IsNullOrEmpty(saveFilePath))
            {
                // Assume cancelled.
                return;
            }

            Config.AssetBundleFileName = saveFilePath;

            try
            {
                Config.SaveConfiguration(true);
                AssetBundleBuilder.BuildQuickDeployAssetBundle(GetEnabledSceneItemPaths());
            }
            catch (Exception ex)
            {
                DialogHelper.DisplayMessage(AssetBundleBuildErrorTitle, ex.Message);
                throw;
            }
        }

        private string[] GetEnabledSceneItemPaths()
        {
            var scenes = _playInstantSceneTreeTreeView.GetRows();
            var scenePaths = new List<string>();

            foreach (var scene in scenes)
            {
                if (((PlayInstantSceneTreeView.SceneItem) scene).Enabled)
                {
                    scenePaths.Add(scene.displayName);
                }
            }

            return scenePaths.ToArray();
        }

        private void OnGuiLoadingScreenSelect()
        {
            var descriptionTextStyle = CreateDescriptionTextStyle();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Configure Loading Scene", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(UserInputGuiStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Specify the URL that points to the deployed AssetBundle. The AssetBundle will be downloaded at game startup. ",
                descriptionTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("AssetBundle URL", GUILayout.MinWidth(FieldMinWidth));
            Config.AssetBundleUrl =
                EditorGUILayout.TextField(Config.AssetBundleUrl, GUILayout.MinWidth(FieldMinWidth));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(UserInputGuiStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(
                "Choose image to use as background for the loading scene.", descriptionTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Background Texture", GUILayout.MinWidth(FieldMinWidth));

            Config.LoadingBackgroundImage = (Texture2D) EditorGUILayout.ObjectField(Config.LoadingBackgroundImage,
                typeof(Texture2D), false,
                GUILayout.MinWidth(FieldMinWidth));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            if (GUILayout.Button("Create Loading Scene..."))
            {
                HandleCreateLoadingSceneButton();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Config.SaveConfiguration(false);
            }
        }

        /// <summary>
        /// Displays a save dialog, saves the specified path to config, and then creates a loading scene at that path.
        /// </summary>
        private void HandleCreateLoadingSceneButton()
        {
            if (string.IsNullOrEmpty(Config.AssetBundleUrl))
            {
                DialogHelper.DisplayMessage(LoadingScreenCreationErrorTitle,
                    "AssetBundle URL text field cannot be null or empty.");
                return;
            }

            string saveFilePath =
                DialogHelper.SaveFilePanelInProject("Create Loading Scene", Config.LoadingSceneFileName, "unity");
            if (String.IsNullOrEmpty(saveFilePath))
            {
                // Assume cancelled.
                return;
            }

            Config.LoadingSceneFileName = saveFilePath;

            try
            {
                Config.SaveConfiguration(true);
                LoadingScreenGenerator.GenerateScene(Config.AssetBundleUrl, Config.LoadingBackgroundImage,
                    saveFilePath);

                // Select the Loading screen element in the generated scene, so the user can see the assetBundle url
                // field in the inspector.
                Selection.SetActiveObjectWithContext(LoadingScreenGenerator.CurrentLoadingScreen, null);
                Close();
            }
            catch (Exception ex)
            {
                DialogHelper.DisplayMessage(LoadingScreenCreationErrorTitle, ex.Message);
                throw;
            }
        }

        // Call this method after any of the SaveFilePanels and OpenFilePanels placed inbetween BeginHorizontal()s or
        // BeginVerticals()s. An error is thrown when a user switches contexts (into a different desktop), and the
        // window reloads. After completing an action, this error is thrown. This method is called to avoid this.
        //  Fix documentation: https://answers.unity.com/questions/1353442/editorutilitysavefilepane-and-beginhorizontal-caus.html
        private void HandleDialogExit()
        {
            GUIUtility.ExitGUI();
        }

        private GUIStyle CreateDescriptionTextStyle()
        {
            return new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Italic,
                wordWrap = true
            };
        }
    }
}