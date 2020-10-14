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
/// Controller for 'Purchasing' wait model canvas
/// Displayed while waiting for results of purchase transaction
/// </summary>
public class WaitController : MonoBehaviour
{
    public Text purchasingText;

    private int _periodCount;
    private string _periodString;
    private float _timeElapsed;

    private void OnEnable()
    {
        _periodCount = 0;
        _periodString = "";
        _timeElapsed = 0.0f;
    }

    private void Update()
    {
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > 1.0f)
        {
            _timeElapsed -= Mathf.Floor(_timeElapsed);
            _periodCount += 1;
            if (_periodCount == 4 )
            {
                _periodCount = 0;
                _periodString = "";
            }
            else
            {
                _periodString += ".";
            }
            purchasingText.text = string.Format("Purchasing{0}", _periodString);
        }
    }
}