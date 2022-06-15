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
    /// A labeled group of <see cref="PlayInputAction"/>s.
    /// </summary>
    public class PlayInputGroup
    {
        /// <summary>
        /// The label, in the current locale, for these collection of input actions (e.g. movement).
        /// </summary>
        public string GroupLabel { get; private set; }

        /// <summary>
        /// A list of input actions associated with this group.
        /// </summary>
        public IList<PlayInputAction> InputActions { get; private set; }

        /// <summary>
        /// Creates a new PlayInputGroup with the specified values.
        /// </summary>
        public static PlayInputGroup Create(string groupLabel, IList<PlayInputAction> inputActions)
        {
            return new PlayInputGroup { GroupLabel = groupLabel, InputActions = inputActions };
        }
    }
}