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

namespace Google.Play.Core.Internal
{
    /// <summary>
    /// Callbacks from Play Core can come from multiple threads.
    /// This class provides methods to forward those to Unity's main thread, where Unity APIs can be called.
    /// </summary>
    public class PlayCoreEventHandler : MonoBehaviour
    {
        private static PlayCoreEventHandler _instance;

        // This must be called from Main thread.
        public static void CreateInScene()
        {
            var obj = new GameObject {name = "PlayCoreEventHandler"};
            DontDestroyOnLoad(obj);
            obj.AddComponent<PlayCoreEventHandler>();
        }

        /// <summary>
        /// Schedules an action to invoke on Unity's main thread.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If there is no instance of PlayCoreEventHandler present in the scene, either because CreateInScene() was
        /// never called or because the instance was somehow destroyed.
        /// </exception>
        public static void HandleEvent(Action action)
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("An instance of PlayCoreEventHandler is not present in the scene.");
            }

            _instance.HandleEventInternal(action);
        }

        // Queue accessed by multiple threads.
        // Note: ConcurrentQueue isn't available in .NET 3.5.
        private readonly Queue<Action> _sharedEventQueue = new Queue<Action>();

        // Queue that is only accessed on the main thread.
        private readonly Queue<Action> _localEventQueue = new Queue<Action>();

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(this);
                return;
            }

            _instance = this;
        }

        private void HandleEventInternal(Action action)
        {
            lock (_sharedEventQueue)
            {
                _sharedEventQueue.Enqueue(action);
            }
        }

        private void Update()
        {
            lock (_sharedEventQueue)
            {
                while (_sharedEventQueue.Count > 0)
                {
                    var action = _sharedEventQueue.Dequeue();
                    _localEventQueue.Enqueue(action);
                }
            }

            // Invoke events outside of the lock because they may take an indeterminate amount of time.
            while (_localEventQueue.Count > 0)
            {
                _localEventQueue.Dequeue().Invoke();
            }
        }
    }
}