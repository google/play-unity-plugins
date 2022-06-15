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

using System.Collections.Generic;

namespace Google.Play.InputMapping
{
    /// <summary>
    /// Specifies a mapping from keyboard/mouse input to game actions.
    /// </summary>
    public class PlayInputMap
    {
        /// <summary>
        /// A list of <see cref="PlayInputGroup"/>s in the order they will be displayed in the Android input overlay.
        /// </summary>
        public IList<PlayInputGroup> InputGroups { get; private set; }

        /// <summary>
        /// Settings related to mouse input.
        /// </summary>
        public PlayMouseSettings MouseSettings { get; private set; }

        /// <summary>
        /// Creates a new PlayInputMap with the specified values.
        /// </summary>
        public static PlayInputMap Create(IList<PlayInputGroup> inputGroups, PlayMouseSettings mouseSettings)
        {
            return new PlayInputMap { InputGroups = inputGroups, MouseSettings = mouseSettings };
        }
    }
}