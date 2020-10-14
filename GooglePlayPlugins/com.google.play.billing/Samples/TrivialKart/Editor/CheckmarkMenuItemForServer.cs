// Copyright 2020 Google LLC
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

using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CheckmarkMenuItemForServer
{
    private const string MENU_NAME = "TrivialKart/BuildOptions/Build with server validation";

    private static bool _enabled;

    // InitializeOnLoad attribute means this is called on load
    static CheckmarkMenuItemForServer()
    {
        _enabled = EditorPrefs.GetBool(MENU_NAME, false);

        // Delaying until first editor tick so that the menu
        // will be populated before setting check state, and
        // re-apply correct action
        EditorApplication.delayCall += () => { PerformAction(_enabled); };
    }

    [MenuItem(MENU_NAME)]
    private static void ToggleAction()
    {
        // Toggling action
        PerformAction(!_enabled);
    }

    private static void PerformAction(bool enabled)
    {
        // Set checkmark on menu item
        Menu.SetChecked(MENU_NAME, enabled);
        // Saving editor state
        EditorPrefs.SetBool(MENU_NAME, enabled);

        _enabled = enabled;
        if (enabled)
        {
            Debug.Log(enabled);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "USE_SERVER");
        }
        else
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "CLIENT_ONLY");
        }
    }
}
