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
/// Controller for coin store page.
/// It listens to coin purchase button events,
/// initializing the purchase flow when the coin purchase button is selected.
/// </summary>
public class CoinStorePageController : MonoBehaviour
{
    public GameObject confirmPanel;
    public Text confirmText;

    private CoinList.Coin _coinToPurchase;
    private bool _canvasHasStarted = false;

    private void Start()
    {
        _canvasHasStarted = true;
        confirmPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (_canvasHasStarted)
        {
            confirmPanel.SetActive(false);
        }
    }

    public void OnCoinStoreItemCLicked(int coinStoreItemIndex)
    {
        _coinToPurchase = CoinList.GetCoinByCoinIndex(coinStoreItemIndex);
        confirmPanel.SetActive(true);
        confirmText.text = "Would you like to purchase " + _coinToPurchase.Amount + "  Coins with $" +
                           _coinToPurchase.Price + "?";
    }

    public void OnConfirmPurchasePanelButtonClicked(bool isConfirmed)
    {
        confirmPanel.SetActive(false);
        if (isConfirmed)
        {
            PurchaseController.BuyProductId(_coinToPurchase.ProductId);
        }
    }

    public static void SetDeferredPurchaseReminderStatus(CoinList.Coin coin, bool isActive)
    {
        var storeItemCoinGameObj = coin.StoreItemCoinGameObj;
        storeItemCoinGameObj.transform.Find("deferredPurchaseReminder")?.gameObject.SetActive(isActive);
        storeItemCoinGameObj.GetComponent<Button>().interactable = !isActive;
    }
}