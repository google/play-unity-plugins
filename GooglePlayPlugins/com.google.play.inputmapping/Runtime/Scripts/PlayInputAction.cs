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
    /// An input action representing one or more single or combined events from the user's input device.
    /// </summary>
    public class PlayInputAction
    {
        /// <summary>
        /// The label, in the current locale, for this input action (e.g. Jump).
        /// </summary>
        public string ActionLabel { get; private set; }

        /// <summary>
        /// An int that uniquely identifies this action.
        /// This value should be unique among all other UniqueIds present in a PlayInputMap.
        /// </summary>
        public int UniqueId { get; private set; }

        /// <summary>
        /// The input controls associated with this action.
        /// </summary>
        public PlayInputControls InputControls { get; private set; }

        /// <summary>
        /// Creates a new PlayInputAction with the specified values.
        /// </summary>
        public static PlayInputAction Create(string actionLabel, int uniqueId, PlayInputControls inputControls)
        {
            return new PlayInputAction
            {
                ActionLabel = actionLabel,
                UniqueId = uniqueId,
                InputControls = inputControls
            };
        }
    }
}