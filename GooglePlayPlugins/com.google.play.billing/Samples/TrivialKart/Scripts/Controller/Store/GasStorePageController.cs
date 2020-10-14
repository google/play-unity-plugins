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

using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the gas store page.
/// It listens to the fill gas button event. 
/// </summary>
public class GasStorePageController : MonoBehaviour
{
    public Text gasPrice;
    public Text panelGasPrice;
    public GameObject panelFillGas;
    public GameObject gasLevelImageObj;
    public GameObject cannotAffordWarning;
    public GameObject car;

    private double _currentCost;
    private Gas _gas;
    private Image _gasLevelImage;

    private void Awake()
    {
        _gas = car.GetComponent<Gas>();
        _gasLevelImage = gasLevelImageObj.GetComponent<Image>();
    }

    // Update the gas price and refresh the page when switching to the gas store page.
    private void OnEnable()
    {
        RefreshGasStorePage();
    }

    private void RefreshGasStorePage()
    {
        _currentCost = Math.Ceiling((Gas.FullGasLevel - _gas.GasLevel) * GameDataController.GetGameData().Discount);
        gasPrice.text = "* " + _currentCost;
        panelGasPrice.text = "Would you like to fill the gas tank with  " + _currentCost + "  coins";
        _gas.SetGasLevelHelper(_gasLevelImage, gasLevelImageObj);
        panelFillGas.SetActive(false);
        cannotAffordWarning.SetActive(false);
    }

    public void OnFillGasButtonClicked()
    {
        var currentCoins = GameDataController.GetGameData().CoinsOwned;
        if (currentCoins >= _currentCost)
        {
            panelFillGas.SetActive(true);
        }
        else
        {
            cannotAffordWarning.SetActive(true);
        }
    }

    public void OnFillGasConfirmPanelButtonClicked(bool isConfirmed)
    {
        panelFillGas.SetActive(false);

        // Fill the gas if the user taps "YES".
        if (isConfirmed)
        {
            GameDataController.GetGameData().ReduceCoinsOwned((int) _currentCost);
            _gas.FilledGas();
            RefreshGasStorePage();
        }
    }
}