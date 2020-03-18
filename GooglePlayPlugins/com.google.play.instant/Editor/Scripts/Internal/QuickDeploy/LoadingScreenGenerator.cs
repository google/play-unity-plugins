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
using Google.Play.Common.LoadingScreen;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Google.Play.Instant.Editor.Internal.QuickDeploy
{
    /// <summary>
    /// Class that generates Unity loading scenes for instant apps.
    /// </summary>
    public static class LoadingScreenGenerator
    {
        public static LoadingScreen CurrentLoadingScreen { get; private set; }

        private const string CanvasName = "Loading Screen Canvas";

        private const string SaveErrorTitle = "Loading Screen Save Error";

        private const int ReferenceWidth = 1080;
        private const int ReferenceHeight = 1920;

        /// <summary>
        /// Creates a scene in the current project that acts as a loading scene until AssetBundles are downloaded.
        /// Takes in an AssetBundle URL and a background image to display behind the loading bar.
        /// Replaces the current loading scene with a new one if it exists.
        /// </summary>
        public static void GenerateScene(string assetBundleUrl, Texture2D loadingScreenImage, string sceneFilePath)
        {
            // Removes the loading scene if it is present, otherwise does nothing.
            EditorSceneManager.CloseScene(
                SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(sceneFilePath)), true);

            var loadingScreenScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            PopulateScene(loadingScreenImage, assetBundleUrl);

            bool saveOk = EditorSceneManager.SaveScene(loadingScreenScene, sceneFilePath);

            if (!saveOk)
            {
                // Not a fatal issue. User can attempt to re-save this scene.
                var warningMessage = string.Format("Issue while saving scene {0}.", sceneFilePath);
                Debug.LogWarning(warningMessage);
                DialogHelper.DisplayMessage(SaveErrorTitle, warningMessage);
            }
            else
            {
                AssetDatabase.Refresh();
                SetMainSceneInBuild(sceneFilePath);
            }
        }

        // Visible for testing
        /// <summary>
        /// Adds the specified path to the build settings, if it isn't there, and marks it as included in the build.
        /// All other scenes are marked as excluded from the build.
        /// </summary>
        public static void SetMainSceneInBuild(string pathToScene)
        {
            var buildScenes = EditorBuildSettings.scenes;
            var index = Array.FindIndex(buildScenes, (scene) => scene.path == pathToScene);

            //Disable all other scenes
            for (int i = 0; i < buildScenes.Length; i++)
            {
                buildScenes[i].enabled = i == index;
            }

            //If the scene isn't already in the list, add it and set it to enabled
            if (index < 0)
            {
                var appendedScenes = new EditorBuildSettingsScene[buildScenes.Length + 1];
                Array.Copy(buildScenes, appendedScenes, buildScenes.Length);
                appendedScenes[buildScenes.Length] = new EditorBuildSettingsScene(pathToScene, true);
                EditorBuildSettings.scenes = appendedScenes;
            }
            else
            {
                EditorBuildSettings.scenes = buildScenes;
            }
        }

        // Visible for testing
        public static void PopulateScene(Texture2D backgroundTexture, string assetBundleUrl)
        {
            var loadingScreenGameObject = new GameObject("Loading Screen");

            var camera = GenerateCamera();
            camera.transform.SetParent(loadingScreenGameObject.transform, false);

            var canvasObject = GenerateCanvas(camera);
            canvasObject.transform.SetParent(loadingScreenGameObject.transform, false);

            var backgroundImage = GenerateBackground(backgroundTexture);
            backgroundImage.transform.SetParent(canvasObject.transform, false);

            var loadingContainer = LoadingBarGenerator.GenerateLoadingContainer();
            loadingContainer.SetParent(canvasObject.transform, false);

            var retryButton = GenerateRetryButton();
            retryButton.transform.SetParent(loadingContainer.transform, false);

            LoadingScreen loadingScreen =
                loadingScreenGameObject.AddComponent<LoadingScreen>();
            loadingScreen.AssetBundleUrl = assetBundleUrl;
            loadingScreen.LoadingBar = LoadingBarGenerator.GenerateLoadingBar();
            loadingScreen.RetryButton = retryButton;
            loadingScreen.LoadingBar.transform.SetParent(loadingContainer.transform, false);
            loadingScreen.LoadingBar.transform.SetAsFirstSibling(); // Places loading bar behind the retry button.

            // Hook up the retry button here so that the AttemptAssetBundleDownload function shows up in its inspector.
            UnityEventTools.AddPersistentListener(retryButton.onClick, loadingScreen.ButtonEventRetryDownload);

            CurrentLoadingScreen = loadingScreen;
        }

        private static Camera GenerateCamera()
        {
            var cameraObject = new GameObject("UI Camera");

            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = ReferenceHeight / 2f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.white;

            return camera;
        }

        private static GameObject GenerateCanvas(Camera camera)
        {
            var canvasObject = new GameObject(CanvasName);

            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;

            var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            canvasScaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<EventSystem>();
            canvasObject.AddComponent<StandaloneInputModule>();
            canvasObject.AddComponent<GraphicRaycaster>();

            return canvasObject;
        }

        private static RawImage GenerateBackground(Texture2D backgroundTexture)
        {
            var backgroundObject = new GameObject("Background");

            var backgroundImage = backgroundObject.AddComponent<RawImage>();
            backgroundImage.texture = backgroundTexture;

            var backgroundRect = backgroundObject.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero; // Scale with parent.
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.sizeDelta = Vector2.zero;

            var backgroundAspectRatioFitter = backgroundObject.AddComponent<AspectRatioFitter>();
            backgroundAspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            if (backgroundImage.texture == null)
            {
                backgroundAspectRatioFitter.aspectRatio = ReferenceWidth / (float) ReferenceHeight;
            }
            else
            {
                var textureDimensions = GetPreImportTextureDimensions(backgroundTexture);
                backgroundAspectRatioFitter.aspectRatio = textureDimensions.x / textureDimensions.y;
            }

            return backgroundImage;
        }

        private static Button GenerateRetryButton()
        {
            var retryObject = new GameObject("Retry Button");

            var retryImage = retryObject.AddComponent<Image>();
            retryImage.sprite = FindReplayButtonSprite();

            var retryRect = retryObject.GetComponent<RectTransform>();
            retryRect.sizeDelta = retryImage.sprite.rect.size;
            retryRect.anchorMin = new Vector2(retryRect.anchorMin.x, 0f);
            retryRect.anchorMax = new Vector2(retryRect.anchorMax.x, 0f);
            retryRect.pivot = new Vector2(0.5f, 0f);

            var retryButton = retryObject.AddComponent<Button>();

            return retryButton;
        }

        /// <summary>
        /// This returns a texture's size before its import settings are applied.
        /// This is useful in cases, for example, where the TextureImporter
        /// rounds an image's size to the nearest power of 2.
        /// </summary>
        /// <exception cref="ArgumentException">If a texture is provided that isn't associated with an asset. </exception>
        private static Vector2 GetPreImportTextureDimensions(Texture2D texture)
        {
            var texturePath = AssetDatabase.GetAssetPath(texture);
            if (string.IsNullOrEmpty(texturePath))
            {
                throw new ArgumentException("The provided texture must be associated with an asset");
            }

            // If the asset path isn't accessible, we default to using the texture's dimensions
            // This occurs if the provided texture is included with the Unity Editor.
            if (!File.Exists(texturePath))
            {
                return new Vector2(texture.width, texture.height);
            }

            // Load the image from disk then return its width and height.
            var imageBytes = File.ReadAllBytes(texturePath);
            var tempTexture = new Texture2D(1, 1);
            tempTexture.LoadImage(imageBytes);
            return new Vector2(tempTexture.width, tempTexture.height);
        }

        /// <summary>
        /// Searches the AssetDatabase for an Asset of type T using the searchFilter string.
        /// See https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html for what can be included in that
        /// string.
        /// </summary>
        /// <exception cref="Exception">Thrown if an asset cannot be found, or if multiple assets are found.</exception>
        public static T FindAssetByFilter<T>(string searchFilter) where T : UnityEngine.Object
        {
            // We search for the asset by name instead of by its path, because we don't require developers to keep the
            // play-instant-unity-plugin folder directly inside the Assets folder.
            var foundGuids = AssetDatabase.FindAssets(searchFilter);

            if (foundGuids.Length == 0)
            {
                throw new Exception("Failed to find any assets that match: " + searchFilter);
            }

            if (foundGuids.Length > 1)
            {
                throw new Exception("Found multiple assets that match: " + searchFilter);
            }

            var path = AssetDatabase.GUIDToAssetPath(foundGuids[0]);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        // Visible for testing.
        public static Sprite FindReplayButtonSprite()
        {
            return FindAssetByFilter<Sprite>("GooglePlayInstantRetryButton t:sprite");
        }
    }
}