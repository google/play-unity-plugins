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

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Abstract serializable class representing a task to be run after scripts reload following a player build.
    /// </summary>
    [Serializable]
    public abstract class PostBuildTask
    {
        /// <summary>
        /// A method that is run on the main thread after scripts are reloaded.
        /// </summary>
        public abstract void RunPostBuildTask();
    }
}