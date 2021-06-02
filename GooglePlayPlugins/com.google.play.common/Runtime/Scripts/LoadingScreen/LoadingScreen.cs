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

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Google.Play.Common.LoadingScreen
{
    /// <summary>
    /// Downloads the AssetBundle available at AssetBundleUrl and updates LoadingBar with its progress.
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        /// <summary>
        /// The URL used to fetch the AssetBundle on <see cref="Start"/>.
        /// </summary>
        [Tooltip("The URL used to fetch the AssetBundle on Start.")]
        public string AssetBundleUrl;

        /// <summary>
        /// The LoadingBar used to indicated download and install progress.
        /// </summary>
        [Tooltip("The LoadingBar used to indicated download and install progress.")]
        public LoadingBar LoadingBar;

        /// <summary>
        /// The button displayed when a download error occurs. Should call ButtonEventRetryDownload in its onClick()
        /// event.
        /// </summary>
        [Tooltip("The button displayed when a download error occurs. " +
                 "Should call ButtonEventRetryDownload in its onClick() event.")]
        public Button RetryButton;

        // Number of attempts before we show the user a retry button.
        private const int InitialAttemptCount = 3;
        private AssetBundle _bundle;
        private int _assetBundleRetrievalAttemptCount;
        private float _maxLoadingBarProgress;
        private bool _downloading;

        private void Start()
        {
            AttemptAssetBundleDownload(InitialAttemptCount);
        }

        /// <summary>
        /// Attempts to retry the AssetBundle download one time.
        /// </summary>
        public void ButtonEventRetryDownload()
        {
            AttemptAssetBundleDownload(1);
        }

        /// <summary>
        /// Attempts to download the AssetBundle available at AssetBundleUrl.
        /// If it fails numberOfAttempts times, then it will display a retry button.
        /// </summary>
        private void AttemptAssetBundleDownload(int numberOfAttempts)
        {
            if (_downloading)
            {
                Debug.Log("Download attempt ignored because a download is already in progress.");
                return;
            }

            HideRetryButton();
            _maxLoadingBarProgress = 0f;
            StartCoroutine(AttemptAssetBundleDownloadsCo(numberOfAttempts));
        }

        private IEnumerator AttemptAssetBundleDownloadsCo(int numberOfAttempts)
        {
            _downloading = true;

            for (var i = 0; i < numberOfAttempts; i++)
            {
                _assetBundleRetrievalAttemptCount++;
                Debug.LogFormat("Attempt #{0} at downloading AssetBundle...", _assetBundleRetrievalAttemptCount);

                yield return GetAssetBundle(AssetBundleUrl);

                if (_bundle != null)
                {
                    break;
                }

                yield return new WaitForSeconds(0.5f);
            }

            if (_bundle == null)
            {
                ShowRetryButton();
                _downloading = false;
                yield break;
            }

            var sceneLoadOperation = SceneManager.LoadSceneAsync(_bundle.GetAllScenePaths()[0]);
            var installStartFill = Mathf.Max(LoadingBar.AssetBundleDownloadToInstallRatio, _maxLoadingBarProgress);
            yield return LoadingBar.FillUntilDone(sceneLoadOperation, installStartFill, 1f, false);

            _downloading = false;
        }

        private IEnumerator GetAssetBundle(string assetBundleUrl)
        {
            UnityWebRequest webRequest;
            var downloadOperation = StartAssetBundleDownload(assetBundleUrl, out webRequest);

            yield return LoadingBar.FillUntilDone(downloadOperation,
                _maxLoadingBarProgress, LoadingBar.AssetBundleDownloadToInstallRatio, true);

            if (IsNetworkError(webRequest))
            {
                _maxLoadingBarProgress = LoadingBar.Progress;
                Debug.LogFormat("Failed to download AssetBundle: {0}", webRequest.error);
            }
            else
            {
                _bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
            }
        }

        private void ShowRetryButton()
        {
            LoadingBar.gameObject.SetActive(false);
            RetryButton.gameObject.SetActive(true);
        }

        private void HideRetryButton()
        {
            LoadingBar.gameObject.SetActive(true);
            RetryButton.gameObject.SetActive(false);
        }

        private static bool IsNetworkError(UnityWebRequest request)
        {
#if UNITY_2020_2_OR_NEWER
            return request.result == UnityWebRequest.Result.ConnectionError ||
                   request.result == UnityWebRequest.Result.ProtocolError;
#else
            return request.isHttpError || request.isNetworkError;
#endif
        }

        private static AsyncOperation StartAssetBundleDownload(string assetBundleUrl, out UnityWebRequest webRequest)
        {
#if UNITY_2018_1_OR_NEWER
            webRequest = UnityWebRequestAssetBundle.GetAssetBundle(assetBundleUrl);
#else
            webRequest = UnityWebRequest.GetAssetBundle(assetBundleUrl);
#endif
            return webRequest.SendWebRequest();
        }
    }
}
