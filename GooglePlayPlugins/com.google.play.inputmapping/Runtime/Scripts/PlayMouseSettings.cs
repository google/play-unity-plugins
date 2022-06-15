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
    /// Settings related to mouse input.
    /// </summary>
    public class PlayMouseSettings
    {
        /// <summary>
        /// True if the Android input overlay should contain an option for the user to adjust the system mouse sensitivity.
        /// </summary>
        public bool AllowMouseSensitivityAdjustment { get; private set; }

        /// <summary>
        /// True if the game uses inverted mouse controls.
        /// </summary>
        public bool InvertMouseMovement { get; private set; }

        /// <summary>
        /// Creates a new PlayMouseSettings with the specified values.
        /// </summary>
        public static PlayMouseSettings Create(bool allowMouseSensitivityAdjustment, bool invertMouseMovement)
        {
            return new PlayMouseSettings {AllowMouseSensitivityAdjustment = allowMouseSensitivityAdjustment, InvertMouseMovement = invertMouseMovement};
        }
    }
}