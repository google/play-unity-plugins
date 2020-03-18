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
using Google.Android.AppBundle.Editor.Internal.Config;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// A window for managing settings related to building an Android app for distribution on Google Play.
    /// </summary>
    public class BuildSettingsWindow : EditorWindow
    {
        public const string WindowTitle = "Play Build Settings";
        private const int WindowMinWidth = 500;
        private const int WindowMinHeight = 250;
        private const int FieldWidth = 175;

        private static BuildSettingsWindow _windowInstance;

        private string _assetBundleManifestPath;

        /// <summary>
        /// Displays this window, creating it if necessary.
        /// </summary>
        public static void ShowWindow()
        {
            _windowInstance = (BuildSettingsWindow) GetWindow(typeof(BuildSettingsWindow), true, WindowTitle);
            _windowInstance.minSize = new Vector2(WindowMinWidth, WindowMinHeight);
        }

        private void OnDestroy()
        {
            _windowInstance = null;
        }

        private void Awake()
        {
            ReadFromBuildConfiguration();
        }

        /// <summary>
        /// Read and update the window with most recent build configuration values.
        /// </summary>
        void ReadFromBuildConfiguration()
        {
            _assetBundleManifestPath = AndroidBuildConfiguration.AssetBundleManifestPath;
        }

        /// <summary>
        /// Update window with most recent build configuration values if the window is open.
        /// </summary>
        public static void UpdateWindowIfOpen()
        {
            if (_windowInstance != null)
            {
                _windowInstance.ReadFromBuildConfiguration();
                _windowInstance.Repaint();
            }
        }

        private void OnGUI()
        {
            // Edge case that takes place when the plugin code gets re-compiled while this window is open.
            if (_windowInstance == null)
            {
                _windowInstance = this;
            }

            var descriptionTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Italic,
                wordWrap = true
            };

            EditorGUILayout.LabelField("Scenes in Build", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                string.Format(
                    "Note: the following settings affect APKs and app bundles built using the \"{0}\" menu's " +
                    "build actions, but not apps built using the \"File\" menu's build actions.",
                    GoogleEditorMenu.MainMenuName), descriptionTextStyle);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "The scenes in the build are selected via Unity's \"Build Settings\" window.", descriptionTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            var enabledScenes = string.Join(", ", AndroidBuildHelper.GetEditorBuildEnabledScenes());
            EditorGUILayout.LabelField(string.Format("Scenes: {0}", enabledScenes), EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Update", GUILayout.Width(100)))
            {
                GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"), true);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(
                "If you use AssetBundles, provide the path to your AssetBundle Manifest file below to ensure that " +
                "required engine components are not stripped during the build process.", descriptionTextStyle);
            EditorGUILayout.Space();
            _assetBundleManifestPath =
                GetLabelAndTextField("AssetBundle Manifest (Optional)", _assetBundleManifestPath);
            EditorGUILayout.Space();

            // Disable the Save button unless one of the fields has changed.
            GUI.enabled = IsAnyFieldChanged();

            if (GUILayout.Button("Save"))
            {
                SaveConfiguration();
            }

            GUI.enabled = true;
        }

        private bool IsAnyFieldChanged()
        {
            return _assetBundleManifestPath != AndroidBuildConfiguration.AssetBundleManifestPath;
        }

        private static string GetLabelAndTextField(string label, string text)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(FieldWidth));
            var result = EditorGUILayout.TextField(text);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            return result;
        }

        private void SaveConfiguration()
        {
            _assetBundleManifestPath = _assetBundleManifestPath.Trim();
            AndroidBuildConfiguration.SaveConfiguration(_assetBundleManifestPath);

            Debug.Log("Saved Android Build Settings");

            // If a TextField is in focus, it won't update to reflect the Trim(). So reassign focus to controlID 0.
            GUIUtility.keyboardControl = 0;
            Repaint();
        }
    }
}