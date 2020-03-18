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
using System.Linq;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Provides methods to register a constructor and construct a list of instances.
    /// </summary>
    public class Registry<T>
    {
        private readonly List<Func<T>> _registeredConstructors = new List<Func<T>>();

        /// <summary>
        /// Register a constructor to be called whenever <see cref="ConstructInstances"/> is called. The result of the
        /// registered constructor will be included in the result of <see cref="ConstructInstances"/>.
        /// </summary>
        public void RegisterConstructor(Func<T> constructor)
        {
            _registeredConstructors.Add(constructor);
        }

        /// <summary>
        /// Calls all registered constructors and a list of instances.
        /// </summary>
        public IList<T> ConstructInstances()
        {
            return _registeredConstructors.Select(constructor => constructor.Invoke()).ToList();
        }
    }
}