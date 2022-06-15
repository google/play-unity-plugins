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
// limitations under the License

using UnityEngine;

namespace Google.Play.InputMapping.Internal
{
    /// <summary>
    /// An AndroidJavaProxy responsible for mediating onProvideInputMap
    /// calls across the JNI boundary.
    /// </summary>
    internal class UnityInputMappingProviderProxy : AndroidJavaProxy
    {
        public PlayInputMappingProvider InputMappingProvider { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputMappingProvider"></param>
        public UnityInputMappingProviderProxy(PlayInputMappingProvider inputMappingProvider) :
            base(JavaConstants.PackagePrefix + "InputMappingProvider")
        {
            InputMappingProvider = inputMappingProvider;
        }

        /// <summary>
        /// Implements the underlying Java object's onProvideInputMap method.
        /// The method name deviates from the C# naming convention to match
        /// the name of the underlying Java method.
        /// </summary>
        public AndroidJavaObject onProvideInputMap()
        {
            var inputMap = InputMappingProvider.OnProvideInputMap();
            return InputMapHelper.ConvertToJavaObject(inputMap);
        }
    }
}