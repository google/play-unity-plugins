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
    /// The input controls for a given action.
    /// Setting both keyboard and mouse events means both need to be happen together to trigger the action.
    /// </summary>
    public class PlayInputControls
    {
        /// <summary>
        /// The list of keycodes that need to be pressed to trigger the associated action.
        /// Multiple entries indicate that multiple buttons need to be held down together to trigger the action.
        /// See KeyEvent https://developer.android.com/reference/android/view/KeyEvent for a list of valid keycodes.
        /// Modifier keys, such as KEY_CODE_SHIFT_LEFT can be set in conjunction with KEYCODE_A if both buttons
        /// need to be held down.
        /// </summary>
        public IList<int> AndroidKeycodes { get; private set; }

        /// <summary>
        /// The PlayMouseAction for this input.
        /// </summary>
        public IList<PlayMouseAction> MouseActions { get; private set; }

        /// <summary>
        /// Creates a new PlayInputControls with the specified values.
        /// </summary>
        public static PlayInputControls Create(IList<int> androidKeycodes, IList<PlayMouseAction> mouseActions)
        {
            return new PlayInputControls { AndroidKeycodes = androidKeycodes, MouseActions = mouseActions };
        }
    }
}