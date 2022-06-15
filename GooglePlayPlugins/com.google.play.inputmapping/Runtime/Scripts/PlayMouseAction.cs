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
    /// Enum representing an input action from the mouse.
    /// </summary>
    public enum PlayMouseAction
    {
        MouseActionUnspecified = 0,
        MouseRightClick = 1,
        MouseTertiaryClick = 2,
        MouseForwardClick = 3,
        MouseBackClick = 4,
        MouseScrollUp = 5,
        MouseScrollDown = 6,
        MouseMovement = 7,
        MouseLeftDrag = 8,
        MouseRightDrag = 9,
        MouseLeftClick = 10
    }
}
