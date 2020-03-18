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

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// A tool used during app builds.
    /// </summary>
    public interface IBuildTool
    {
        /// <summary>
        /// Method that should be called to initialize the <see cref="IBuildTool"/> for use and to check whether there
        /// are any issues with the tool or its dependencies. This method should only be run on the main thread.
        /// Since most <see cref="IBuildTool"/> work may be done on background threads, use this method to copy any
        /// Unity state that can only be accessed from the main thread, e.g. properties from Unity's Application,
        /// EditorPrefs, or PlayerSettings classes.
        /// </summary>
        /// <param name="buildToolLogger">Used to log errors and/or display dialogs if there is a build issue.</param>
        /// <returns>true if this tool is ready to be used, or false if there is an error.</returns>
        bool Initialize(BuildToolLogger buildToolLogger);
    }
}