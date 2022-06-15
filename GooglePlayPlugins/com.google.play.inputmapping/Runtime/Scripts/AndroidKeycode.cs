// Copyright (C) 2006 The Android Open Source Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Google.Play.InputMapping
{
    /// <summary>
    /// A collection of Android KeyCodes copied from android.view.KeyEvents, specifically this commit:
    /// https://cs.android.com/android/platform/superproject/+/master:frameworks/base/core/java/android/view/KeyEvent.java;drc=5d123b67756dffcfdebdb936ab2de2b29c799321
    /// </summary>
    public static class AndroidKeyCode
    {
        /// <summary>
        /// Key code constant: Unknown key code.
        /// </summary>
        public const int KEYCODE_UNKNOWN = 0;

        /// <summary>
        /// Key code constant: Soft Left key.
        /// Usually situated below the display on phones and used as a multi-function
        /// feature key for selecting a software defined function shown on the bottom left
        /// of the display.
        /// </summary>
        public const int KEYCODE_SOFT_LEFT = 1;

        /// <summary>
        /// Key code constant: Soft Right key.
        /// Usually situated below the display on phones and used as a multi-function
        /// feature key for selecting a software defined function shown on the bottom right
        /// of the display.
        /// </summary>
        public const int KEYCODE_SOFT_RIGHT = 2;

        /// <summary>
        /// Key code constant: Home key.
        /// This key is handled by the framework and is never delivered to applications.
        /// </summary>
        public const int KEYCODE_HOME = 3;

        /// <summary>
        /// Key code constant: Back key.
        /// </summary>
        public const int KEYCODE_BACK = 4;

        /// <summary>
        /// Key code constant: Call key.
        /// </summary>
        public const int KEYCODE_CALL = 5;

        /// <summary>
        /// Key code constant: End Call key.
        /// </summary>
        public const int KEYCODE_ENDCALL = 6;

        /// <summary>
        /// Key code constant: '0' key.
        /// </summary>
        public const int KEYCODE_0 = 7;

        /// <summary>
        /// Key code constant: '1' key.
        /// </summary>
        public const int KEYCODE_1 = 8;

        /// <summary>
        /// Key code constant: '2' key.
        /// </summary>
        public const int KEYCODE_2 = 9;

        /// <summary>
        /// Key code constant: '3' key.
        /// </summary>
        public const int KEYCODE_3 = 10;

        /// <summary>
        /// Key code constant: '4' key.
        /// </summary>
        public const int KEYCODE_4 = 11;

        /// <summary>
        /// Key code constant: '5' key.
        /// </summary>
        public const int KEYCODE_5 = 12;

        /// <summary>
        /// Key code constant: '6' key.
        /// </summary>
        public const int KEYCODE_6 = 13;

        /// <summary>
        /// Key code constant: '7' key.
        /// </summary>
        public const int KEYCODE_7 = 14;

        /// <summary>
        /// Key code constant: '8' key.
        /// </summary>
        public const int KEYCODE_8 = 15;

        /// <summary>
        /// Key code constant: '9' key.
        /// </summary>
        public const int KEYCODE_9 = 16;

        /// <summary>
        /// Key code constant: '*' key.
        /// </summary>
        public const int KEYCODE_STAR = 17;

        /// <summary>
        /// Key code constant: '#' key.
        /// </summary>
        public const int KEYCODE_POUND = 18;

        /// <summary>
        /// Key code constant: Directional Pad Up key.
        /// May also be synthesized from trackball motions.
        /// </summary>
        public const int KEYCODE_DPAD_UP = 19;

        /// <summary>
        /// Key code constant: Directional Pad Down key.
        /// May also be synthesized from trackball motions.
        /// </summary>
        public const int KEYCODE_DPAD_DOWN = 20;

        /// <summary>
        /// Key code constant: Directional Pad Left key.
        /// May also be synthesized from trackball motions.
        /// </summary>
        public const int KEYCODE_DPAD_LEFT = 21;

        /// <summary>
        /// Key code constant: Directional Pad Right key.
        /// May also be synthesized from trackball motions.
        /// </summary>
        public const int KEYCODE_DPAD_RIGHT = 22;

        /// <summary>
        /// Key code constant: Directional Pad Center key.
        /// May also be synthesized from trackball motions.
        /// </summary>
        public const int KEYCODE_DPAD_CENTER = 23;

        /// <summary>
        /// Key code constant: Volume Up key.
        /// Adjusts the speaker volume up.
        /// </summary>
        public const int KEYCODE_VOLUME_UP = 24;

        /// <summary>
        /// Key code constant: Volume Down key.
        /// Adjusts the speaker volume down.
        /// </summary>
        public const int KEYCODE_VOLUME_DOWN = 25;

        /// <summary>
        /// Key code constant: Power key.
        /// </summary>
        public const int KEYCODE_POWER = 26;

        /// <summary>
        /// Key code constant: Camera key.
        /// Used to launch a camera application or take pictures.
        /// </summary>
        public const int KEYCODE_CAMERA = 27;

        /// <summary>
        /// Key code constant: Clear key.
        /// </summary>
        public const int KEYCODE_CLEAR = 28;

        /// <summary>
        /// Key code constant: 'A' key.
        /// </summary>
        public const int KEYCODE_A = 29;

        /// <summary>
        /// Key code constant: 'B' key.
        /// </summary>
        public const int KEYCODE_B = 30;

        /// <summary>
        /// Key code constant: 'C' key.
        /// </summary>
        public const int KEYCODE_C = 31;

        /// <summary>
        /// Key code constant: 'D' key.
        /// </summary>
        public const int KEYCODE_D = 32;

        /// <summary>
        /// Key code constant: 'E' key.
        /// </summary>
        public const int KEYCODE_E = 33;

        /// <summary>
        /// Key code constant: 'F' key.
        /// </summary>
        public const int KEYCODE_F = 34;

        /// <summary>
        /// Key code constant: 'G' key.
        /// </summary>
        public const int KEYCODE_G = 35;

        /// <summary>
        /// Key code constant: 'H' key.
        /// </summary>
        public const int KEYCODE_H = 36;

        /// <summary>
        /// Key code constant: 'I' key.
        /// </summary>
        public const int KEYCODE_I = 37;

        /// <summary>
        /// Key code constant: 'J' key.
        /// </summary>
        public const int KEYCODE_J = 38;

        /// <summary>
        /// Key code constant: 'K' key.
        /// </summary>
        public const int KEYCODE_K = 39;

        /// <summary>
        /// Key code constant: 'L' key.
        /// </summary>
        public const int KEYCODE_L = 40;

        /// <summary>
        /// Key code constant: 'M' key.
        /// </summary>
        public const int KEYCODE_M = 41;

        /// <summary>
        /// Key code constant: 'N' key.
        /// </summary>
        public const int KEYCODE_N = 42;

        /// <summary>
        /// Key code constant: 'O' key.
        /// </summary>
        public const int KEYCODE_O = 43;

        /// <summary>
        /// Key code constant: 'P' key.
        /// </summary>
        public const int KEYCODE_P = 44;

        /// <summary>
        /// Key code constant: 'Q' key.
        /// </summary>
        public const int KEYCODE_Q = 45;

        /// <summary>
        /// Key code constant: 'R' key.
        /// </summary>
        public const int KEYCODE_R = 46;

        /// <summary>
        /// Key code constant: 'S' key.
        /// </summary>
        public const int KEYCODE_S = 47;

        /// <summary>
        /// Key code constant: 'T' key.
        /// </summary>
        public const int KEYCODE_T = 48;

        /// <summary>
        /// Key code constant: 'U' key.
        /// </summary>
        public const int KEYCODE_U = 49;

        /// <summary>
        /// Key code constant: 'V' key.
        /// </summary>
        public const int KEYCODE_V = 50;

        /// <summary>
        /// Key code constant: 'W' key.
        /// </summary>
        public const int KEYCODE_W = 51;

        /// <summary>
        /// Key code constant: 'X' key.
        /// </summary>
        public const int KEYCODE_X = 52;

        /// <summary>
        /// Key code constant: 'Y' key.
        /// </summary>
        public const int KEYCODE_Y = 53;

        /// <summary>
        /// Key code constant: 'Z' key.
        /// </summary>
        public const int KEYCODE_Z = 54;

        /// <summary>
        /// Key code constant: ',' key.
        /// </summary>
        public const int KEYCODE_COMMA = 55;

        /// <summary>
        /// Key code constant: '.' key.
        /// </summary>
        public const int KEYCODE_PERIOD = 56;

        /// <summary>
        /// Key code constant: Left Alt modifier key.
        /// </summary>
        public const int KEYCODE_ALT_LEFT = 57;

        /// <summary>
        /// Key code constant: Right Alt modifier key.
        /// </summary>
        public const int KEYCODE_ALT_RIGHT = 58;

        /// <summary>
        /// Key code constant: Left Shift modifier key.
        /// </summary>
        public const int KEYCODE_SHIFT_LEFT = 59;

        /// <summary>
        /// Key code constant: Right Shift modifier key.
        /// </summary>
        public const int KEYCODE_SHIFT_RIGHT = 60;

        /// <summary>
        /// Key code constant: Tab key.
        /// </summary>
        public const int KEYCODE_TAB = 61;

        /// <summary>
        /// Key code constant: Space key.
        /// </summary>
        public const int KEYCODE_SPACE = 62;

        /// <summary>
        /// Key code constant: Symbol modifier key.
        /// Used to enter alternate symbols.
        /// </summary>
        public const int KEYCODE_SYM = 63;

        /// <summary>
        /// Key code constant: Explorer special function key.
        /// Used to launch a browser application.
        /// </summary>
        public const int KEYCODE_EXPLORER = 64;

        /// <summary>
        /// Key code constant: Envelope special function key.
        /// Used to launch a mail application.
        /// </summary>
        public const int KEYCODE_ENVELOPE = 65;

        /// <summary>
        /// Key code constant: Enter key.
        /// </summary>
        public const int KEYCODE_ENTER = 66;

        /// <summary>
        /// Key code constant: Backspace key.
        /// Deletes characters before the insertion point, unlike KEYCODE_FORWARD_DEL.
        /// </summary>
        public const int KEYCODE_DEL = 67;

        /// <summary>
        /// Key code constant: '`' (backtick) key.
        /// </summary>
        public const int KEYCODE_GRAVE = 68;

        /// <summary>
        /// Key code constant: '-'.
        /// </summary>
        public const int KEYCODE_MINUS = 69;

        /// <summary>
        /// Key code constant: '=' key.
        /// </summary>
        public const int KEYCODE_EQUALS = 70;

        /// <summary>
        /// Key code constant: '[' key.
        /// </summary>
        public const int KEYCODE_LEFT_BRACKET = 71;

        /// <summary>
        /// Key code constant: ']' key.
        /// </summary>
        public const int KEYCODE_RIGHT_BRACKET = 72;

        /// <summary>
        /// Key code constant: '\' key.
        /// </summary>
        public const int KEYCODE_BACKSLASH = 73;

        /// <summary>
        /// Key code constant: ';' key.
        /// </summary>
        public const int KEYCODE_SEMICOLON = 74;

        /// <summary>
        /// Key code constant: ''' (apostrophe) key.
        /// </summary>
        public const int KEYCODE_APOSTROPHE = 75;

        /// <summary>
        /// Key code constant: '/' key.
        /// </summary>
        public const int KEYCODE_SLASH = 76;

        /// <summary>
        /// Key code constant: '@' key.
        /// </summary>
        public const int KEYCODE_AT = 77;

        /// <summary>
        /// Key code constant: Number modifier key.
        /// Used to enter numeric symbols.
        /// This key is not Num Lock; it is more like KEYCODE_ALT_LEFT and is
        /// interpreted as an ALT key by android.text.method.MetaKeyKeyListener.
        /// </summary>
        public const int KEYCODE_NUM = 78;

        /// <summary>
        /// Key code constant: Headset Hook key.
        /// Used to hang up calls and stop media.
        /// </summary>
        public const int KEYCODE_HEADSETHOOK = 79;

        /// <summary>
        /// Key code constant: Camera Focus key.
        /// Used to focus the camera.
        /// </summary>
        public const int KEYCODE_FOCUS = 80; // *Camera* focus

        /// <summary>
        /// Key code constant: '+' key.
        /// </summary>
        public const int KEYCODE_PLUS = 81;

        /// <summary>
        /// Key code constant: Menu key.
        /// </summary>
        public const int KEYCODE_MENU = 82;

        /// <summary>
        /// Key code constant: Notification key.
        /// </summary>
        public const int KEYCODE_NOTIFICATION = 83;

        /// <summary>
        /// Key code constant: Search key.
        /// </summary>
        public const int KEYCODE_SEARCH = 84;

        /// <summary>
        /// Key code constant: Play/Pause media key.
        /// </summary>
        public const int KEYCODE_MEDIA_PLAY_PAUSE = 85;

        /// <summary>
        /// Key code constant: Stop media key.
        /// </summary>
        public const int KEYCODE_MEDIA_STOP = 86;

        /// <summary>
        /// Key code constant: Play Next media key.
        /// </summary>
        public const int KEYCODE_MEDIA_NEXT = 87;

        /// <summary>
        /// Key code constant: Play Previous media key.
        /// </summary>
        public const int KEYCODE_MEDIA_PREVIOUS = 88;

        /// <summary>
        /// Key code constant: Rewind media key.
        /// </summary>
        public const int KEYCODE_MEDIA_REWIND = 89;

        /// <summary>
        /// Key code constant: Fast Forward media key.
        /// </summary>
        public const int KEYCODE_MEDIA_FAST_FORWARD = 90;

        /// <summary>
        /// Key code constant: Mute key.
        /// Mutes the microphone, unlike KEYCODE_VOLUME_MUTE.
        /// </summary>
        public const int KEYCODE_MUTE = 91;

        /// <summary>
        /// Key code constant: Page Up key.
        /// </summary>
        public const int KEYCODE_PAGE_UP = 92;

        /// <summary>
        /// Key code constant: Page Down key.
        /// </summary>
        public const int KEYCODE_PAGE_DOWN = 93;

        /// <summary>
        /// Key code constant: Picture Symbols modifier key.
        /// Used to switch symbol sets (Emoji, Kao-moji).
        /// </summary>
        public const int KEYCODE_PICTSYMBOLS = 94; // switch symbol-sets (Emoji,Kao-moji)

        /// <summary>
        /// Key code constant: Switch Charset modifier key.
        /// Used to switch character sets (Kanji, Katakana).
        /// </summary>
        public const int KEYCODE_SWITCH_CHARSET = 95; // switch char-sets (Kanji,Katakana)

        /// <summary>
        /// Key code constant: A Button key.
        /// On a game controller, the A button should be either the button labeled A
        /// or the first button on the bottom row of controller buttons.
        /// </summary>
        public const int KEYCODE_BUTTON_A = 96;

        /// <summary>
        /// Key code constant: B Button key.
        /// On a game controller, the B button should be either the button labeled B
        /// or the second button on the bottom row of controller buttons.
        /// </summary>
        public const int KEYCODE_BUTTON_B = 97;

        /// <summary>
        /// Key code constant: C Button key.
        /// On a game controller, the C button should be either the button labeled C
        /// or the third button on the bottom row of controller buttons.
        /// </summary>
        public const int KEYCODE_BUTTON_C = 98;

        /// <summary>
        /// Key code constant: X Button key.
        /// On a game controller, the X button should be either the button labeled X
        /// or the first button on the upper row of controller buttons.
        /// </summary>
        public const int KEYCODE_BUTTON_X = 99;

        /// <summary>
        /// Key code constant: Y Button key.
        /// On a game controller, the Y button should be either the button labeled Y
        /// or the second button on the upper row of controller buttons.
        /// </summary>
        public const int KEYCODE_BUTTON_Y = 100;

        /// <summary>
        /// Key code constant: Z Button key.
        /// On a game controller, the Z button should be either the button labeled Z
        /// or the third button on the upper row of controller buttons.
        /// </summary>
        public const int KEYCODE_BUTTON_Z = 101;

        /// <summary>
        /// Key code constant: L1 Button key.
        /// On a game controller, the L1 button should be either the button labeled L1 (or L)
        /// or the top left trigger button.
        /// </summary>
        public const int KEYCODE_BUTTON_L1 = 102;

        /// <summary>
        /// Key code constant: R1 Button key.
        /// On a game controller, the R1 button should be either the button labeled R1 (or R)
        /// or the top right trigger button.
        /// </summary>
        public const int KEYCODE_BUTTON_R1 = 103;

        /// <summary>
        /// Key code constant: L2 Button key.
        /// On a game controller, the L2 button should be either the button labeled L2
        /// or the bottom left trigger button.
        /// </summary>
        public const int KEYCODE_BUTTON_L2 = 104;

        /// <summary>
        /// Key code constant: R2 Button key.
        /// On a game controller, the R2 button should be either the button labeled R2
        /// or the bottom right trigger button.
        /// </summary>
        public const int KEYCODE_BUTTON_R2 = 105;

        /// <summary>
        /// Key code constant: Left Thumb Button key.
        /// On a game controller, the left thumb button indicates that the left (or only)
        /// joystick is pressed.
        /// </summary>
        public const int KEYCODE_BUTTON_THUMBL = 106;

        /// <summary>
        /// Key code constant: Right Thumb Button key.
        /// On a game controller, the right thumb button indicates that the right
        /// joystick is pressed.
        /// </summary>
        public const int KEYCODE_BUTTON_THUMBR = 107;

        /// <summary>
        /// Key code constant: Start Button key.
        /// On a game controller, the button labeled Start.
        /// </summary>
        public const int KEYCODE_BUTTON_START = 108;

        /// <summary>
        /// Key code constant: Select Button key.
        /// On a game controller, the button labeled Select.
        /// </summary>
        public const int KEYCODE_BUTTON_SELECT = 109;

        /// <summary>
        /// Key code constant: Mode Button key.
        /// On a game controller, the button labeled Mode.
        /// </summary>
        public const int KEYCODE_BUTTON_MODE = 110;

        /// <summary>
        /// Key code constant: Escape key.
        /// </summary>
        public const int KEYCODE_ESCAPE = 111;

        /// <summary>
        /// Key code constant: Forward Delete key.
        /// Deletes characters ahead of the insertion point, unlike KEYCODE_DEL.
        /// </summary>
        public const int KEYCODE_FORWARD_DEL = 112;

        /// <summary>
        /// Key code constant: Left Control modifier key.
        /// </summary>
        public const int KEYCODE_CTRL_LEFT = 113;

        /// <summary>
        /// Key code constant: Right Control modifier key.
        /// </summary>
        public const int KEYCODE_CTRL_RIGHT = 114;

        /// <summary>
        /// Key code constant: Caps Lock key.
        /// </summary>
        public const int KEYCODE_CAPS_LOCK = 115;

        /// <summary>
        /// Key code constant: Scroll Lock key.
        /// </summary>
        public const int KEYCODE_SCROLL_LOCK = 116;

        /// <summary>
        /// Key code constant: Left Meta modifier key.
        /// </summary>
        public const int KEYCODE_META_LEFT = 117;

        /// <summary>
        /// Key code constant: Right Meta modifier key.
        /// </summary>
        public const int KEYCODE_META_RIGHT = 118;

        /// <summary>
        /// Key code constant: Function modifier key.
        /// </summary>
        public const int KEYCODE_FUNCTION = 119;

        /// <summary>
        /// Key code constant: System Request / Print Screen key.
        /// </summary>
        public const int KEYCODE_SYSRQ = 120;

        /// <summary>
        /// Key code constant: Break / Pause key.
        /// </summary>
        public const int KEYCODE_BREAK = 121;

        /// <summary>
        /// Key code constant: Home Movement key.
        /// Used for scrolling or moving the cursor around to the start of a line
        /// or to the top of a list.
        /// </summary>
        public const int KEYCODE_MOVE_HOME = 122;

        /// <summary>
        /// Key code constant: End Movement key.
        /// Used for scrolling or moving the cursor around to the end of a line
        /// or to the bottom of a list.
        /// </summary>
        public const int KEYCODE_MOVE_END = 123;

        /// <summary>
        /// Key code constant: Insert key.
        /// Toggles insert / overwrite edit mode.
        /// </summary>
        public const int KEYCODE_INSERT = 124;

        /// <summary>
        /// Key code constant: Forward key.
        /// Navigates forward in the history stack.  Complement of KEYCODE_BACK.
        /// </summary>
        public const int KEYCODE_FORWARD = 125;

        /// <summary>
        /// Key code constant: Play media key.
        /// </summary>
        public const int KEYCODE_MEDIA_PLAY = 126;

        /// <summary>
        /// Key code constant: Pause media key.
        /// </summary>
        public const int KEYCODE_MEDIA_PAUSE = 127;

        /// <summary>
        /// Key code constant: Close media key.
        /// May be used to close a CD tray, for example.
        /// </summary>
        public const int KEYCODE_MEDIA_CLOSE = 128;

        /// <summary>
        /// Key code constant: Eject media key.
        /// May be used to eject a CD tray, for example.
        /// </summary>
        public const int KEYCODE_MEDIA_EJECT = 129;

        /// <summary>
        /// Key code constant: Record media key.
        /// </summary>
        public const int KEYCODE_MEDIA_RECORD = 130;

        /// <summary>
        /// Key code constant: F1 key.
        /// </summary>
        public const int KEYCODE_F1 = 131;

        /// <summary>
        /// Key code constant: F2 key.
        /// </summary>
        public const int KEYCODE_F2 = 132;

        /// <summary>
        /// Key code constant: F3 key.
        /// </summary>
        public const int KEYCODE_F3 = 133;

        /// <summary>
        /// Key code constant: F4 key.
        /// </summary>
        public const int KEYCODE_F4 = 134;

        /// <summary>
        /// Key code constant: F5 key.
        /// </summary>
        public const int KEYCODE_F5 = 135;

        /// <summary>
        /// Key code constant: F6 key.
        /// </summary>
        public const int KEYCODE_F6 = 136;

        /// <summary>
        /// Key code constant: F7 key.
        /// </summary>
        public const int KEYCODE_F7 = 137;

        /// <summary>
        /// Key code constant: F8 key.
        /// </summary>
        public const int KEYCODE_F8 = 138;

        /// <summary>
        /// Key code constant: F9 key.
        /// </summary>
        public const int KEYCODE_F9 = 139;

        /// <summary>
        /// Key code constant: F10 key.
        /// </summary>
        public const int KEYCODE_F10 = 140;

        /// <summary>
        /// Key code constant: F11 key.
        /// </summary>
        public const int KEYCODE_F11 = 141;

        /// <summary>
        /// Key code constant: F12 key.
        /// </summary>
        public const int KEYCODE_F12 = 142;

        /// <summary>
        /// Key code constant: Num Lock key.
        /// This is the Num Lock key; it is different from KEYCODE_NUM.
        /// This key alters the behavior of other keys on the numeric keypad.
        /// </summary>
        public const int KEYCODE_NUM_LOCK = 143;

        /// <summary>
        /// Key code constant: Numeric keypad '0' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_0 = 144;

        /// <summary>
        /// Key code constant: Numeric keypad '1' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_1 = 145;

        /// <summary>
        /// Key code constant: Numeric keypad '2' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_2 = 146;

        /// <summary>
        /// Key code constant: Numeric keypad '3' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_3 = 147;

        /// <summary>
        /// Key code constant: Numeric keypad '4' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_4 = 148;

        /// <summary>
        /// Key code constant: Numeric keypad '5' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_5 = 149;

        /// <summary>
        /// Key code constant: Numeric keypad '6' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_6 = 150;

        /// <summary>
        /// Key code constant: Numeric keypad '7' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_7 = 151;

        /// <summary>
        /// Key code constant: Numeric keypad '8' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_8 = 152;

        /// <summary>
        /// Key code constant: Numeric keypad '9' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_9 = 153;

        /// <summary>
        /// Key code constant: Numeric keypad '/' key (for division).
        /// </summary>
        public const int KEYCODE_NUMPAD_DIVIDE = 154;

        /// <summary>
        /// Key code constant: Numeric keypad '*' key (for multiplication).
        /// </summary>
        public const int KEYCODE_NUMPAD_MULTIPLY = 155;

        /// <summary>
        /// Key code constant: Numeric keypad '-' key (for subtraction).
        /// </summary>
        public const int KEYCODE_NUMPAD_SUBTRACT = 156;

        /// <summary>
        /// Key code constant: Numeric keypad '+' key (for addition).
        /// </summary>
        public const int KEYCODE_NUMPAD_ADD = 157;

        /// <summary>
        /// Key code constant: Numeric keypad '.' key (for decimals or digit grouping).
        /// </summary>
        public const int KEYCODE_NUMPAD_DOT = 158;

        /// <summary>
        /// Key code constant: Numeric keypad ',' key (for decimals or digit grouping).
        /// </summary>
        public const int KEYCODE_NUMPAD_COMMA = 159;

        /// <summary>
        /// Key code constant: Numeric keypad Enter key.
        /// </summary>
        public const int KEYCODE_NUMPAD_ENTER = 160;

        /// <summary>
        /// Key code constant: Numeric keypad '=' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_EQUALS = 161;

        /// <summary>
        /// Key code constant: Numeric keypad '(' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_LEFT_PAREN = 162;

        /// <summary>
        /// Key code constant: Numeric keypad ')' key.
        /// </summary>
        public const int KEYCODE_NUMPAD_RIGHT_PAREN = 163;

        /// <summary>
        /// Key code constant: Volume Mute key.
        /// Mutes the speaker, unlike KEYCODE_MUTE.
        /// This key should normally be implemented as a toggle such that the first press
        /// mutes the speaker and the second press restores the original volume.
        /// </summary>
        public const int KEYCODE_VOLUME_MUTE = 164;

        /// <summary>
        /// Key code constant: Info key.
        /// Common on TV remotes to show additional information related to what is
        /// currently being viewed.
        /// </summary>
        public const int KEYCODE_INFO = 165;

        /// <summary>
        /// Key code constant: Channel up key.
        /// On TV remotes, increments the television channel.
        /// </summary>
        public const int KEYCODE_CHANNEL_UP = 166;

        /// <summary>
        /// Key code constant: Channel down key.
        /// On TV remotes, decrements the television channel.
        /// </summary>
        public const int KEYCODE_CHANNEL_DOWN = 167;

        /// <summary>
        /// Key code constant: Zoom in key.
        /// </summary>
        public const int KEYCODE_ZOOM_IN = 168;

        /// <summary>
        /// Key code constant: Zoom out key.
        /// </summary>
        public const int KEYCODE_ZOOM_OUT = 169;

        /// <summary>
        /// Key code constant: TV key.
        /// On TV remotes, switches to viewing live TV.
        /// </summary>
        public const int KEYCODE_TV = 170;

        /// <summary>
        /// Key code constant: Window key.
        /// On TV remotes, toggles picture-in-picture mode or other windowing functions.
        /// On Android Wear devices, triggers a display offset.
        /// </summary>
        public const int KEYCODE_WINDOW = 171;

        /// <summary>
        /// Key code constant: Guide key.
        /// On TV remotes, shows a programming guide.
        /// </summary>
        public const int KEYCODE_GUIDE = 172;

        /// <summary>
        /// Key code constant: DVR key.
        /// On some TV remotes, switches to a DVR mode for recorded shows.
        /// </summary>
        public const int KEYCODE_DVR = 173;

        /// <summary>
        /// Key code constant: Bookmark key.
        /// On some TV remotes, bookmarks content or web pages.
        /// </summary>
        public const int KEYCODE_BOOKMARK = 174;

        /// <summary>
        /// Key code constant: Toggle captions key.
        /// Switches the mode for closed-captioning text, for example during television shows.
        /// </summary>
        public const int KEYCODE_CAPTIONS = 175;

        /// <summary>
        /// Key code constant: Settings key.
        /// Starts the system settings activity.
        /// </summary>
        public const int KEYCODE_SETTINGS = 176;

        /// <summary>
        /// Key code constant: TV power key.
        /// On TV remotes, toggles the power on a television screen.
        /// </summary>
        public const int KEYCODE_TV_POWER = 177;

        /// <summary>
        /// Key code constant: TV input key.
        /// On TV remotes, switches the input on a television screen.
        /// </summary>
        public const int KEYCODE_TV_INPUT = 178;

        /// <summary>
        /// Key code constant: Set-top-box power key.
        /// On TV remotes, toggles the power on an external Set-top-box.
        /// </summary>
        public const int KEYCODE_STB_POWER = 179;

        /// <summary>
        /// Key code constant: Set-top-box input key.
        /// On TV remotes, switches the input mode on an external Set-top-box.
        /// </summary>
        public const int KEYCODE_STB_INPUT = 180;

        /// <summary>
        /// Key code constant: A/V Receiver power key.
        /// On TV remotes, toggles the power on an external A/V Receiver.
        /// </summary>
        public const int KEYCODE_AVR_POWER = 181;

        /// <summary>
        /// Key code constant: A/V Receiver input key.
        /// On TV remotes, switches the input mode on an external A/V Receiver.
        /// </summary>
        public const int KEYCODE_AVR_INPUT = 182;

        /// <summary>
        /// Key code constant: Red "programmable" key.
        /// On TV remotes, acts as a contextual/programmable key.
        /// </summary>
        public const int KEYCODE_PROG_RED = 183;

        /// <summary>
        /// Key code constant: Green "programmable" key.
        /// On TV remotes, actsas a contextual/programmable key.
        /// </summary>
        public const int KEYCODE_PROG_GREEN = 184;

        /// <summary>
        /// Key code constant: Yellow "programmable" key.
        /// On TV remotes, acts as a contextual/programmable key.
        /// </summary>
        public const int KEYCODE_PROG_YELLOW = 185;

        /// <summary>
        /// Key code constant: Blue "programmable" key.
        /// On TV remotes, acts as a contextual/programmable key.
        /// </summary>
        public const int KEYCODE_PROG_BLUE = 186;

        /// <summary>
        /// Key code constant: App switch key.
        /// Should bring up the application switcher dialog.
        /// </summary>
        public const int KEYCODE_APP_SWITCH = 187;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #1.
        /// </summary>
        public const int KEYCODE_BUTTON_1 = 188;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #2.
        /// </summary>
        public const int KEYCODE_BUTTON_2 = 189;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #3.
        /// </summary>
        public const int KEYCODE_BUTTON_3 = 190;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #4.
        /// </summary>
        public const int KEYCODE_BUTTON_4 = 191;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #5.
        /// </summary>
        public const int KEYCODE_BUTTON_5 = 192;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #6.
        /// </summary>
        public const int KEYCODE_BUTTON_6 = 193;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #7.
        /// </summary>
        public const int KEYCODE_BUTTON_7 = 194;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #8.
        /// </summary>
        public const int KEYCODE_BUTTON_8 = 195;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #9.
        /// </summary>
        public const int KEYCODE_BUTTON_9 = 196;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #10.
        /// </summary>
        public const int KEYCODE_BUTTON_10 = 197;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #11.
        /// </summary>
        public const int KEYCODE_BUTTON_11 = 198;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #12.
        /// </summary>
        public const int KEYCODE_BUTTON_12 = 199;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #13.
        /// </summary>
        public const int KEYCODE_BUTTON_13 = 200;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #14.
        /// </summary>
        public const int KEYCODE_BUTTON_14 = 201;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #15.
        /// </summary>
        public const int KEYCODE_BUTTON_15 = 202;

        /// <summary>
        /// Key code constant: Generic Game Pad Button #16.
        /// </summary>
        public const int KEYCODE_BUTTON_16 = 203;

        /// <summary>
        /// Key code constant: Language Switch key.
        /// Toggles the current input language such as switching between English and Japanese on
        /// a QWERTY keyboard.  On some devices, the same function may be performed by
        /// pressing Shift+Spacebar.
        /// </summary>
        public const int KEYCODE_LANGUAGE_SWITCH = 204;

        /// <summary>
        /// Key code constant: Manner Mode key.
        /// Toggles silent or vibrate mode on and off to make the device behave more politely
        /// in certain settings such as on a crowded train.  On some devices, the key may only
        /// operate when long-pressed.
        /// </summary>
        public const int KEYCODE_MANNER_MODE = 205;

        /// <summary>
        /// Key code constant: 3D Mode key.
        /// Toggles the display between 2D and 3D mode.
        /// </summary>
        public const int KEYCODE_3D_MODE = 206;

        /// <summary>
        /// Key code constant: Contacts special function key.
        /// Used to launch an address book application.
        /// </summary>
        public const int KEYCODE_CONTACTS = 207;

        /// <summary>
        /// Key code constant: Calendar special function key.
        /// Used to launch a calendar application.
        /// </summary>
        public const int KEYCODE_CALENDAR = 208;

        /// <summary>
        /// Key code constant: Music special function key.
        /// Used to launch a music player application.
        /// </summary>
        public const int KEYCODE_MUSIC = 209;

        /// <summary>
        /// Key code constant: Calculator special function key.
        /// Used to launch a calculator application.
        /// </summary>
        public const int KEYCODE_CALCULATOR = 210;

        /// <summary>
        /// Key code constant: Japanese full-width / half-width key.
        /// </summary>
        public const int KEYCODE_ZENKAKU_HANKAKU = 211;

        /// <summary>
        /// Key code constant: Japanese alphanumeric key.
        /// </summary>
        public const int KEYCODE_EISU = 212;

        /// <summary>
        /// Key code constant: Japanese non-conversion key.
        /// </summary>
        public const int KEYCODE_MUHENKAN = 213;

        /// <summary>
        /// Key code constant: Japanese conversion key.
        /// </summary>
        public const int KEYCODE_HENKAN = 214;

        /// <summary>
        /// Key code constant: Japanese katakana / hiragana key.
        /// </summary>
        public const int KEYCODE_KATAKANA_HIRAGANA = 215;

        /// <summary>
        /// Key code constant: Japanese Yen key.
        /// </summary>
        public const int KEYCODE_YEN = 216;

        /// <summary>
        /// Key code constant: Japanese Ro key.
        /// </summary>
        public const int KEYCODE_RO = 217;

        /// <summary>
        /// Key code constant: Japanese kana key.
        /// </summary>
        public const int KEYCODE_KANA = 218;

        /// <summary>
        /// Key code constant: Assist key.
        /// Launches the global assist activity.  Not delivered to applications.
        /// </summary>
        public const int KEYCODE_ASSIST = 219;

        /// <summary>
        /// Key code constant: Brightness Down key.
        /// Adjusts the screen brightness down.
        /// </summary>
        public const int KEYCODE_BRIGHTNESS_DOWN = 220;

        /// <summary>
        /// Key code constant: Brightness Up key.
        /// Adjusts the screen brightness up.
        /// </summary>
        public const int KEYCODE_BRIGHTNESS_UP = 221;

        /// <summary>
        /// Key code constant: Audio Track key.
        /// Switches the audio tracks.
        /// </summary>
        public const int KEYCODE_MEDIA_AUDIO_TRACK = 222;

        /// <summary>
        /// Key code constant: Sleep key.
        /// Puts the device to sleep.  Behaves somewhat like KEYCODE_POWER but it
        /// has no effect if the device is already asleep.
        /// </summary>
        public const int KEYCODE_SLEEP = 223;

        /// <summary>
        /// Key code constant: Wakeup key.
        /// Wakes up the device.  Behaves somewhat like KEYCODE_POWER but it
        /// has no effect if the device is already awake.
        /// </summary>
        public const int KEYCODE_WAKEUP = 224;

        /// <summary>
        /// Key code constant: Pairing key.
        /// Initiates peripheral pairing mode. Useful for pairing remote control
        /// devices or game controllers, especially if no other input mode is
        /// available.
        /// </summary>
        public const int KEYCODE_PAIRING = 225;

        /// <summary>
        /// Key code constant: Media Top Menu key.
        /// Goes to the top of media menu.
        /// </summary>
        public const int KEYCODE_MEDIA_TOP_MENU = 226;

        /// <summary>
        /// Key code constant: '11' key.
        /// </summary>
        public const int KEYCODE_11 = 227;

        /// <summary>
        /// Key code constant: '12' key.
        /// </summary>
        public const int KEYCODE_12 = 228;

        /// <summary>
        /// Key code constant: Last Channel key.
        /// Goes to the last viewed channel.
        /// </summary>
        public const int KEYCODE_LAST_CHANNEL = 229;

        /// <summary>
        /// Key code constant: TV data service key.
        /// Displays data services like weather, sports.
        /// </summary>
        public const int KEYCODE_TV_DATA_SERVICE = 230;

        /// <summary>
        /// Key code constant: Voice Assist key.
        /// Launches the global voice assist activity. Not delivered to applications.
        /// </summary>
        public const int KEYCODE_VOICE_ASSIST = 231;

        /// <summary>
        /// Key code constant: Radio key.
        /// Toggles TV service / Radio service.
        /// </summary>
        public const int KEYCODE_TV_RADIO_SERVICE = 232;

        /// <summary>
        /// Key code constant: Teletext key.
        /// Displays Teletext service.
        /// </summary>
        public const int KEYCODE_TV_TELETEXT = 233;

        /// <summary>
        /// Key code constant: Number entry key.
        /// Initiates to enter multi-digit channel nubmber when each digit key is assigned
        /// for selecting separate channel. Corresponds to Number Entry Mode (0x1D) of CEC
        /// User Control Code.
        /// </summary>
        public const int KEYCODE_TV_NUMBER_ENTRY = 234;

        /// <summary>
        /// Key code constant: Analog Terrestrial key.
        /// Switches to analog terrestrial broadcast service.
        /// </summary>
        public const int KEYCODE_TV_TERRESTRIAL_ANALOG = 235;

        /// <summary>
        /// Key code constant: Digital Terrestrial key.
        /// Switches to digital terrestrial broadcast service.
        /// </summary>
        public const int KEYCODE_TV_TERRESTRIAL_DIGITAL = 236;

        /// <summary>
        /// Key code constant: Satellite key.
        /// Switches to digital satellite broadcast service.
        /// </summary>
        public const int KEYCODE_TV_SATELLITE = 237;

        /// <summary>
        /// Key code constant: BS key.
        /// Switches to BS digital satellite broadcasting service available in Japan.
        /// </summary>
        public const int KEYCODE_TV_SATELLITE_BS = 238;

        /// <summary>
        /// Key code constant: CS key.
        /// Switches to CS digital satellite broadcasting service available in Japan.
        /// </summary>
        public const int KEYCODE_TV_SATELLITE_CS = 239;

        /// <summary>
        /// Key code constant: BS/CS key.
        /// Toggles between BS and CS digital satellite services.
        /// </summary>
        public const int KEYCODE_TV_SATELLITE_SERVICE = 240;

        /// <summary>
        /// Key code constant: Toggle Network key.
        /// Toggles selecting broacast services.
        /// </summary>
        public const int KEYCODE_TV_NETWORK = 241;

        /// <summary>
        /// Key code constant: Antenna/Cable key.
        /// Toggles broadcast input source between antenna and cable.
        /// </summary>
        public const int KEYCODE_TV_ANTENNA_CABLE = 242;

        /// <summary>
        /// Key code constant: HDMI #1 key.
        /// Switches to HDMI input #1.
        /// </summary>
        public const int KEYCODE_TV_INPUT_HDMI_1 = 243;

        /// <summary>
        /// Key code constant: HDMI #2 key.
        /// Switches to HDMI input #2.
        /// </summary>
        public const int KEYCODE_TV_INPUT_HDMI_2 = 244;

        /// <summary>
        /// Key code constant: HDMI #3 key.
        /// Switches to HDMI input #3.
        /// </summary>
        public const int KEYCODE_TV_INPUT_HDMI_3 = 245;

        /// <summary>
        /// Key code constant: HDMI #4 key.
        /// Switches to HDMI input #4.
        /// </summary>
        public const int KEYCODE_TV_INPUT_HDMI_4 = 246;

        /// <summary>
        /// Key code constant: Composite #1 key.
        /// Switches to composite video input #1.
        /// </summary>
        public const int KEYCODE_TV_INPUT_COMPOSITE_1 = 247;

        /// <summary>
        /// Key code constant: Composite #2 key.
        /// Switches to composite video input #2.
        /// </summary>
        public const int KEYCODE_TV_INPUT_COMPOSITE_2 = 248;

        /// <summary>
        /// Key code constant: Component #1 key.
        /// Switches to component video input #1.
        /// </summary>
        public const int KEYCODE_TV_INPUT_COMPONENT_1 = 249;

        /// <summary>
        /// Key code constant: Component #2 key.
        /// Switches to component video input #2.
        /// </summary>
        public const int KEYCODE_TV_INPUT_COMPONENT_2 = 250;

        /// <summary>
        /// Key code constant: VGA #1 key.
        /// Switches to VGA (analog RGB) input #1.
        /// </summary>
        public const int KEYCODE_TV_INPUT_VGA_1 = 251;

        /// <summary>
        /// Key code constant: Audio description key.
        /// Toggles audio description off / on.
        /// </summary>
        public const int KEYCODE_TV_AUDIO_DESCRIPTION = 252;

        /// <summary>
        /// Key code constant: Audio description mixing volume up key.
        /// Louden audio description volume as compared with normal audio volume.
        /// </summary>
        public const int KEYCODE_TV_AUDIO_DESCRIPTION_MIX_UP = 253;

        /// <summary>
        /// Key code constant: Audio description mixing volume down key.
        /// Lessen audio description volume as compared with normal audio volume.
        /// </summary>
        public const int KEYCODE_TV_AUDIO_DESCRIPTION_MIX_DOWN = 254;

        /// <summary>
        /// Key code constant: Zoom mode key.
        /// Changes Zoom mode (Normal, Full, Zoom, Wide-zoom, etc.)
        /// </summary>
        public const int KEYCODE_TV_ZOOM_MODE = 255;

        /// <summary>
        /// Key code constant: Contents menu key.
        /// Goes to the title list. Corresponds to Contents Menu (0x0B) of CEC User Control
        /// Code
        /// </summary>
        public const int KEYCODE_TV_CONTENTS_MENU = 256;

        /// <summary>
        /// Key code constant: Media context menu key.
        /// Goes to the context menu of media contents. Corresponds to Media Context-sensitive
        /// Menu (0x11) of CEC User Control Code.
        /// </summary>
        public const int KEYCODE_TV_MEDIA_CONTEXT_MENU = 257;

        /// <summary>
        /// Key code constant: Timer programming key.
        /// Goes to the timer recording menu. Corresponds to Timer Programming (0x54) of
        /// CEC User Control Code.
        /// </summary>
        public const int KEYCODE_TV_TIMER_PROGRAMMING = 258;

        /// <summary>
        /// Key code constant: Help key.
        /// </summary>
        public const int KEYCODE_HELP = 259;

        /// <summary>
        /// Key code constant: Navigate to previous key.
        /// Goes backward by one item in an ordered collection of items.
        /// </summary>
        public const int KEYCODE_NAVIGATE_PREVIOUS = 260;

        /// <summary>
        /// Key code constant: Navigate to next key.
        /// Advances to the next item in an ordered collection of items.
        /// </summary>
        public const int KEYCODE_NAVIGATE_NEXT = 261;

        /// <summary>
        /// Key code constant: Navigate in key.
        /// Activates the item that currently has focus or expands to the next level of a navigation
        /// hierarchy.
        /// </summary>
        public const int KEYCODE_NAVIGATE_IN = 262;

        /// <summary>
        /// Key code constant: Navigate out key.
        /// Backs out one level of a navigation hierarchy or collapses the item that currently has
        /// focus.
        /// </summary>
        public const int KEYCODE_NAVIGATE_OUT = 263;

        /// <summary>
        /// Key code constant: Primary stem key for Wear
        /// Main power/reset button on watch.
        /// </summary>
        public const int KEYCODE_STEM_PRIMARY = 264;

        /// <summary>
        /// Key code constant: Generic stem key 1 for Wear
        /// </summary>
        public const int KEYCODE_STEM_1 = 265;

        /// <summary>
        /// Key code constant: Generic stem key 2 for Wear
        /// </summary>
        public const int KEYCODE_STEM_2 = 266;

        /// <summary>
        /// Key code constant: Generic stem key 3 for Wear
        /// </summary>
        public const int KEYCODE_STEM_3 = 267;

        /// <summary>
        /// Key code constant: Directional Pad Up-Left
        /// </summary>
        public const int KEYCODE_DPAD_UP_LEFT = 268;

        /// <summary>
        /// Key code constant: Directional Pad Down-Left
        /// </summary>
        public const int KEYCODE_DPAD_DOWN_LEFT = 269;

        /// <summary>
        /// Key code constant: Directional Pad Up-Right
        /// </summary>
        public const int KEYCODE_DPAD_UP_RIGHT = 270;

        /// <summary>
        /// Key code constant: Directional Pad Down-Right
        /// </summary>
        public const int KEYCODE_DPAD_DOWN_RIGHT = 271;

        /// <summary>
        /// Key code constant: Skip forward media key.
        /// </summary>
        public const int KEYCODE_MEDIA_SKIP_FORWARD = 272;

        /// <summary>
        /// Key code constant: Skip backward media key.
        /// </summary>
        public const int KEYCODE_MEDIA_SKIP_BACKWARD = 273;

        /// <summary>
        /// Key code constant: Step forward media key.
        /// Steps media forward, one frame at a time.
        /// </summary>
        public const int KEYCODE_MEDIA_STEP_FORWARD = 274;

        /// <summary>
        /// Key code constant: Step backward media key.
        /// Steps media backward, one frame at a time.
        /// </summary>
        public const int KEYCODE_MEDIA_STEP_BACKWARD = 275;

        /// <summary>
        /// Key code constant: put device to sleep unless a wakelock is held.
        /// </summary>
        public const int KEYCODE_SOFT_SLEEP = 276;

        /// <summary>
        /// Key code constant: Cut key.
        /// </summary>
        public const int KEYCODE_CUT = 277;

        /// <summary>
        /// Key code constant: Copy key.
        /// </summary>
        public const int KEYCODE_COPY = 278;

        /// <summary>
        /// Key code constant: Paste key.
        /// </summary>
        public const int KEYCODE_PASTE = 279;

        /// <summary>
        /// Key code constant: Consumed by the system for navigation up
        /// </summary>
        public const int KEYCODE_SYSTEM_NAVIGATION_UP = 280;

        /// <summary>
        /// Key code constant: Consumed by the system for navigation down
        /// </summary>
        public const int KEYCODE_SYSTEM_NAVIGATION_DOWN = 281;

        /// <summary>
        /// Key code constant: Consumed by the system for navigation left
        /// </summary>
        public const int KEYCODE_SYSTEM_NAVIGATION_LEFT = 282;

        /// <summary>
        /// Key code constant: Consumed by the system for navigation right
        /// </summary>
        public const int KEYCODE_SYSTEM_NAVIGATION_RIGHT = 283;

        /// <summary>
        /// Key code constant: Show all apps
        /// </summary>
        public const int KEYCODE_ALL_APPS = 284;

        /// <summary>
        /// Key code constant: Refresh key.
        /// </summary>
        public const int KEYCODE_REFRESH = 285;

        /// <summary>
        /// Key code constant: Thumbs up key. Apps can use this to let user upvote content.
        /// </summary>
        public const int KEYCODE_THUMBS_UP = 286;

        /// <summary>
        /// Key code constant: Thumbs down key. Apps can use this to let user downvote content.
        /// </summary>
        public const int KEYCODE_THUMBS_DOWN = 287;

        /// <summary>
        /// Key code constant: Used to switch current android.accounts.Account that is
        /// consuming content. May be consumed by system to set account globally.
        /// </summary>
        public const int KEYCODE_PROFILE_SWITCH = 288;
    }
}