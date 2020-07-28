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
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Forwards pack state updates coming from play core.
    /// We can't receive updates while the app is in the background, so this MonoBehaviour ensures that pack states
    /// are updated when the app enters the foreground.
    /// </summary>
    internal class AssetDeliveryUpdateHandler : MonoBehaviour
    {
        public event Action<AssetPackState> OnStateUpdateEvent = delegate { };

        /// <summary>
        /// The pack names of any AssetBundles that have received state updates after GetPackStates but before the
        /// Play Core Task has completed. This is to prevent the race condition between the task returned from
        /// <see cref="AssetPackManager.GetPackStates"/> and
        /// <see cref="AssetPackStateUpdateListener.OnStateUpdateEvent"/>.
        /// </summary>
        private readonly HashSet<string> _stateUpdatesSinceGetPackStates = new HashSet<string>();

        private bool _gettingPackStates;
        private AssetPackStateUpdateListener _stateUpdateListener;
        private AssetPackManager _assetPackManager;
        private PlayRequestRepository _requestRepository;

        public static AssetDeliveryUpdateHandler CreateInScene(AssetPackManager assetPackManager,
            PlayRequestRepository requestRepository)
        {
            var componentHolder = new GameObject();
            DontDestroyOnLoad(componentHolder);
            componentHolder.name = "AssetDeliveryUpdateHandler";

            var instance = componentHolder.AddComponent<AssetDeliveryUpdateHandler>();
            instance.Init(assetPackManager, requestRepository);

            return instance;
        }

        private void Init(AssetPackManager assetPackManager, PlayRequestRepository requestRepository)
        {
            _assetPackManager = assetPackManager;
            _requestRepository = requestRepository;
            _stateUpdateListener = new AssetPackStateUpdateListener();
            _stateUpdateListener.OnStateUpdateEvent += OnStateUpdateReceived;
            StartListeningForUpdates();
        }

        private void Start()
        {
            if (_assetPackManager == null || _stateUpdateListener == null)
            {
                throw new InvalidOperationException("AssetDeliveryUpdateHandler was never initialized.");
            }
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                StopListeningForUpdates();
            }
            else
            {
                StartListeningForUpdates();
                ForcePackStatesUpdate();
            }
        }

        private void OnDestroy()
        {
            StopListeningForUpdates();
        }

        /// <summary>
        /// Asks <see cref="AssetPackManager"/> for the current pack states and routes them to
        /// <see cref="OnForcedStateUpdate"/>.
        /// </summary>
        private void ForcePackStatesUpdate()
        {
            if (_gettingPackStates)
            {
                Debug.LogWarning(
                    "ForceStateUpdate attempt ignored because the latest states are still being retrieved.");
                return;
            }

            var activeRequestBundleNames = _requestRepository.GetActiveAssetPackNames();
            if (activeRequestBundleNames.Length == 0)
            {
                // No requests to update.
                return;
            }

            BeginGetPackStates();
            var getPackStateTask = _assetPackManager.GetPackStates(activeRequestBundleNames);
            getPackStateTask.RegisterOnSuccessCallback(ProcessPackStates);
        }

        /// <summary>
        /// Processes a java AssetPackStates object and invokes <see cref="OnForcedStateUpdate"/> for each pack
        /// state it contains.
        /// </summary>
        /// <param name="javaPackStates">A java object representing a AssetPackStates object.</param>
        private void ProcessPackStates(AndroidJavaObject javaPackStates)
        {
            try
            {
                var packStates = new AssetPackStates(javaPackStates);

                foreach (var packStatePair in packStates.PackStates)
                {
                    if (_stateUpdatesSinceGetPackStates.Contains(packStatePair.Key))
                    {
                        // A newer state update was received before the getPackStateTask finished.
                        // Discard this state.
                        continue;
                    }

                    OnStateUpdateEvent.Invoke(packStatePair.Value);
                }
            }
            finally
            {
                EndGetPackStates();
            }
        }

        private void OnStateUpdateReceived(AssetPackState newState)
        {
            if (_gettingPackStates)
            {
                _stateUpdatesSinceGetPackStates.Add(newState.Name);
            }

            OnStateUpdateEvent.Invoke(newState);
        }

        private void BeginGetPackStates()
        {
            _gettingPackStates = true;
        }

        private void EndGetPackStates()
        {
            _gettingPackStates = false;
            _stateUpdatesSinceGetPackStates.Clear();
        }

        private void StartListeningForUpdates()
        {
            _assetPackManager.RegisterListener(_stateUpdateListener);
        }

        private void StopListeningForUpdates()
        {
            // Due to a Unity 5.6 issue that prevents unregisterListener from working with
            // AndroidJavaProxies, we clear all listeners rather than just _stateUpdateListener.
            _assetPackManager.ClearListeners();
        }
    }
}