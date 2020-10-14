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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

/// <summary>
/// Controller for the dollar item purchase flow in the game.
/// It uses Unity IAP and the Google Play Billing Library plugin 
/// to do purchasing, order verification, and order restore.
/// </summary>
public class PurchaseController : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController; // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    private static Google.Play.Billing.IGooglePlayStoreExtensions _playStoreExtensions;
    private static GameManager _gameManager;
    private static UIPriceChangeController _uiPriceChangeController;
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _uiPriceChangeController = FindObjectOfType<UIPriceChangeController>();
        // If we haven't set up the Unity Purchasing reference.
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing.
            InitializePurchasing();
        }
    }

    private void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, passing in the Google Play store module
        var builder = ConfigurationBuilder.Instance(Google.Play.Billing.GooglePlayStoreModule.Instance());

        // Enable deferred purchases
        builder.Configure<Google.Play.Billing.IGooglePlayConfiguration>()
            .EnableDeferredPurchase();

        // Add consumable products (coins) associated with their product types to sell / restore by way of its identifier,
        // associating the general identifier with its store-specific identifiers.
        foreach (var coin in CoinList.List)
        {
            builder.AddProduct(coin.ProductId, ProductType.Consumable);
        }

        // Continue adding the non-consumable products (car) with their product types.
        foreach (var car in CarList.List.Where(car => car.IsRealMoneyPurchase && car.Price > 0))
        {
            builder.AddProduct(car.ProductId, ProductType.NonConsumable);
        }

        // Adding subscription products with their product types.
        foreach (var subscription in SubscriptionList.List.Where(subscription =>
            subscription.Type != SubscriptionType.NoSubscription))
        {
            builder.AddProduct(subscription.ProductId, ProductType.Subscription);
        }

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
        Debug.Log("PurchaseController: Calling UnityPurchasing.Intialize");
    }

    private static bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public static void BuyProductId(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing
            // system's products collection.
            var product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ...
            if (product != null && product.availableToPurchase)
            {
                // Bring up the 'Purchasing' modal message
                _gameManager.SetPurchasingMessageActive(true);

                Debug.Log(string.Format("PurchaseController: Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation
                Debug.Log(
                    "PurchaseController: BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or
            // retrying initialization.
            Debug.Log("PurchaseController: BuyProductID FAIL. Not initialized.");
        }
    }


    public static void PurchaseASubscription(SubscriptionList.Subscription oldSubscription,
        SubscriptionList.Subscription newSubscription)
    {
        // If the player is subscribe to a new subscription from no subscription,
        // go through the purchase IAP follow.
        if (oldSubscription.Type == SubscriptionType.NoSubscription)
        {
            BuyProductId(newSubscription.ProductId);
        }
        // If the player wants to update or downgrade the subscription,
        // use the upgrade and downgrade flow.
        else
        {
            var oldProduct = m_StoreController.products.WithID(oldSubscription.ProductId);
            var newProduct = m_StoreController.products.WithID(newSubscription.ProductId);
            _playStoreExtensions.UpdateSubscription(oldProduct, newProduct);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("PurchaseController: OnInitialized success");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        // Retrieve the current item prices from the store
        UpdateItemPrices();

        // Set play store extensions.
        _playStoreExtensions =
            m_StoreExtensionProvider.GetExtension<Google.Play.Billing.IGooglePlayStoreExtensions>();
        CheckSubscriptionsAvailabilityBasedOnReceipt(controller);
        // Pass an obfuscated account id.
        _playStoreExtensions.SetObfuscatedAccountId(TrivialKartClientUtil.GetObfuscatedAccountId());
        // Set the deferred purchases callback.
        _playStoreExtensions.SetDeferredPurchaseListener(
            delegate(Product product)
            {
                ProcessDeferredPurchase(product.definition.id);
                // Do not grant the item here. Instead, record the purchase and remind
                // the user to complete the transaction in the Play Store.
            });
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("PurchaseController: OnInitializeFailed InitializationFailureReason:" + error);
    }

    private void UpdateItemPrices()
    {
        Debug.Log("PurchaseController: UpdateItemPrices");

        // This is a very basic loop through all the items, updating the
        // default price information with the current localized price
        // supplied by the store.
        foreach (var product in m_StoreController.products.all) 
        {
            float currentPrice = Convert.ToSingle(product.metadata.localizedPrice);
            string productId = product.definition.storeSpecificId;
            string localizedPriceString = product.metadata.localizedPriceString;
            bool foundItem = false;

            foreach (var coin in CoinList.List.Where(coin =>
                string.Equals(productId, coin.ProductId, StringComparison.Ordinal)))
            {
                Debug.Log($"Updated price for Product: '{productId}', New Price: {currentPrice}");
                coin.Price = currentPrice;
                foundItem = true;
                break;
            }

            if (!foundItem)
            {
                foreach (var car in CarList.List.Where(car => string.Equals(productId, car.ProductId,
                    StringComparison.Ordinal)))
                {
                    Debug.Log($"Updated price for Product: '{productId}', New Price: {currentPrice}");
                    car.Price = currentPrice;
                    foundItem = true;
                    break;
                }
            }

            if (!foundItem)
            {
                foreach (var subscription in SubscriptionList.List.Where(subscription => string.Equals(productId,
                    subscription.ProductId,
                    StringComparison.Ordinal)))
                {
                    Debug.Log($"Updated price for Product: '{productId}', New Price: {currentPrice}");
                    subscription.Price = currentPrice;
                    foundItem = true;
                    break;
                }
            }

            // If we found the item, update the UI text element that actually displays
            // the price information for the current item
            if (foundItem)
            {
                _uiPriceChangeController.UpdatePriceTextItem(productId, localizedPriceString);
            }
        }
    }

    private static void CheckSubscriptionsAvailabilityBasedOnReceipt(IStoreController controller)
    {
        var silverSubscriptionProduct = controller.products.WithID(SubscriptionList.SilverSubscription.ProductId);
        var goldenSubscriptionProduct = controller.products.WithID(SubscriptionList.GoldenSubscription.ProductId);
        if (!(silverSubscriptionProduct.hasReceipt && ClientSideReceiptValidation(silverSubscriptionProduct.receipt)) &&
            !(goldenSubscriptionProduct.hasReceipt && ClientSideReceiptValidation(goldenSubscriptionProduct.receipt)))
        {
            GameDataController.GetGameData().UpdateSubscription(SubscriptionList.NoSubscription);
            Debug.Log("PurchaseController: No subscription receipt found. Unsubscribe all subscriptions");
        }
    }

    private static void ProcessDeferredPurchase(string productId)
    {
        // Check if a consumable (coins) has been deferred purchased by this user.
        foreach (var coin in CoinList.List.Where(coin =>
            string.Equals(productId, coin.ProductId, StringComparison.Ordinal)))
        {
            CoinStorePageController.SetDeferredPurchaseReminderStatus(coin, true);
            return;
        }

        // Check if a non-consumable (car) has been deferred purchased by this user.
        foreach (var car in CarList.List.Where(car => string.Equals(productId, car.ProductId,
            StringComparison.Ordinal)))
        {
            CarStorePageController.SetDeferredPurchaseReminderStatus(car, true);
            return;
        }

        Debug.LogError("PurchaseController: Product ID doesn't match any of exist one-time products that can be deferred purchase.");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        
        Debug.Log($"PurchaseController: ProcessPurchase: PASS. Product: '{args.purchasedProduct.definition.id}'");
#if USE_SERVER
        Debug.Log("Calling VerifyAndSaveUserPurchase");
        NetworkRequestController.VerifyAndSaveUserPurchase(args.purchasedProduct);
        // Make sure the 'Purchasing' modal message is dismissed
        _gameManager.SetPurchasingMessageActive(false);
        return PurchaseProcessingResult.Pending;
#else
        if (ClientSideReceiptValidation(args.purchasedProduct.receipt))
        {
            // Unlock the appropriate content.
            UnlockInGameContent(args.purchasedProduct.definition.id);
        }

        // Make sure the 'Purchasing' modal message is dismissed
        _gameManager.SetPurchasingMessageActive(false);

        // Return a flag indicating whether this product has completely been received, or if the application needs
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still
        // saving purchased products to the cloud, and when that save is delayed.
        return PurchaseProcessingResult.Complete;
#endif
    }

    public static void ConfirmPendingPurchase(Product product, bool purchaseVerifiedSuccess)
    {
        if (purchaseVerifiedSuccess)
        {
            UnlockInGameContent(product.definition.id);
        }

        m_StoreController.ConfirmPendingPurchase(product);
    }

    private static bool ClientSideReceiptValidation(string unityIapReceipt)
    {
        bool validPurchase = true;
#if UNITY_ANDROID
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

        try
        {
            // Validate the signature of the receipt with unity cross platform validator
            var result = validator.Validate(unityIapReceipt);

            // Validate the obfuscated account id of the receipt.
            ObfuscatedAccountIdValidation(unityIapReceipt);

            // For informational purposes, we list the receipt(s).
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        }
        catch (IAPSecurityException)
        {
            Debug.Log("PurchaseController: Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif
        return validPurchase;
    }

    // Check if the obfuscated account id on the receipt is same as the one on the device.
    private static void ObfuscatedAccountIdValidation(string unityIapReceipt)
    {
        Dictionary<string, object> unityIapReceiptDictionary =
            (Dictionary<string, object>) MiniJson.JsonDecode(unityIapReceipt);
        string payload = (string) unityIapReceiptDictionary["Payload"];
        Dictionary<string, object> payLoadDictionary = (Dictionary<string, object>) MiniJson.JsonDecode(payload);
        string receipt = (string) payLoadDictionary["json"];

        Dictionary<string, object> receiptDictionary = (Dictionary<string, object>) MiniJson.JsonDecode(receipt);
        if (!receiptDictionary.ContainsKey("obfuscatedAccountId") ||
            !receiptDictionary["obfuscatedAccountId"].Equals(TrivialKartClientUtil.GetObfuscatedAccountId()))
        {
            Debug.Log("PurchaseController: Obfuscated account id is invalid");
            throw new IAPSecurityException();
        }
    }

    private static void UnlockInGameContent(string productId)
    {
        var gameData = GameDataController.GetGameData();

        // Check if a consumable (coins) has been purchased by this user.
        foreach (var coin in CoinList.List.Where(coin =>
            string.Equals(productId, coin.ProductId, StringComparison.Ordinal)))
        {
            gameData.UpdateCoins(coin);
            return;
        }

        // Check if a non-consumable (car) has been purchased by this user.
        foreach (var car in CarList.List.Where(car => string.Equals(productId, car.ProductId,
            StringComparison.Ordinal)))
        {
            gameData.PurchaseCar(car);
            return;
        }

        // Check if a subscription has been purchased by this user.
        foreach (var subscription in SubscriptionList.List.Where(subscription => string.Equals(productId,
            subscription.ProductId,
            StringComparison.Ordinal)))
        {
            gameData.UpdateSubscription(subscription);
            return;
        }

        
        // Pop up window
        Debug.LogError("PurchaseController: Product ID doesn't match any existing products.");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // Make sure the 'Purchasing' modal message is dismissed
        _gameManager.SetPurchasingMessageActive(false);
            
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(
            $"PurchaseController: OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");

        // If the purchase failed with a duplicate transaction response, and the item is not shown in client side,
        // do a restore purchase to fetch the product.
        // This situation can occur when the user loses internet connectivity after sending the purchase.
        if (failureReason == PurchaseFailureReason.DuplicateTransaction)
        {
            _playStoreExtensions.RestoreTransactions(null);
        }
    }

    public static void confirmSubscriptionPriceChange(string productId)
    {
        _playStoreExtensions.ConfirmSubscriptionPriceChange(productId,
            delegate(bool priceChangeSucess)
            {
                if (priceChangeSucess)
                {
                    // Here you can choose to make an update or record that the user accpected the new price
                    Debug.Log("PurchaseController: The user accepted the price change");
                }
                else
                {
                    Debug.Log("PurchaseController: The user did not accept the price change");
                }
            }
        );
    }

    // Restore purchase when the user login to a new device.
    public static void RestorePurchase()
    {
        _playStoreExtensions.RestoreTransactions(
            delegate(bool restoreSuccess)
            {
                var garageController = FindObjectOfType<GarageController>();
                if (restoreSuccess)
                {
                    Debug.Log("PurchaseController: Successfully restored purchase!");
                    garageController.OnRestorePurchaseSuccess();
                }
                else
                {
                    Debug.Log("PurchaseController: Failed to restore purchase");
                    garageController.OnRestorePurchaseFail();
                }
            });
    }
}
