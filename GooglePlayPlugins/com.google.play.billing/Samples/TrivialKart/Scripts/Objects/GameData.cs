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
using Object = UnityEngine.Object;

public enum Ownership
{
    NotOwned,
    Owned
}

public enum SubscriptionType
{
    NoSubscription,
    SilverSubscription,
    GoldenSubscription
}

public enum CarName
{
    Sedan,
    Truck,
    Jeep,
    Kart
}

public enum BackgroundName
{
    BlueGrass,
    Mushroom
}


/// <summary>
/// GameData stores all the items/data the player obtained.
/// It contains methods that changes those data.
/// </summary>
[Serializable]
public class GameData
{
    public CarName carInUseName;
    public Ownership[] carIndexToOwnership;
    public Ownership[] backgroundNameToOwnership;
    public int coinsOwned;
    public SubscriptionType subscriptionType;
    public BackgroundName backgroundInUseName;

    private const int InitialCoinAmount = 20;
    private const int TotalCarCount = 4;
    private const int TotalBackgroundCount = 2;
    private PlayerController _playerController = Object.FindObjectOfType<PlayerController>();
    private GameObject _backgroundImages = GameObject.Find("Background/backGroundImages").gameObject;


    public GameData()
    {
        coinsOwned = InitialCoinAmount;
        carIndexToOwnership = new Ownership[TotalCarCount];
        foreach (var car in CarList.List)
        {
            carIndexToOwnership[(int) car.Name] = Ownership.NotOwned;
        }

        carIndexToOwnership[(int) CarList.CarSedan.Name] = Ownership.Owned;

        backgroundNameToOwnership = new Ownership[TotalBackgroundCount];
        foreach (var background in BackgroundList.List)
        {
            backgroundNameToOwnership[(int) background.Name] = Ownership.NotOwned;
        }

        backgroundNameToOwnership[(int) BackgroundList.BlueGrassBackground.Name] = Ownership.Owned;

        carInUseName = CarName.Sedan;
        subscriptionType = SubscriptionType.NoSubscription;
        backgroundInUseName = BackgroundName.BlueGrass;
    }

    public CarList.Car CarInUseObj => CarList.GetCarByName(carInUseName);

    public BackgroundList.Background BackgroundInUseObj => BackgroundList.GetBackgroundByName(backgroundInUseName);

    public SubscriptionList.Subscription CurSubscriptionObj =>
        SubscriptionList.GetSubscriptionByType(subscriptionType);

    public int CoinsOwned => coinsOwned;

    // Return possible discount on in store items.
    public float Discount => subscriptionType == SubscriptionType.GoldenSubscription ? 0.6f : 1;

    // Reduce amount of coins owned.
    public void ReduceCoinsOwned(int reduceAmount)
    {
        coinsOwned -= reduceAmount;
        UpdateCoinText();
    }

    // Increase amount of coins owned.
    public void IncreaseCoinsOwned(int increaseAmount)
    {
        coinsOwned += increaseAmount;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        Object.FindObjectOfType<StoreController>().SetCoinsBasedOnGameData();
    }

    // Update coins when player purchases coins.
    public void UpdateCoins(CoinList.Coin coinToPurchase)
    {
        IncreaseCoinsOwned(coinToPurchase.Amount);
        CoinStorePageController.SetDeferredPurchaseReminderStatus(coinToPurchase, false);
    }

    // Purchase a car.
    public void PurchaseCar(CarList.Car car)
    {
        if (!car.IsRealMoneyPurchase)
        {
            ReduceCoinsOwned((int) (car.Price * Discount));
        }

        carIndexToOwnership[(int) car.Name] = Ownership.Owned;
        Object.FindObjectOfType<CarStorePageController>()?.RefreshPage();
    }

    // Check if the user owns a specific car.
    // Return true if the user owns it; Otherwise return false.
    public bool CheckCarOwnership(CarList.Car car)
    {
        return carIndexToOwnership[(int) car.Name] == Ownership.Owned;
    }

    // Update car in use and apply the changes to the play page.
    public void UpdateCarInUse(CarList.Car targetCar)
    {
        carInUseName = targetCar.Name;
        _playerController.UpdateCarInUse();
    }

    // Check if the user owns a specific background.
    // Return true if the user owns it; Otherwise return false.
    public bool CheckBackgroundOwnership(BackgroundList.Background background)
    {
        return backgroundNameToOwnership[(int) background.Name] == Ownership.Owned;
    }

    // Update background in use and apply the changes to the play page.
    public void UpdateBackgroundInUse(BackgroundList.Background targetBackground)
    {
        backgroundInUseName = targetBackground.Name;

        foreach (Transform background in _backgroundImages.transform)
        {
            background.gameObject.GetComponent<SpriteRenderer>().sprite = targetBackground.ImageSprite;
        }
    }

    // Set background according to the background in use.
    public void SetBackgroundBasedOnGameData()
    {
        UpdateBackgroundInUse(BackgroundList.GetBackgroundByName(backgroundInUseName));
    }

    // Update current subscription to target subscription.
    public void UpdateSubscription(SubscriptionList.Subscription targetSubscription)
    {
        subscriptionType = targetSubscription.Type;
        if (subscriptionType == SubscriptionType.NoSubscription)
        {
            backgroundNameToOwnership[(int) BackgroundName.Mushroom] = Ownership.NotOwned;
            UpdateBackgroundInUse(BackgroundList.BlueGrassBackground);
        }
        else
        {
            backgroundNameToOwnership[(int) BackgroundName.Mushroom] = Ownership.Owned;
            UpdateBackgroundInUse(BackgroundList.MushroomBackground);
        }

        Object.FindObjectOfType<SubscriptionStorePageController>()?.RefreshPage();
    }
}