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

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Classes that implement this interface will be instantiated by <see cref="BuildAndRun"/> and given the
    /// opportunity to override the default functionality with their own.
    ///
    /// Implementing classes must expose a public constructor with no arguments, in order for <see cref="BuildAndRun"/>
    /// to initialize them.
    /// </summary>
    public interface IBuildAndRunExtension
    {
        /// <summary>
        /// Whether or not to override the default Build and Run action.
        /// </summary>
        bool ShouldOverride();

        /// <summary>
        /// Perform custom Build and Run action.
        /// Only called if ShouldOverride() returns true.
        /// </summary>
        void BuildAndRun();
    }
}