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

namespace Google.Play.Billing.Internal
{
    /// <summary>
    /// Represents a Google Play Store in-app purchase.
    /// </summary>
    public class Purchase
    {
        private readonly PurchaseData _purchaseData;
        private readonly Identifiers _identifiers;
        private readonly PurchaseReceipt _purchaseReceipt;

        public string ProductId {
            get
            {
                return _purchaseData.productId;
            }
        }
        public string PackageName
        {
            get
            {
                return _purchaseData.packageName;
            }
        }

        public string OrderId
        {
            get
            {
                return _purchaseData.orderId;
            }
        }

        /// <summary>
        /// Returns the purchase token.
        /// Old purchases may have it in "token" while new purchases have it in "purchaseToken".
        /// </summary>
        public string PurchaseToken
        {
            get
            {
                return string.IsNullOrEmpty(_purchaseData.token) ? _purchaseData.purchaseToken : _purchaseData.token;
            }
        }

        /// <summary>
        /// Returns the status of this purchase.
        /// The status is defaulted to Purchased unless explicitly specified as Pending. purchaseState = 4 in purchase
        /// json stands for Pending state.
        /// </summary>
        public State PurchaseState
        {
            get
            {
                return _purchaseData.purchaseState == 4 ? State.Pending : State.Purchased;
            }
        }

        public long PurchaseTime
        {
            get
            {
                return _purchaseData.purchaseTime;
            }
        }

        public bool Acknowledged
        {
            get
            {
                return _purchaseData.acknowledged;
            }
        }

        public bool AutoRenewing
        {
            get
            {
                return _purchaseData.autoRenewing;
            }
        }

        public Identifiers AccountIdentifiers
        {
            get
            {
                return _identifiers;
            }
        }

        /// <summary>
        /// Returns the purchase receipt by converting it into a JSON object.
        /// </summary>
        public string JsonReceipt
        {
            get
            {
                return JsonUtility.ToJson(_purchaseReceipt);
            }
        }

        /// <summary>
        /// Returns the transaction identifier.
        /// Test purchases don't have an order Id so purchaseToken is returned instead.
        /// </summary>
        public string TransactionId
        {
            get
            {
                return string.IsNullOrEmpty(OrderId) ? PurchaseToken : OrderId;
            }
        }

        /// <summary>
        /// Creates a purchase object from the JSON representation.
        /// </summary>
        /// <returns>
        /// true if a <cref="Purchase"/> object gets created successfully; otherwise, it returns false and sets
        /// input purchase to null.
        /// </returns>
        public static bool FromJson(string jsonPurchaseData, string signature, out Purchase purchase)
        {
            try
            {
                var purchaseData = JsonUtility.FromJson<PurchaseData>(jsonPurchaseData);
                purchase = new Purchase(purchaseData, jsonPurchaseData, signature);
                return true;
            }
            catch (Exception)
            {
                // Error is logged at the caller side.
                purchase = null;
                return false;
            }
        }

        private Purchase(PurchaseData purchaseData, string jsonPurchaseData, string signature)
        {
            _purchaseData = purchaseData;
            _identifiers = new Identifiers(_purchaseData.obfuscatedAccountId, _purchaseData.obfuscatedProfileId);
            _purchaseReceipt = new PurchaseReceipt(jsonPurchaseData, signature);
        }

        public class Identifiers
        {
            private string _obfuscatedAccountId;
            private string _obfuscatedProfileId;

            public string ObfuscatedAccountId
            {
                get
                {
                    return _obfuscatedAccountId;
                }
            }

            public string ObfuscatedProfileId
            {
                get
                {
                    return _obfuscatedProfileId;
                }
            }

            public Identifiers(string obfuscatedAccountId, string obfuscatedProfileId)
            {
                _obfuscatedAccountId = obfuscatedAccountId;
                _obfuscatedProfileId = obfuscatedProfileId;
            }
        }

        [Serializable]
        private class PurchaseData
        {
#pragma warning disable 649
            // Variables names are matched to the format used in JSON representation.
            public string productId;
            public string packageName;

            public string orderId;
            public string token;
            public string purchaseToken;
            public int purchaseState;
            public long purchaseTime;

            // Acknowledged is true if unspecified in purchase object.
            public bool acknowledged = true;
            public bool autoRenewing;

            // Account identifiers
            public string obfuscatedAccountId;
            public string obfuscatedProfileId;
#pragma warning restore 649
        }

        /// <summary>
        /// A receipt that describes the details about a Google Play in-app purchase. It is defined based on
        /// https://docs.unity3d.com/Manual/UnityIAPPurchaseReceipts.html
        /// </summary>
        [Serializable]
        private class PurchaseReceipt
        {
            // Variables names are matched to Unity's definition.
            public string json;
            public string signature;

            public PurchaseReceipt(string jsonPurchaseData, string signature)
            {
                json = jsonPurchaseData;
                this.signature = signature;
            }
        }

        /// <summary>
        /// Indicates the status of the purchase.
        /// </summary>
        public enum State
        {
            UnspecifiedState = 0,
            Purchased = 1,
            Pending = 2,
        }
    }
}
