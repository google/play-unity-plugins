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
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.Utils
{
    /// <summary>
    /// Serializable class encapsulating parameters for running a command line process. This fields of this class
    /// are set up to survive a Unity script reload.
    /// </summary>
    [Serializable]
    public class CommandLineParameters
    {
        [SerializeField] public string FileName;
        [SerializeField] public string Arguments;

        // Unity doesn't support serializing dictionaries, but does support List<string>.
        // See https://docs.unity3d.com/Manual/script-Serialization.html
        [SerializeField] private List<string> _environmentKeys;
        [SerializeField] private List<string> _environmentValues;

        public void AddEnvironmentVariable(string key, string value)
        {
            if (_environmentKeys == null || _environmentValues == null)
            {
                _environmentKeys = new List<string>();
                _environmentValues = new List<string>();
            }

            _environmentKeys.Add(key);
            _environmentValues.Add(value);
        }

        public Dictionary<string, string> EnvironmentVariables
        {
            get
            {
                if (_environmentKeys == null || _environmentKeys.Count == 0)
                {
                    return null;
                }

                if (_environmentKeys.Count != _environmentValues.Count)
                {
                    throw new Exception(string.Format(
                        "EnvironmentVariables size mismatch for keys ({0}) and values({1})",
                        _environmentKeys.Count, _environmentValues.Count));
                }

                var dictionary = new Dictionary<string, string>();
                for (var i = 0; i < _environmentKeys.Count; i++)
                {
                    dictionary.Add(_environmentKeys[i], _environmentValues[i]);
                }

                return dictionary;
            }
        }
    }
}