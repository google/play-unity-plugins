// Copyright 2021 Google LLC
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
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Google.Play.InputMapping.Tests")]

namespace Google.Play.InputMapping.Internal
{
    /// <summary>
    /// Internal implementation of <see cref="PlayInputMappingClient"/> that interfaces with the underlying library.
    /// </summary>
    internal class InputMappingClientInternal : PlayInputMappingClient
    {
        private const string InputClassName = JavaConstants.PackagePrefix + "Input";
        private readonly AndroidJavaObject _javaInputMappingClient;
        private UnityInputMappingProviderProxy _currentProxy;

        /// <summary>
        /// Constructor.
        /// </summary>
        public InputMappingClientInternal()
        {
            using (var activity = GetCurrentActivity())
            using (var inputClass = CreateInput())
            {
                _javaInputMappingClient = inputClass.CallStatic<AndroidJavaObject>("getInputMappingClient", activity);
            }
        }

        /// <summary>
        /// Calls the underlying Java library to register the specified <see cref="PlayInputMappingProvider"/>.
        /// </summary>
        public void SetInputMappingProvider(PlayInputMappingProvider inputMappingProvider)
        {
            _currentProxy = new UnityInputMappingProviderProxy(inputMappingProvider);
            _javaInputMappingClient.Call("setInputMappingProvider", _currentProxy);
        }

        /// <summary>
        /// Calls the underlying Java library to clear the current <see cref="PlayInputMappingProvider"/>.
        /// </summary>
        public void ClearInputMappingProvider()
        {
            if (_currentProxy == null)
            {
                return;
            }

            _javaInputMappingClient.Call("clearInputMappingProvider");
            _currentProxy = null;
        }

        /// <summary>
        /// Triggers the OnProvideInputMap method of the registered <see cref="PlayInputMappingProvider"/>.
        /// Available for testing purposes.
        /// </summary>
        public AndroidJavaObject TriggerOnProvide()
        {
            if (_currentProxy == null)
            {
                return null;
            }

            return _currentProxy.onProvideInputMap();
        }

        /// <summary>
        /// Returns an AndroidJavaObject representing the underlying Input class.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the underlying java class cannot be found.
        /// </exception>
        private static AndroidJavaObject CreateInput()
        {
            try
            {
                return new AndroidJavaClass(InputClassName);
            }
            catch (AndroidJavaException e)
            {
                var message = String.Format(
                    "Failed to create an instance of the underlying Java class: {0}. The InputMapping AAR could be " +
                    "missing, or this class was removed or obfuscated by proguard.", InputClassName);
                throw new InvalidOperationException(message, e);
            }
        }

        /// <summary>
        /// Gets the current activity running in Unity. This object should be disposed after use.
        /// </summary>
        /// <returns>A wrapped activity object. The AndroidJavaObject should be disposed.</returns>
        private static AndroidJavaObject GetCurrentActivity()
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
    }
}