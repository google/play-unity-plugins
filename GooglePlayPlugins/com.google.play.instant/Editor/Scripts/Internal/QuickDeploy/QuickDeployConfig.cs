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
using System.IO;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal.QuickDeploy
{
    /// <summary>
    /// Contains a set of operations for storing and retrieving quick deploy configurations.
    /// Used to preserve user input data when quick deploy windows are reloaded or closed.
    /// </summary>
    public class QuickDeployConfig
    {
        internal static readonly string EditorConfigurationFilePath =
            Path.Combine("Library", "PlayInstantQuickDeployEditorConfig.json");

        /// <summary>
        /// The Editor Configuration singleton that should be used to read and modify Quick Deploy configuration.
        /// Modified values are persisted by calling SaveEditorConfiguration.
        /// </summary>
        private EditorConfiguration _editorConfig;

        // Copy of fields from EditorConfig for holding unsaved values set in the UI.
        public string AssetBundleUrl;
        public string AssetBundleFileName;
        public string LoadingSceneFileName;
        public Texture2D LoadingBackgroundImage;
        public PlayInstantSceneTreeView.State AssetBundleScenes;

        private const float SaveCooldown = 1.0f; //How long should we wait before saving config in seconds.
        private bool _configChangedSinceLastSave;
        private float _lastSaveTime;

        public void LoadConfiguration()
        {
            _editorConfig = LoadEditorConfiguration(EditorConfigurationFilePath);

            // Copy of fields from EditorConfig for holding unsaved values set in the UI.
            AssetBundleUrl = _editorConfig.assetBundleUrl;
            AssetBundleFileName = _editorConfig.assetBundleFileName;
            AssetBundleScenes = _editorConfig.assetBundleScenes;
            LoadingSceneFileName = _editorConfig.loadingSceneFileName;
            LoadingBackgroundImage = _editorConfig.loadingBackgroundImage;
        }

        public void PollForChanges()
        {
            var timeSinceLastSave = Time.realtimeSinceStartup - _lastSaveTime;
            if (timeSinceLastSave > SaveCooldown && _configChangedSinceLastSave)
            {
                SaveEditorConfiguration(_editorConfig, EditorConfigurationFilePath);
            }
        }

        /// <summary>
        /// Store configuration to persistent storage.
        /// </summary>
        /// <param name="saveImmediately">
        /// If false, writing to persistent storage may be delayed by up to SaveCooldown seconds to limit file I/O.
        /// </param>
        public void SaveConfiguration(bool saveImmediately)
        {
            if (saveImmediately)
            {
                SaveEditorConfiguration(_editorConfig, EditorConfigurationFilePath);
            }
            else
            {
                _configChangedSinceLastSave = true;
            }
        }

        // Visible for testing
        public void SaveEditorConfiguration(EditorConfiguration configuration, string editorConfigurationPath)
        {
            _lastSaveTime = Time.realtimeSinceStartup;
            _configChangedSinceLastSave = false;

            configuration.assetBundleFileName = AssetBundleFileName;
            configuration.assetBundleScenes = AssetBundleScenes;
            configuration.assetBundleUrl = AssetBundleUrl;
            configuration.loadingBackgroundImage = LoadingBackgroundImage;
            configuration.loadingSceneFileName = LoadingSceneFileName;

            // Shouldn't hurt to write to persistent storage as long as SaveEditorConfiguration(currentTab) is only
            // called when a major action happens.
            File.WriteAllText(editorConfigurationPath, JsonUtility.ToJson(configuration));
        }

        // Visible for testing
        /// <summary>
        /// De-serialize editor configuration file contents into EditorConfiguration instance if the file exists exists,
        /// otherwise return Configuration instance with empty fields.
        /// </summary>
        public static EditorConfiguration LoadEditorConfiguration(string editorConfigurationPath)
        {
            if (!File.Exists(editorConfigurationPath))
            {
                return new EditorConfiguration();
            }

            var configurationJson = File.ReadAllText(editorConfigurationPath);
            return JsonUtility.FromJson<EditorConfiguration>(configurationJson);
        }

        /// <summary>
        /// Represents JSON contents of the quick deploy configuration file.
        /// </summary>
        [Serializable]
        public class EditorConfiguration
        {
            public string assetBundleUrl;
            public string assetBundleFileName = Path.Combine("Assets", "MainBundle");
            public string loadingSceneFileName = Path.Combine("Assets", "PlayInstantLoadingScreen.unity");
            public Texture2D loadingBackgroundImage;
            public PlayInstantSceneTreeView.State assetBundleScenes;
        }
    }
}