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
using UnityEngine.Purchasing;

namespace Google.Play.Billing
{
    /// <summary>
    /// Extends IStoreExtension to provide additional features supported by Google Play Store.
    /// </summary>
    public interface IGooglePlayStoreExtensions : IStoreExtension
    {

        /// <summary>
        /// Specifies an optional obfuscated string that is uniquely associated with the user's account
        /// in your app.
        ///
        /// If you pass this value, Google Play can use it to detect irregular activity, such as many
        /// devices making purchases on the same account in a short period of time. Do not use this field
        /// to store any Personally Identifiable Information (PII) such as emails in cleartext.
        /// Attempting to store PII in this field will result in purchases being blocked. Google Play
        /// recommends that you use either encryption or a secure one-way hash to generate an obfuscated
        /// identifier to send to Google Play.
        ///
        /// You can also retrieve this identifier via the purchase receipt.
        ///
        /// This identifier is limited to 64 characters.
        /// </summary>
        /// <param name="obfuscatedAccountId">The obfuscated user's in-app account ID.</param>
        void SetObfuscatedAccountId(string obfuscatedAccountId);

        /// <summary>
        /// Specifies an optional obfuscated string that is uniquely associated with the user's profile
        /// in your app.
        ///
        /// Some applications allow users to have multiple profiles within a single account. Use this
        /// method to send the user's profile identifier to Google. Setting this field requests the
        /// user's obfuscated account id to be pass via {@link #setObfuscatedAccountId(String)}.
        ///
        /// If you pass this value, Google Play can use it to detect irregular activity, such as many
        /// devices making purchases on the same account in a short period of time. Do not use this field
        /// to store any Personally Identifiable Information (PII) such as emails in cleartext.
        /// Attempting to store PII in this field will result in purchases being blocked. Google Play
        /// recommends that you use either encryption or a secure one-way hash to generate an obfuscated
        /// identifier to send to Google Play.
        ///
        /// You can also retrieve this identifier via the purchase receipt.
        ///
        /// This identifier is limited to 64 characters.
        /// </summary>
        /// <param name="obfuscatedProfileId">Obfuscated string of developer profile name</param>
        void SetObfuscatedProfileId(string obfuscatedProfileId);

        /// <summary>
        /// Returns a Dictionary that reflects SKU details in the internal inventory. The result maps Play Store SKU Ids
        /// to strings in JSON format that contain SKU details.
        /// </summary>
        Dictionary<string, string> GetProductJSONDictionary();

        /// <summary>
        /// Upgrades or downgrades a purchased subscription to a new subscription product.
        /// </summary>
        [Obsolete("Deprecated, please use IGooglePlayStoreExtensions.UpdateSubscription(...) instead.")]
        void UpgradeDowngradeSubscription(string oldSkuMetadata, string newSku);

        /// <summary>
        /// Upgrades or downgrades a purchased subscription to a new subscription product.
        /// </summary>
        /// <param name="oldProduct">Currently purchased subscription.</param>
        /// <param name="newProduct">The new subscription to update to.</param>
        /// <param name="prorationMode">Specifies the mode of proration during subscription upgrade/downgrade. To find
        /// more details: https://developer.android.com/reference/com/android/billingclient/api/BillingFlowParams.Builder.html#setReplaceSkusProrationMode(int).</param>
        void UpdateSubscription(Product oldProduct, Product newProduct,
            GooglePlayStoreProrationMode prorationMode = GooglePlayStoreProrationMode.Unknown);

        /// <summary>
        /// Requests Google Play Store to check for new purchases. If a new purchase is found, it posts results to
        /// IStoreListener.ProcessPurchase.
        /// </summary>
        void RestoreTransactions(Action<bool> callback);

        /// <summary>
        /// Finishes a transaction by consuming a consumable purchase or acknowledging an non-consumable purchase.
        /// <param name="productId">the Unity product identifier.</param>
        /// <param name="transactionId">the Id associated with the product to be finished.</param>
        /// </summary>
        void FinishAdditionalTransaction(string productId, string transactionId);

        /// <summary>
        /// Initiate a flow to confirm the change of price for an item subscribed by the user.
        /// <param name="productId">the Unity product Id.</param>
        /// <param name="callback">the callback that notifies the caller the result from launching the flow. <c>true</c>
        /// represents success.</param>
        /// </summary>
        void ConfirmSubscriptionPriceChange(string productId, Action<bool> callback);

        /// <summary>
        /// Sets up a listener to receive callback on Google Play deferred purchases, i.e. purchases required to be
        /// completed in a physical store using cash. Developers can listen to this callback and give users instructions
        /// on how to complete such transactions.
        /// </summary>
        /// <param name="callback">the callback that notifies developers about Google Play deferred purchases (cash
        /// purchases waiting to be completed).</param>
        void SetDeferredPurchaseListener(Action<Product> callback);

        void EndConnection();
    }
}

#endif // UNITY_PURCHASING