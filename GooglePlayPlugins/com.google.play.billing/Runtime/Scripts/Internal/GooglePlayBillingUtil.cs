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
using System.Collections.Generic;
using UnityEngine;

namespace Google.Play.Billing.Internal
{
    /// <summary>
    /// Helper methods for working with Google Play Billing Library"/>.
    /// </summary>
    public class GooglePlayBillingUtil : MonoBehaviour
    {
        private const string GooglePlayBillingLoggingTag = "Google Play Store: ";

#if UNITY_2017_1_OR_NEWER
        private readonly ILogger _logger = Debug.unityLogger;
#else
        private readonly ILogger _logger = Debug.logger;
#endif
        private static readonly List<Action> _callbacks = new List<Action>();
        private static volatile bool _callbacksPending;

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (!_callbacksPending)
                return;
            Action[] array;
            lock (_callbacks)
            {
                if (_callbacks.Count == 0)
                {
                    return;
                }

                array = new Action[_callbacks.Count];
                _callbacks.CopyTo(array);
                _callbacks.Clear();
                _callbacksPending = false;
            }

            foreach (var action in array)
            {
                action();
            }
        }

        /// <summary>
        /// Execute the runnable on the main thread.
        /// </summary>
        public void RunOnMainThread(Action runnable)
        {
            lock (_callbacks)
            {
                _callbacks.Add(runnable);
                _callbacksPending = true;
            }
        }

        /// <summary>
        /// Logs a formatted message with ILogger.
        /// </summary>
        public void LogFormat(string format, params object[] args)
        {
            _logger.LogFormat(LogType.Log, GooglePlayBillingLoggingTag + format, args);
        }

        /// <summary>
        /// Logs a formatted warning message with ILogger.
        /// </summary>
        public void LogWarningFormat(string format, params object[] args)
        {
            _logger.LogFormat(LogType.Warning, GooglePlayBillingLoggingTag + format, args);
        }

        /// <summary>
        /// Logs a formatted error message with ILogger.
        /// </summary>
        public void LogErrorFormat(string format, params object[] args)
        {
            _logger.LogFormat(LogType.Error, GooglePlayBillingLoggingTag + format, args);
        }
    }
}