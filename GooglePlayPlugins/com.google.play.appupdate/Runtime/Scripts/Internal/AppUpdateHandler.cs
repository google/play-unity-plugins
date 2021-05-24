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
using UnityEngine;

namespace Google.Play.AppUpdate.Internal
{
    /// <summary>
    /// Forwards app update states coming from Play Core. We can't receive updates while the app is in the background,
    /// so this MonoBehaviour ensures that states are updated when the app enters the foreground.
    /// </summary>
    internal class AppUpdateHandler : MonoBehaviour
    {
        public event Action<AppUpdateState> OnStateUpdateEvent = delegate { };

        private AppUpdateStateListener _stateUpdateListener;
        private AppUpdateManagerInternal _appUpdateManagerInternal;

        public static AppUpdateHandler CreateInScene(AppUpdateManagerInternal appUpdateManagerInternal)
        {
            var componentHolder = new GameObject();
            componentHolder.name = "AppUpdateHandler";
            DontDestroyOnLoad(componentHolder);

            var instance = componentHolder.AddComponent<AppUpdateHandler>();
            instance.Init(appUpdateManagerInternal);

            return instance;
        }

        private void Init(AppUpdateManagerInternal appUpdateManagerInternal)
        {
            _appUpdateManagerInternal = appUpdateManagerInternal;
            _stateUpdateListener = new AppUpdateStateListener();
            _stateUpdateListener.OnStateUpdateEvent += OnStateUpdateReceived;
            StartListeningForUpdates();
        }

        private void Start()
        {
            if (_appUpdateManagerInternal == null || _stateUpdateListener == null)
            {
                throw new InvalidOperationException("AppUpdateHandler was never initialized.");
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
                ForceStatesUpdate();
            }
        }

        private void OnDestroy()
        {
            StopListeningForUpdates();
        }

        private void ForceStatesUpdate()
        {
            // Request the state of updates currently in progress through getAppUpdateInfo(), returning a wrapped
            // Play Core task which will return an AppUpdateInfo AndroidJavaObject.
            var appUpdateStateTask = _appUpdateManagerInternal.GetAppUpdateInfo();
            appUpdateStateTask.RegisterOnSuccessCallback(ProcessUpdateInfo);
        }

        private void ProcessUpdateInfo(AndroidJavaObject javaAppUpdateInfo)
        {
            // Parsing AppUpdateInfo java object as an AppUpdateState since AppUpdateInfo contains all the fields for
            // an AppUpdateState.
            var updateState = new AppUpdateState(javaAppUpdateInfo);
            OnStateUpdateEvent.Invoke(updateState);
        }

        private void OnStateUpdateReceived(AppUpdateState newState)
        {
            OnStateUpdateEvent.Invoke(newState);
        }

        private void StartListeningForUpdates()
        {
            _appUpdateManagerInternal.RegisterListener(_stateUpdateListener);
        }

        private void StopListeningForUpdates()
        {
            _appUpdateManagerInternal.UnregisterListener(_stateUpdateListener);
        }
    }
}