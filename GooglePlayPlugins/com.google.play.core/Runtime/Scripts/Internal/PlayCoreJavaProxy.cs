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

using UnityEngine;

namespace Google.Play.Core.Internal
{
    /// <summary>
    /// AndroidJavaProxy wrapper that includes methods that were added in 2017.1. This is needed to support 5.6.
    /// </summary>
    public abstract class PlayCoreJavaProxy : AndroidJavaProxy
    {
        public PlayCoreJavaProxy(string javaInterface) : base(javaInterface)
        {
        }

#if !UNITY_2017_1_OR_NEWER
        public bool equals(AndroidJavaObject obj)
        {
            // TODO Find a way to compare obj and this AndroidJavaProxy instead of returning false.
            return false;
        }

        public int hashCode()
        {
            return 0;
        }

        public string toString()
        {
            return "<c# proxy java object>";
        }
#endif
    }
}