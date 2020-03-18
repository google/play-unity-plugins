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
    /// A fake implementation to provide platform abstraction.
    /// </summary>
    public class FakeGooglePlayStoreExtensions : IGooglePlayStoreExtensions
    {
        public void UpgradeDowngradeSubscription(string oldSku, string newSku)
        {
        }

        public void UpdateSubscription(Product oldProduct, Product newProduct,
            GooglePlayStoreProrationMode prorationMode = GooglePlayStoreProrationMode.Unknown)
        {
        }

        public void RestoreTransactions(Action<bool> callback)
        {
        }

        public void SetObfuscatedAccountId(string obfuscatedAccountId)
        {
        }

        public void SetObfuscatedProfileId(string obfuscatedProfileId)
        {
        }

        public Dictionary<string, string> GetProductJSONDictionary()
        {
            return null;
        }

        public void FinishAdditionalTransaction(string productId, string transactionId)
        {
        }

        public void ConfirmSubscriptionPriceChange(string productId, Action<bool> callback)
        {
        }

        public void SetDeferredPurchaseListener(Action<Product> callback)
        {
        }

        public void EndConnection()
        {
        }
    }
}

#endif // UNITY_PURCHASING