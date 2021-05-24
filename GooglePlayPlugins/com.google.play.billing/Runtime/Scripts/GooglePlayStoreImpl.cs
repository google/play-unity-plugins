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

#if UNITY_PURCHASING

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Google.Play.Billing.Internal;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Google.Play.Billing
{
    /// <summary>
    /// Implements Unity defined IStore, IGooglePlayStoreExtensions, and IGooglePlayConfiguration.
    /// </summary>
    public class GooglePlayStoreImpl : IStore, IGooglePlayStoreExtensions, IGooglePlayConfiguration
    {
        private AndroidJavaObject _billingClient;
        private BillingClientStateListener _billingClientStateListener;
        private Action<Product> _deferredPurchaseListener;
        private IStoreCallback _callback;
        private readonly GooglePlayBillingInventory _inventory;
        private readonly GooglePlayBillingUtil _billingUtil;
        private readonly JniUtils _jniUtils;

        private volatile Dictionary<SkuType, AsyncRequestStatus> _billingClientQuerySkuDetailsCallStatus =
            new Dictionary<SkuType, AsyncRequestStatus>
            {
                {SkuType.InApp, AsyncRequestStatus.Pending},
                {SkuType.Subs, AsyncRequestStatus.Pending},
            };

        // A thread-safe boolean flag to indicate whether Java PBL is ready or not.
        private volatile bool _billingClientReady;

        // A thread-safe Unity product that is currently in purchase flow. Another purchase flow cannot start if this
        // is not null.
        private volatile ProductDefinition _productInPurchaseFlow;

        // Thread-safe variables for IGooglePlayStoreExtensions.
        private volatile string _obfuscatedProfileId;
        private volatile string _obfuscatedAccountId;

        // Thread-safe variables for IGooglePlayConfiguration.
        private volatile bool _deferredPurchasesEnabled = false;

        public GooglePlayStoreImpl(GooglePlayBillingUtil googlePlayBillingUtil)
        {
            _inventory = new GooglePlayBillingInventory();
            _billingUtil = googlePlayBillingUtil;
            _jniUtils = new JniUtils(_billingUtil);
        }

        #region IStore implementations.

        public void Initialize(IStoreCallback callback)
        {
            _callback = callback;
        }

        // Unity IAP framework will call this method after Initialize method regardless.
        public void RetrieveProducts(ReadOnlyCollection<ProductDefinition> products)
        {
            _billingClientStateListener = new BillingClientStateListener();
            // Retry connection when Play Billing Service is disconnected.
            _billingClientStateListener.OnBillingServiceDisconnected += () =>
            {
                _billingUtil.LogWarningFormat("Service disconnected, retrying connection...");
                EndConnection();
                InstantiateBillingClientAndMakeConnection();
            };
            _billingClientStateListener.OnBillingSetupFinished += (billingResult) =>
            {
                MarkBillingClientStartConnectionCallComplete(billingResult);
                if (_billingClientReady)
                {
                    _inventory.UpdateCatalog(products);

                    // Kick off tasks to retrieve products information on the main thread.
                    _billingUtil.RunOnMainThread(() =>
                        RetrieveProductsInternal(products));
                }
                else
                {
                    _billingUtil.LogWarningFormat("Google Play Store: play billing service unavailable!");
                    _billingUtil.RunOnMainThread(() =>
                        _callback.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable));
                }
            };

            InstantiateBillingClientAndMakeConnection();
        }

        private void RetrieveProductsInternal(ReadOnlyCollection<ProductDefinition> products)
        {
            var queryResult = QueryPurchasesForSkuType(SkuType.InApp);
            if (queryResult != BillingResponseCode.Ok)
            {
                _billingUtil.RunOnMainThread(() =>
                    _callback.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable));
                return;
            }

            queryResult = QueryPurchasesForSkuType(SkuType.Subs);
            if (queryResult != BillingResponseCode.Ok)
            {
                _billingUtil.RunOnMainThread(() =>
                    _callback.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable));
                return;
            }

            // Set the SkuDetails request status to pending state.
            _billingClientQuerySkuDetailsCallStatus[SkuType.InApp] = AsyncRequestStatus.Pending;
            _billingClientQuerySkuDetailsCallStatus[SkuType.Subs] = AsyncRequestStatus.Pending;
            QuerySkuDetailsForSkuType(products, SkuType.InApp);
            QuerySkuDetailsForSkuType(products, SkuType.Subs);
        }

        public void Purchase(ProductDefinition product, string developerPayload)
        {
            if (!_billingClientReady)
            {
                _billingUtil.LogErrorFormat("Google Play Store: play billing service unavailable!");
                return;
            }

            if (_productInPurchaseFlow != null)
            {
                _billingUtil.RunOnMainThread(() =>
                    _callback.OnPurchaseFailed(
                        new PurchaseFailureDescription(
                            product.storeSpecificId, PurchaseFailureReason.ExistingPurchasePending,
                            string.Format("A purchase for {0} is already in progress.",
                                _productInPurchaseFlow.id))));
                return;
            }

            if (_deferredPurchasesEnabled && _deferredPurchaseListener == null)
            {
                throw new InvalidOperationException(
                    "To listen to deferred purchases, please register a listener via IGooglePlayStoreExtensions.");
            }

            // Skip copying PayoutDefinition as it is only supported after 2017.2 and we don't actually need it.
            _productInPurchaseFlow = new ProductDefinition(product.id, product.storeSpecificId, product.type,
                product.enabled);
            using (var billingFlowParamBuilder = new AndroidJavaObject(Constants.BillingFlowParamsBuilder))
            {
                SkuDetails skuDetails;
                if (!_inventory.GetSkuDetails(product.storeSpecificId, out skuDetails))
                {
                    // A SKU that has ProductDefinition should appear in the inventory. If not, the issue could be
                    // transient so that it asks to try again later.
                    _callback.OnPurchaseFailed(
                        new PurchaseFailureDescription(product.storeSpecificId,
                            PurchaseFailureReason.ProductUnavailable,
                            string.Format("Cannot purchase {0} now, please try again later.",
                                _productInPurchaseFlow.id)));
                    return;
                }

                // Log a warning if developerPayload is set.
                if (!String.IsNullOrEmpty(developerPayload))
                {
                    _billingUtil.LogWarningFormat(
                        "Developer payload is no longer supported in Google Play Store. " +
                        "See https://developer.android.com/google/play/billing/developer-payload for more details.");
                }

                billingFlowParamBuilder.Call<AndroidJavaObject>(
                    Constants.BillingFlowParamsBuilderSetSkuDetailsMethod, skuDetails.ToJava());
                LaunchBillingFlow(billingFlowParamBuilder);
            }
        }

        public void FinishTransaction(ProductDefinition product, string transactionId)
        {
            Purchase purchase;
            if (!_inventory.GetPurchase(product, out purchase))
            {
                // TODO: handle the use case where purchase is not found.
                return;
            }

            FinishTransactionInternal(purchase, product.type);
        }

        #endregion

        #region IGooglePlayStoreExtensions implementations.

        public void SetObfuscatedAccountId(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                throw new ArgumentException("Google Play Store: cannot set obfuscated account ID to null or empty.");
            }

            _obfuscatedAccountId = accountId;
        }

        public void SetObfuscatedProfileId(string profileId)
        {
            if (string.IsNullOrEmpty(profileId))
            {
                throw new ArgumentException("Google Play Store: cannot set obfuscated profile ID to null or empty.");
            }

            _obfuscatedProfileId = profileId;
        }

        public void ConfirmSubscriptionPriceChange(string productId, Action<bool> callback)
        {
            if (!IsGooglePlayInAppBillingServiceAvailable())
            {
                _billingUtil.RunOnMainThread(() => callback.Invoke(false));
                return;
            }

            ProductDefinition unityProduct;
            SkuDetails skuDetails;

            if (!_inventory.GetUnityProductDefinition(productId, out unityProduct)
                || !_inventory.GetSkuDetails(unityProduct.storeSpecificId, out skuDetails))
            {
                _billingUtil.LogErrorFormat("Cannot find SKU for Unity product {0}", productId);
                _billingUtil.RunOnMainThread(() => callback.Invoke(false));
                return;
            }

            var listener = new PriceChangeConfirmationListener();
            Action<AndroidJavaObject> cb = (billingResult) =>
                _billingUtil.RunOnMainThread(() => ProcessPriceChangeResult(billingResult, callback));
            listener.OnPriceChangeConfirmationResult += cb;

            using (var priceChangeConfirmationParamBuilder =
                new AndroidJavaObject(Constants.PriceChangeFlowParamsBuilder)
                    .Call<AndroidJavaObject>(Constants.PriceChangeFlowParamsBuilderSetSkuDetailsMethod,
                        skuDetails.ToJava()))
            {
                _billingClient.Call(
                    Constants.LaunchPriceChangeConfirmationFlowMethod,
                    JniUtils.GetUnityAndroidActivity(),
                    priceChangeConfirmationParamBuilder.Call<AndroidJavaObject>(Constants.BuildMethod),
                    listener);
            }
        }

        public void EndConnection()
        {
            if (!IsGooglePlayInAppBillingServiceAvailable())
            {
                return;
            }

            _productInPurchaseFlow = null;
            _deferredPurchaseListener = null;
            _billingClient.Call(Constants.BillingClientEndConnectionMethod);
            _billingClientReady = false;
        }

        public void UpgradeDowngradeSubscription(string oldSkuMetadata, string newSku)
        {
            throw new GooglePlayStoreUnsupportedException(
                "For upgrading/downgrading subscription products on Google Play, " +
                "please use IGooglePlayStoreExtensions.UpdateSubscription.");
        }

        public void UpdateSubscription(
            Product oldProduct, Product newProduct, GooglePlayStoreProrationMode prorationMode)
        {
            if (!IsGooglePlayInAppBillingServiceAvailable())
            {
                return;
            }

            Purchase oldPurchase;
            if (!_inventory.GetPurchase(oldProduct.definition, out oldPurchase))
            {
                _billingUtil.LogErrorFormat(
                    "Cannot find the subscription purchase record to be updated: {0}",
                    oldProduct.definition.storeSpecificId);
                return;
            }

            if (_productInPurchaseFlow != null)
            {
                _billingUtil.RunOnMainThread(() =>
                    _callback.OnPurchaseFailed(
                        new PurchaseFailureDescription(newProduct.definition.storeSpecificId,
                            PurchaseFailureReason.ExistingPurchasePending,
                            string.Format("A purchase for {0} is already in progress.",
                                _productInPurchaseFlow.id))));
                return;
            }

            _productInPurchaseFlow = newProduct.definition;
            using (var billingFlowParamBuilder = new AndroidJavaObject(Constants.BillingFlowParamsBuilder))
            {
                SkuDetails skuDetails;
                if (!_inventory.GetSkuDetails(newProduct.definition.storeSpecificId, out skuDetails))
                {
                    // A SKU that has ProductDefinition should appear in the inventory. If not, the issue could be
                    // transient so that it asks to try again later.
                    _callback.OnPurchaseFailed(
                        new PurchaseFailureDescription(newProduct.definition.storeSpecificId,
                            PurchaseFailureReason.ProductUnavailable,
                            string.Format("Cannot update subscription for {0} now, please try again later.",
                                _productInPurchaseFlow.id)));
                    return;
                }

                billingFlowParamBuilder.Call<AndroidJavaObject>(
                    Constants.BillingFlowParamsBuilderSetSkuDetailsMethod, skuDetails.ToJava());
                billingFlowParamBuilder.Call<AndroidJavaObject>(
                    Constants.BillingFlowParamsBuilderSetOldSkuMethod,
                    oldPurchase.ProductId, oldPurchase.PurchaseToken);
                billingFlowParamBuilder.Call<AndroidJavaObject>(
                    Constants.BillingFlowParamsBuilderSetReplaceSkusProrationModeMethod,
                    (int) prorationMode);

                LaunchBillingFlow(billingFlowParamBuilder);
            }
        }

        public void RestoreTransactions(Action<bool> callback)
        {
            if (!IsGooglePlayInAppBillingServiceAvailable())
            {
                callback.Invoke(false);
                return;
            }

            var inAppPurchasesResult = _billingClient.Call<AndroidJavaObject>(Constants.QueryPurchasesMethod,
                SkuType.InApp.ToString());
            var subsPurchasesResult = _billingClient.Call<AndroidJavaObject>(Constants.QueryPurchasesMethod,
                SkuType.Subs.ToString());

            if (_jniUtils.GetResponseCodeFromQueryPurchasesResult(inAppPurchasesResult) !=
                BillingResponseCode.Ok ||
                _jniUtils.GetResponseCodeFromQueryPurchasesResult(subsPurchasesResult) != BillingResponseCode.Ok)
            {
                callback.Invoke(false);
                return;
            }

            var allPurchases = _jniUtils.ParseQueryPurchasesResult(inAppPurchasesResult)
                .Concat(_jniUtils.ParseQueryPurchasesResult(subsPurchasesResult))
                .ToList();

            _inventory.UpdatePurchaseInventory(allPurchases);
            _billingUtil.RunOnMainThread(() =>
                _callback.OnProductsRetrieved(_inventory.CreateProductDescriptionList()));
            _billingUtil.RunOnMainThread(() => callback.Invoke(true));
        }

        public void FinishAdditionalTransaction(string productId, string transactionId)
        {
            Purchase purchase;
            ProductDefinition unityProduct;
            if (!_inventory.GetUnityProductDefinition(productId, out unityProduct)
                || !_inventory.GetPurchase(unityProduct.storeSpecificId, out purchase)
                || string.IsNullOrEmpty(transactionId)
                || !transactionId.Equals(purchase.TransactionId))
            {
                // TODO: handle the use case where purchase is not found.
                return;
            }

            FinishTransactionInternal(purchase, unityProduct.type);
        }

        public Dictionary<string, string> GetProductJSONDictionary()
        {
            return IsGooglePlayInAppBillingServiceAvailable() ? _inventory.GetAllSkuDetails() : null;
        }

        public void SetDeferredPurchaseListener(Action<Product> callback)
        {
            if (!_deferredPurchasesEnabled)
            {
                throw new InvalidOperationException(
                    "To listen to deferred purchases, please enable deferred purchases from " +
                    "IGooglePlayConfiguration.");
            }

            _deferredPurchaseListener = callback;
        }

        #endregion

        #region IGooglePlayConfiguration implementations.

        public void EnableDeferredPurchase()
        {
            _deferredPurchasesEnabled = true;
        }

        #endregion

        /// <summary>
        /// Possible status of an async request made to the Java Play Billing Library.
        /// </summary>
        private enum AsyncRequestStatus
        {
            Pending,
            Failed,
            Succeed,
        }

        private bool IsGooglePlayInAppBillingServiceAvailable()
        {
            if (_billingClientReady)
            {
                return true;
            }

            _billingUtil.LogErrorFormat("Service is unavailable.");
            return false;
        }

        private void InstantiateBillingClientAndMakeConnection()
        {
            // Set ready flag to false as this action could be triggered when in-app billing service is disconnected.
            _billingClientReady = false;

            var context = JniUtils.GetApplicationContext();

            _billingClient = new AndroidJavaObject(Constants.BillingClient,
                Constants.Version);

            var purchaseUpdatedListener = new PurchasesUpdatedListener();
            purchaseUpdatedListener.OnPurchasesUpdated += ParsePurchaseResult;

            _billingClient.Call(
                Constants.BillingClientSetUpMethod,
                context,
                purchaseUpdatedListener,
                _deferredPurchasesEnabled);

            _billingClient.Call(Constants.BillingClientStartConnectionMethod, _billingClientStateListener);
        }

        private void MarkBillingClientStartConnectionCallComplete(AndroidJavaObject billingResult)
        {
            var responseCode = _jniUtils.GetResponseCodeFromBillingResult(billingResult);
            if (responseCode == BillingResponseCode.Ok)
            {
                _billingClientReady = true;
            }
            else
            {
                _billingUtil.LogErrorFormat(
                    "Failed to connect to service with error code '{0}' and debug message: '{1}'.",
                    responseCode, JniUtils.GetDebugMessageFromBillingResult(billingResult));
            }
        }

        private void QuerySkuDetailsForSkuType(ReadOnlyCollection<ProductDefinition> products,
            SkuType skuType)
        {
            var skuDetailsParamBuilder = new AndroidJavaObject(Constants.SkuDetailsParamBuilder);
            string[] skuIds = { };
            if (skuType == SkuType.InApp)
            {
                skuIds = products.Where(p => p.type == ProductType.Consumable || p.type == ProductType.NonConsumable)
                    .Select(p => p.storeSpecificId).ToArray();
            }
            else if (skuType == SkuType.Subs)
            {
                skuIds = products.Where(p => p.type == ProductType.Subscription).Select(p => p.storeSpecificId)
                    .ToArray();
            }
            else
            {
                throw new NotImplementedException(String.Format("Unsupported Google Play Store skuType {0}.", skuType));
            }

            // Set the SkuDetails request status to success state if the list is empty.
            if (!skuIds.Any())
            {
                _billingClientQuerySkuDetailsCallStatus[skuType] = AsyncRequestStatus.Succeed;
                NotifyUnityRetrieveProductsResults();
                return;
            }

            skuDetailsParamBuilder.Call<AndroidJavaObject>(Constants.SkuDetailsParamBuilderSetSkuListMethod,
                JniUtils.CreateJavaArrayList(skuIds));
            skuDetailsParamBuilder.Call<AndroidJavaObject>(Constants.SkuDetailsParamBuilderSetTypeMethod,
                skuType.ToString());
            var skuDetailsParam =
                skuDetailsParamBuilder.Call<AndroidJavaObject>(Constants.BuildMethod);
            var skuDetailsResponseListener = new SkuDetailsResponseListener(skuType);
            skuDetailsResponseListener.OnSkuDetailsResponse += ParseSkuDetailsResults;
            _billingClient.Call(Constants.QuerySkuDetailsMethod, skuDetailsParam,
                skuDetailsResponseListener);
        }

        private void ParseSkuDetailsResults(SkuType skuType, AndroidJavaObject billingResult,
            AndroidJavaObject skuDetailsList)
        {
            _inventory.UpdateSkuDetailsInventory(_jniUtils.ParseSkuDetailsResult(billingResult, skuDetailsList));

            var responseCode = _jniUtils.GetResponseCodeFromBillingResult(billingResult);
            _billingClientQuerySkuDetailsCallStatus[skuType] = responseCode == BillingResponseCode.Ok
                ? AsyncRequestStatus.Succeed
                : AsyncRequestStatus.Failed;

            NotifyUnityRetrieveProductsResults();
        }

        private void NotifyUnityRetrieveProductsResults()
        {
            // Do not fire callback if there is any pending SkuDetails call.
            if (_billingClientQuerySkuDetailsCallStatus[SkuType.InApp] == AsyncRequestStatus.Pending
                || _billingClientQuerySkuDetailsCallStatus[SkuType.Subs] == AsyncRequestStatus.Pending)
            {
                return;
            }

            if (_billingClientQuerySkuDetailsCallStatus[SkuType.InApp] == AsyncRequestStatus.Succeed
                && _billingClientQuerySkuDetailsCallStatus[SkuType.Subs] == AsyncRequestStatus.Succeed)
            {
                _billingUtil.RunOnMainThread(() =>
                    _callback.OnProductsRetrieved(_inventory.CreateProductDescriptionList()));
            }
            else
            {
                // TODO: Retry RetrieveProducts call.
                _billingUtil.RunOnMainThread(() =>
                    _callback.OnSetupFailed(InitializationFailureReason.PurchasingUnavailable));
            }
        }

        private BillingResponseCode QueryPurchasesForSkuType(SkuType skuType)
        {
            var purchasesResult =
                _billingClient.Call<AndroidJavaObject>(Constants.QueryPurchasesMethod, skuType.ToString());
            _inventory.UpdatePurchaseInventory(_jniUtils.ParseQueryPurchasesResult(purchasesResult));
            return _jniUtils.GetResponseCodeFromQueryPurchasesResult(purchasesResult);
        }

        private void ParsePurchaseResult(AndroidJavaObject billingResult, AndroidJavaObject javaPurchasesList)
        {
            var responseCode = _jniUtils.GetResponseCodeFromBillingResult(billingResult);
            var debugMessage = JniUtils.GetDebugMessageFromBillingResult(billingResult);
            if (responseCode == BillingResponseCode.Ok)
            {
                var purchasesList = _jniUtils.ParseJavaPurchaseList(javaPurchasesList);
                _inventory.UpdatePurchaseInventory(purchasesList);
                // Unity IAP only supports one purchase at a time.
                Purchase purchase = purchasesList.First();
                if (Google.Play.Billing.Internal.Purchase.State.Pending.Equals(purchase.PurchaseState))
                {
                    string unityProductId;
                    _inventory.GetUnityProductId(purchase.ProductId, out unityProductId);
                    _billingUtil.RunOnMainThread(() =>
                        _deferredPurchaseListener.Invoke(_callback.products.WithID(unityProductId)));
                }
                else
                {
                    _billingUtil.RunOnMainThread(() =>
                        _callback.OnPurchaseSucceeded(purchase.ProductId, purchase.JsonReceipt,
                            purchase.TransactionId));
                }
            }
            else
            {
                _billingUtil.LogWarningFormat("Purchase failed with error code '{0}' and debug message: '{1}'",
                    responseCode, debugMessage);
                PurchaseFailureReason purchaseFailureReason;
                switch (responseCode)
                {
                    case BillingResponseCode.UserCancelled:
                        purchaseFailureReason = PurchaseFailureReason.UserCancelled;
                        break;
                    case BillingResponseCode.ServiceTimeout:
                    case BillingResponseCode.ServiceDisconnected:
                    case BillingResponseCode.ServiceUnavailable:
                    case BillingResponseCode.BillingUnavailable:
                        purchaseFailureReason = PurchaseFailureReason.PurchasingUnavailable;
                        break;
                    case BillingResponseCode.ItemUnavailable:
                        purchaseFailureReason = PurchaseFailureReason.ProductUnavailable;
                        break;
                    case BillingResponseCode.ItemAlreadyOwned:
                        purchaseFailureReason = PurchaseFailureReason.DuplicateTransaction;
                        break;
                    default:
                        purchaseFailureReason = PurchaseFailureReason.Unknown;
                        break;
                }

                // Copy _productInPurchaseFlow.id into the delegate as the delegate is called by the main thread and
                // _productInPurchaseFlow will get cleaned up immediately in this thread.
                var productId = _productInPurchaseFlow.storeSpecificId;
                _billingUtil.RunOnMainThread(() =>
                    _callback.OnPurchaseFailed(
                        new PurchaseFailureDescription(productId, purchaseFailureReason, debugMessage)));
            }

            _productInPurchaseFlow = null;
        }

        private void ProcessAcknowledgePurchaseResult(string skuId, SkuType skuType, AndroidJavaObject billingResult)
        {
            string productId;
            if (!_inventory.GetUnityProductId(skuId, out productId))
            {
                _billingUtil.LogErrorFormat(
                    "Couldn't find Unity product that maps to {0} when processing acknowledge purchase result",
                    skuId);
                return;
            }

            var responseCode = _jniUtils.GetResponseCodeFromBillingResult(billingResult);
            if (responseCode != BillingResponseCode.Ok)
            {
                _billingUtil.LogErrorFormat(
                    "Acknowledging purchase {0} failed with error code {1} and debug message: {2}.",
                    productId,
                    _jniUtils.GetResponseCodeFromBillingResult(billingResult),
                    JniUtils.GetDebugMessageFromBillingResult(billingResult));
                return;
            }

            // Call queryPurchases again to update the acknowledge status.
            var queryResult = QueryPurchasesForSkuType(skuType);
            if (queryResult != BillingResponseCode.Ok)
            {
                _billingUtil.LogWarningFormat(
                    "Update the purchase {0} after FinishTransaction failed with error code {1}. " +
                    "The purchase receipt might be out of date. One can call FetchAdditionalProducts " +
                    "on this product to get the updated purchase receipt.",
                    productId, queryResult);
            }

            // Double check updated results....
            var updateList = _inventory.UpdateProductDescriptionList(new List<string> {skuId});
            _callback.OnProductsRetrieved(updateList);
        }

        private void ProcessConsumePurchaseResult(string skuId, AndroidJavaObject billingResult)
        {
            var responseCode = _jniUtils.GetResponseCodeFromBillingResult(billingResult);
            if (responseCode == BillingResponseCode.Ok)
            {
                _inventory.RemovePurchase(skuId);
            }
            else
            {
                var debugMessage = JniUtils.GetDebugMessageFromBillingResult(billingResult);
                _billingUtil.LogErrorFormat(
                    "Failed to finish the transaction with error code {0} and debug message: {1}. " +
                    "Please consider to call FinishAdditionalTransaction using the extension.",
                    responseCode, debugMessage);
            }
        }

        // Launches billing flow using billingFlowParamBuilder, in addition, it will add corresponding
        // accountId, developerId parameters.
        private void LaunchBillingFlow(AndroidJavaObject billingFlowParamBuilder)
        {
            if (!string.IsNullOrEmpty(_obfuscatedProfileId))
            {
                billingFlowParamBuilder.Call<AndroidJavaObject>(
                    Constants.BillingFlowParamsBuilderSetObfuscatedProfileIdMethod, _obfuscatedProfileId);
            }

            if (!string.IsNullOrEmpty(_obfuscatedAccountId))
            {
                billingFlowParamBuilder.Call<AndroidJavaObject>(
                    Constants.BillingFlowParamsBuilderSetObfuscatedAccountIdMethod, _obfuscatedAccountId);
            }

            _billingClient.Call<AndroidJavaObject>(
                Constants.BillingClientLaunchBillingFlowMethod,
                JniUtils.GetUnityAndroidActivity(),
                billingFlowParamBuilder.Call<AndroidJavaObject>(Constants.BuildMethod));
        }

        private void FinishTransactionInternal(Purchase purchase, ProductType productType)
        {
            switch (productType)
            {
                case ProductType.Consumable:
                    using (var consumeParamsBuilder = new AndroidJavaObject(Constants.ConsumeParamsBuilder))
                    {
                        consumeParamsBuilder.Call<AndroidJavaObject>(
                            Constants.ConsumeParamsBuilderSetPurchaseTokenMethod,
                            purchase.PurchaseToken);
                        var consumeResponseListener = new ConsumeResponseListener(purchase.ProductId);
                        consumeResponseListener.OnConsumeResponse += ProcessConsumePurchaseResult;
                        _billingClient.Call(Constants.BillingClientConsumePurchaseMethod,
                            consumeParamsBuilder.Call<AndroidJavaObject>(Constants.BuildMethod),
                            consumeResponseListener);
                    }

                    break;
                case ProductType.NonConsumable:
                case ProductType.Subscription:
                    // Skip calling acknowledgePurchase if it is already acknowledged.
                    if (purchase.Acknowledged)
                    {
                        return;
                    }

                    using (var acknowledgeParamsBuilder = new AndroidJavaObject(Constants.AcknowledgeParamsBuilder))
                    {
                        acknowledgeParamsBuilder.Call<AndroidJavaObject>(
                            Constants.AcknowledgeParamsBuilderSetPurchaseTokenMethod,
                            purchase.PurchaseToken);
                        var skuType = productType == ProductType.Subscription ? SkuType.Subs : SkuType.InApp;
                        var acknowledgeResponseListener = new AcknowledgeResponseListener(purchase.ProductId, skuType);
                        acknowledgeResponseListener.OnAcknowledgeResponse += ProcessAcknowledgePurchaseResult;
                        _billingClient.Call(Constants.BillingClientAcknowledgePurchaseMethod,
                            acknowledgeParamsBuilder.Call<AndroidJavaObject>(Constants.BuildMethod),
                            acknowledgeResponseListener);
                    }

                    break;
                default:
                    throw new NotImplementedException(
                        string.Format("Unhandled product type {0} in FinishTransaction", productType));
            }
        }

        private void ProcessPriceChangeResult(AndroidJavaObject billingResult, Action<bool> callback)
        {
            var responseCode = _jniUtils.GetResponseCodeFromBillingResult(billingResult);
            if (responseCode != BillingResponseCode.Ok)
            {
                _billingUtil.LogWarningFormat(
                    "Confirming subscription price change failed with error code {0} and debug message: {1}",
                    responseCode, JniUtils.GetDebugMessageFromBillingResult(billingResult));
            }

            callback.Invoke(responseCode == BillingResponseCode.Ok);
        }
    }
}

#endif // UNITY_PURCHASING