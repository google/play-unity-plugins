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

using System.Collections.Generic;
using System.Linq;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace Google.Play.Billing.Internal
{
    /// <summary>
    /// Manages the products metadata and products ownership information.
    /// </summary>
    public class GooglePlayBillingInventory
    {
        // A constant to convert amount in micros into decimal amount.
        private static readonly decimal Micros = new decimal(1000000L);

        // A shared lock object for all maps used in this class.
        private readonly object _inventoryLock = new object();

        // A full list of mapping between Unity defined product id and Unity ProductDefinition.
        private readonly Dictionary<string, ProductDefinition> _catalog = new Dictionary<string, ProductDefinition>();
        private readonly Dictionary<string, string> _googlePlaySkuIdToUnityProductId = new Dictionary<string, string>();

        // Internal inventories are indexed using the Google Play Store specific sku id, which could be different from
        // Unity product id.
        private readonly Dictionary<string, SkuDetails> _skuDetailsInventory = new Dictionary<string, SkuDetails>();

        private readonly Dictionary<string, Purchase> _purchaseInventory = new Dictionary<string, Purchase>();

        private readonly Dictionary<string, Purchase> _pendingPurchaseInventory = new Dictionary<string, Purchase>();

        /// <summary>
        /// Updates the catalog of all products. These products will be alive while the GooglePlayStore instance is
        /// alive.
        /// </summary>
        public void UpdateCatalog(IEnumerable<ProductDefinition> products)
        {
            lock (_inventoryLock)
            {
                foreach (var product in products)
                {
                    _catalog[product.id] = product;
                    _googlePlaySkuIdToUnityProductId[product.storeSpecificId] = product.id;
                }
            }
        }

        /// <summary>
        /// Updates the inventory of skuDetails with the provided list of skuDetails.
        /// </summary>
        public void UpdateSkuDetailsInventory(IEnumerable<SkuDetails> skuDetailsList)
        {
            lock (_inventoryLock)
            {
                foreach (var skuDetails in skuDetailsList)
                {
                    _skuDetailsInventory[skuDetails.productId] = skuDetails;
                }
            }
        }

        /// <summary>
        /// Updates the inventory of purchases with the provided list of purchases.
        /// </summary>
        public void UpdatePurchaseInventory(IEnumerable<Purchase> purchaseList)
        {
            lock (_inventoryLock)
            {
                foreach (var purchase in purchaseList)
                {
                    if (Purchase.State.Pending.Equals(purchase.PurchaseState))
                    {
                        _pendingPurchaseInventory[purchase.ProductId] = purchase;
                    }
                    else
                    {
                        _purchaseInventory[purchase.ProductId] = purchase;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ProductDefinition"/> associated with the given Unity product Id.
        /// </summary>
        /// <returns>true if the inventory contains requested Unity product Id; otherwise, false.</returns>
        public bool GetUnityProductDefinition(string unityProductId, out ProductDefinition unityProduct)
        {
            lock (_inventoryLock)
            {
                return _catalog.TryGetValue(unityProductId, out unityProduct);
            }
        }

        /// <summary>
        /// Gets the Unity product Id given the Google Play SKU Id.
        /// </summary>
        public bool GetUnityProductId(string googlePlaySkuId, out string unityProductId)
        {
            lock (_inventoryLock)
            {
                return _googlePlaySkuIdToUnityProductId.TryGetValue(googlePlaySkuId, out unityProductId);
            }
        }

        /// <summary>
        /// Gets the <cref="SkuDetails"/> associated with the specified Google Play Store SKU id from inventory.
        /// <returns>true if the inventory contains the requested SKU; otherwise, false.</returns>
        /// </summary>
        public bool GetSkuDetails(string sku, out SkuDetails skuDetails)
        {
            lock (_inventoryLock)
            {
                return _skuDetailsInventory.TryGetValue(sku, out skuDetails);
            }
        }

        /// <summary>
        /// Gets all the <see cref="SkuDetails"/>s from inventory.
        /// <returns>A dictionary that maps Play Store specific Ids to JSON format strings that represent the
        /// corresponding <see cref="SkuDetails"/>s. </returns>
        /// </summary>
        public Dictionary<string, string> GetAllSkuDetails()
        {
            lock (_inventoryLock)
            {
                return _skuDetailsInventory.ToDictionary(pair => pair.Key, pair => pair.Value.JsonSkuDetails);
            }
        }

        /// <summary>
        /// Gets the <cref="Purchase"/> associated with the specified Google Play Store SKU id from inventory.
        /// <returns>true if the inventory contains the requested SKU; otherwise, false.</returns>
        /// </summary>
        public bool GetPurchase(ProductDefinition product, out Purchase purchase)
        {
            return GetPurchase(product.storeSpecificId, out purchase);
        }

        /// <summary>
        /// Gets the <cref="Purchase"/> associated with the specified Google Play Store SKU id from inventory.
        /// <returns>true if the inventory contains the requested SKU; otherwise, false.</returns>
        /// </summary>
        public bool GetPurchase(string googlePlaySkuId, out Purchase purchase)
        {
            lock (_inventoryLock)
            {
                return _purchaseInventory.TryGetValue(googlePlaySkuId, out purchase);
            }
        }

        /// <summary>
        /// Removes the <cref="Purchase"/> associated with the specified Google Play Store SKU id from inventory.
        /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
        /// </summary>
        public bool RemovePurchase(string skuId)
        {
            if (string.IsNullOrEmpty(skuId))
            {
                return false;
            }
            lock (_inventoryLock)
            {
                // Try to clear it from pending purchases too just in case, ignore the result.
                _pendingPurchaseInventory.Remove(skuId);
                var skuIdPurchaseValue = _purchaseInventory[skuId];
                _purchaseInventory.Remove(skuId);
                return skuIdPurchaseValue != null;
            }
        }

        /// <summary>
        /// Creates a list of ProductDescription from the inventory of skuDetails and purchases.
        /// This list will be used to trigger OnProductRetrieved callback.
        /// </summary>
        public List<ProductDescription> CreateProductDescriptionList()
        {
            var productDescriptionList = new List<ProductDescription>();
            lock (_inventoryLock)
            {
                foreach (var skuId in _skuDetailsInventory.Keys)
                {
                    productDescriptionList.Add(GetProductDescriptionForSku(skuId));
                }

            }
            return productDescriptionList;
        }

        /// <summary>
        /// Creates a updated list of ProductDescription for specified Google Play SKU Ids from the inventory of
        /// skuDetails and purchases.
        /// This list will be used to trigger OnProductRetrieved callback.
        /// </summary>
        public List<ProductDescription> UpdateProductDescriptionList(IEnumerable<string> googlePlaySkuIds)
        {
            var productDescriptionList = new List<ProductDescription>();
            lock (_inventoryLock)
            {
                foreach (var skuId in googlePlaySkuIds)
                {
                    if (!_skuDetailsInventory.ContainsKey(skuId))
                    {
                        continue;
                    }

                    productDescriptionList.Add(GetProductDescriptionForSku(skuId));
                }
            }

            return productDescriptionList;
        }

        // The caller is responsible for locking the map access.
        private ProductDescription GetProductDescriptionForSku(string skuId)
        {
            var skuDetails = _skuDetailsInventory[skuId];
            var productMetadata = new ProductMetadata(
                skuDetails.price,
                skuDetails.title,
                skuDetails.description,
                skuDetails.price_currency_code,
                decimal.Divide(new decimal(skuDetails.price_amount_micros), Micros));

            string receipt = null;
            string transactionId = null;
            if (_purchaseInventory.ContainsKey(skuId))
            {
                receipt = _purchaseInventory[skuId].JsonReceipt;
                transactionId = _purchaseInventory[skuId].TransactionId;
            }

            return new ProductDescription(skuId, productMetadata, receipt, transactionId);
        }
    }
}

#endif // UNITY_PURCHASING