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

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the gameplay canvas.
/// It updates the coin text indicator in the gameplay canvas.
/// </summary>
public class GamePlayCanvasController : MonoBehaviour
{
    public Text coinsCount;

    private bool _canvasHasStarted = false;

    // Start is called before the first frame update
    private void Start()
    {
        SetBackgroundBasedOnGameData();
        SetCoinsBasedOnGameData();
        _canvasHasStarted = true;
        RefreshPage();
    }

    private void OnEnable()
    {
        // Refresh the page only when the canvas has started.
        if (_canvasHasStarted)
        {
            RefreshPage();
        }
    }

    private void RefreshPage()
    {
        SetCoinsBasedOnGameData();
    }

    private static void SetBackgroundBasedOnGameData()
    {
        GameDataController.GetGameData().SetBackgroundBasedOnGameData();
    }

    private void SetCoinsBasedOnGameData()
    {
        coinsCount.text = GameDataController.GetGameData().coinsOwned.ToString();
    }
}