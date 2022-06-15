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

namespace Google.Play.InputMapping
{
    /// <summary>
    /// Exposes a PlayInputMap to the Android framework via its OnProvideInputMap method.
    /// </summary>
    public interface PlayInputMappingProvider
    {
        /// <summary>
        /// Get an <see cref="PlayInputMap"/> containing the controls currently available to the player.
        /// Note: This method can be called on threads other than the main thread.
        /// </summary>
        /// <returns>
        /// An <see cref="PlayInputMap"/>, the contents of which will be displayed to the user in the Android game overlay.
        /// </returns>
        PlayInputMap OnProvideInputMap();
    }
}