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
    /// An interface for setting and clearing the current InputMappingProvider.
    /// </summary>
    public interface PlayInputMappingClient
    {
        /// <summary>
        /// Sets the current PlayInputMappingProvider.
        /// The specified PlayInputMappingProvider will then be asked to provide input mappings via it's
        /// OnProvideInputMap method, when the game controls overlay is displayed.
        /// </summary>
        /// <param name="inputMappingProvider">An PlayInputMappingProvider.</param>
        void SetInputMappingProvider(PlayInputMappingProvider inputMappingProvider);

        /// <summary>
        /// Clears the current PlayInputMappingProvider. If no PlayInputMappingProvider is
        /// currently set, this method does nothing.
        /// </summary>
        void ClearInputMappingProvider();
    }
}