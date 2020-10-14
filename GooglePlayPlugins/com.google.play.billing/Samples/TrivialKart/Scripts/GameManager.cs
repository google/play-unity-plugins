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

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager inits the game when the game starts and controls the play canvas.
/// It inits constant data, requests for game data load;
/// It controls canvas switches among play, store and garage;
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject playPageCanvas;
    public GameObject storePageCanvas;
    public GameObject garagePageCanvas;
    public GameObject waitCanvas;
    public GameObject storeItemFiveCoinGameObject;
    public GameObject storeItemTenCoinGameObject;
    public GameObject storeItemTwentyCoinGameObject;
    public GameObject storeItemFiftyCoinGameObject;
    public GameObject storeItemCarSedanGameObj;
    public GameObject storeItemCarTruckGameObj;
    public GameObject storeItemCarJeepGameObj;
    public GameObject storeItemCarKartGameObj;
    public GameObject garageItemCarSedanGameObj;
    public GameObject garageItemCarTruckGameObj;
    public GameObject garageItemCarJeepGameObj;
    public GameObject garageItemCarKartGameObj;
    public GameObject playCarSedanGameObj;
    public GameObject playCarJeepGameObj;
    public GameObject playCarTruckGameObj;
    public GameObject playCarKartGameObj;
    public GameObject blueGrassBackgroundGarageItemGameObj;
    public GameObject mushroomBackGroundGarageItemGameObj;
    public GameObject silverVipSubscribeButtonGameObj;
    public GameObject goldenVipSubscribeButtonGameObj;

    private List<GameObject> _canvasPagesList;
    private bool _purchasingMessageActive; 
    
    // Init the game.
    public void Awake()
    {
        _purchasingMessageActive = false;
#if USE_SERVER
        NetworkRequestController.RegisterUserDevice();
#endif
        InitConstantData();
        GameDataController.LoadGameData();
        SetCanvas(playPageCanvas);
    }

    public bool GetPurchasingMessageActive()
    {
        return _purchasingMessageActive;
    }

    public void SetPurchasingMessageActive(bool active)
    {
        if (active != _purchasingMessageActive)
        {
            _purchasingMessageActive = active;
            waitCanvas.SetActive(active);
        }
    }

    // Switch pages when enter the store.
    public void OnEnterStoreButtonClicked()
    {
        SetCanvas(storePageCanvas);
    }

    // Switch pages when enter the play.
    public void OnEnterPlayPageButtonClicked()
    {
        SetCanvas(playPageCanvas);
    }

    // Switch pages when enter the garage.
    public void OnEnterGaragePageButtonClicked()
    {
        SetCanvas(garagePageCanvas);
#if USE_SERVER
        NetworkRequestController.CheckSubscriptionPriceChange();
#endif
    }

    private void SetCanvas(GameObject targetCanvasPage)
    {
        // Set all canvas pages to be inactive.
        foreach (var canvasPage in _canvasPagesList)
        {
            canvasPage.SetActive(false);
        }

        // Set the target canvas page to be active.
        targetCanvasPage.SetActive(true);
    }

    // Check if the player is in play mode (page).
    public bool IsInPlayCanvas()
    {
        return playPageCanvas.activeInHierarchy;
    }

    // Init constant game data before the game starts.
    private void InitConstantData()
    {
        InitCoinList();
        InitCarList();
        InitBackGroundList();
        InitSubscriptionList();
        _canvasPagesList = new List<GameObject>() {playPageCanvas, storePageCanvas, garagePageCanvas};
    }
    
    // Link car game object to the car object in carList.
    private void InitCarList()
    {
        // TODO: Improve it.
        CarList.CarSedan.GarageItemGameObj = garageItemCarSedanGameObj;
        CarList.CarSedan.PlayCarGameObj = playCarSedanGameObj;
        CarList.CarSedan.StoreItemCarGameObj = storeItemCarSedanGameObj;
        CarList.CarTruck.GarageItemGameObj = garageItemCarTruckGameObj;
        CarList.CarTruck.PlayCarGameObj = playCarTruckGameObj;
        CarList.CarTruck.StoreItemCarGameObj = storeItemCarTruckGameObj;
        CarList.CarJeep.GarageItemGameObj = garageItemCarJeepGameObj;
        CarList.CarJeep.PlayCarGameObj = playCarJeepGameObj;
        CarList.CarJeep.StoreItemCarGameObj = storeItemCarJeepGameObj;
        CarList.CarKart.GarageItemGameObj = garageItemCarKartGameObj;
        CarList.CarKart.PlayCarGameObj = playCarKartGameObj;
        CarList.CarKart.StoreItemCarGameObj = storeItemCarKartGameObj;
    }
    
    // Link background game object to the background object in backgroundList.
    private void InitBackGroundList()
    {
        BackgroundList.BlueGrassBackground.GarageItemGameObj = blueGrassBackgroundGarageItemGameObj;
        BackgroundList.BlueGrassBackground.ImageSprite = Resources.Load<Sprite>("background/blueGrass");
        BackgroundList.MushroomBackground.GarageItemGameObj = mushroomBackGroundGarageItemGameObj;
        BackgroundList.MushroomBackground.ImageSprite = Resources.Load<Sprite>("background/coloredShroom");
    }

    private void InitSubscriptionList()
    {
        SubscriptionList.SilverSubscription.SubscribeButtonGameObj = silverVipSubscribeButtonGameObj;
        SubscriptionList.GoldenSubscription.SubscribeButtonGameObj = goldenVipSubscribeButtonGameObj;
        SubscriptionList.NoSubscription.SubscribeButtonGameObj = new GameObject();
    }

    private void InitCoinList()
    {
        CoinList.FiveCoins.StoreItemCoinGameObj = storeItemFiveCoinGameObject;
        CoinList.TenCoins.StoreItemCoinGameObj = storeItemTenCoinGameObject;
        CoinList.TwentyCoins.StoreItemCoinGameObj = storeItemTwentyCoinGameObject;
        CoinList.FiftyCoins.StoreItemCoinGameObj = storeItemFiftyCoinGameObject;
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        GameDataController.SaveGameData();
    }

    private void OnApplicationQuit()
    {
        GameDataController.SaveGameData();
    }
}