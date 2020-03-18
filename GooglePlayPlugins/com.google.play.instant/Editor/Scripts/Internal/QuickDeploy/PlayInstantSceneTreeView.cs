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
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Google.Play.Instant.Editor.Internal.QuickDeploy
{
    /// <summary>
    /// Class that encapsulates the TreeView representation of all scenes found in the current project.
    /// </summary>
    public class PlayInstantSceneTreeView : TreeView
    {
        private const int ToggleWidth = 18;
        private int rowID = 0;
        private List<SceneItem> _allItems = new List<SceneItem>();

        public event Action<State> OnTreeStateChanged = delegate { };

        public PlayInstantSceneTreeView(State scenesViewState)
            : base(scenesViewState.ViewState)
        {
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            extraSpaceBeforeIconAndLabel = ToggleWidth;

            AddScenes(scenesViewState.ScenePaths, scenesViewState.IsSceneEnabled);

            Reload();
        }

        /// <summary>
        /// Public inner class that contains all the state necessary to restore the PlayInstantSceneTreeView.
        /// </summary>
        [Serializable]
        public class State
        {
            // We use two implicitly linked arrays here because Unity doesn't serialize structs.
            public string[] ScenePaths;
            public bool[] IsSceneEnabled;
            public TreeViewState ViewState;

            public State()
            {
                ScenePaths = new string[0];
                IsSceneEnabled = new bool[0];
                ViewState = new TreeViewState();
            }
        }

        /// <summary>
        /// Public inner class that extends the TreeViewItem representation of Scenes to include the enabled attribute.
        /// </summary>
        public class SceneItem : TreeViewItem
        {
            public bool Enabled;
            public bool OldEnabledValue;
            public string SceneBuildIndexString;
        }

        public void AddOpenScenes()
        {
            var scenePaths = GetOpenScenePaths();
            var isSceneEnabled = new bool[scenePaths.Length];
            for (int i = 0; i < isSceneEnabled.Length; i++)
            {
                isSceneEnabled[i] = true;
            }

            AddScenes(scenePaths, isSceneEnabled);
        }

        private void AddScenes(string[] scenePaths, bool[] isSceneEnabled)
        {
            for (var i = 0; i < scenePaths.Length; i++)
            {
                var sceneItem = new SceneItem
                {
                    id = rowID++,
                    depth = 0,
                    displayName = scenePaths[i],
                    Enabled = isSceneEnabled[i]
                };

                var duplicateItem = _allItems.Find((element) => element.displayName == sceneItem.displayName);
                if (duplicateItem == null)
                {
                    _allItems.Add(sceneItem);
                }
            }

            OnRowsChanged();
            Reload();
        }

        private void OnRowsChanged()
        {
            var sceneViewState = new State();
            sceneViewState.ViewState = new TreeViewState();
            sceneViewState.ScenePaths = _allItems.Select(sceneItem => sceneItem.displayName).ToArray();
            sceneViewState.IsSceneEnabled = _allItems.Select(sceneItem => sceneItem.Enabled).ToArray();

            OnTreeStateChanged.Invoke(sceneViewState);
            EditSceneBuildIndexString();
        }

        private void EditSceneBuildIndexString()
        {
            var buildIndex = 0;
            foreach (var item in _allItems)
            {
                var sceneItem = (SceneItem) item;
                sceneItem.SceneBuildIndexString = sceneItem.Enabled ? "" + buildIndex++ : "";
            }
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem
            {
                id = 0,
                depth = -1,
                displayName = "Root"
            };

            var items = _allItems.Cast<TreeViewItem>().ToList();
            SetupParentsAndChildrenFromDepths(root, items);
            return root;
        }

        private static string[] GetOpenScenePaths()
        {
            var scenePaths = new string[SceneManager.sceneCount];
            for (var i = 0; i < scenePaths.Length; i++)
            {
                scenePaths[i] = SceneManager.GetSceneAt(i).path;
            }

            return scenePaths;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var toggleRect = args.rowRect;
            toggleRect.x += GetContentIndent(args.item);
            toggleRect.width = ToggleWidth;

            var item = (SceneItem) args.item;

            item.OldEnabledValue = item.Enabled;

            item.Enabled = EditorGUI.Toggle(toggleRect, item.Enabled);

            if (item.OldEnabledValue != item.Enabled)
            {
                OnRowsChanged();
            }

            DefaultGUI.LabelRightAligned(args.rowRect, item.SceneBuildIndexString, args.selected, args.focused);

            base.RowGUI(args);

            var current = Event.current;

            if (args.rowRect.Contains(current.mousePosition) && current.type == EventType.ContextClick)
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove Selection"), false, RemoveSelectedScenes, item);
                menu.ShowAsContext();
            }
        }

        private void RemoveSelectedScenes(object clickedItem)
        {
            var selectedIds = GetSelection();

            //If nothing is selected, just remove the item that was right-clicked
            if (selectedIds.Count <= 0)
            {
                _allItems.Remove((SceneItem) clickedItem);
            }
            else
            {
                _allItems = _allItems.Where((item) => !selectedIds.Contains(item.id)).ToList();
            }

            OnRowsChanged();
            Reload();
        }
    }
}