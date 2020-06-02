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
using Google.Play.Billing.Internal;
using UnityEngine;

namespace Google.Play.Billing.Internal
{
    [Serializable]
    public class SkuDetails
    {
#pragma warning disable 649
        // Variables names are matched to the format used in JSON representation.
        public string productId;
        public string type;
        public string title;
        public string description;
        public string skuDetailsToken;
        public string iconUrl;

        public string price;
        public long price_amount_micros;
        public string price_currency_code;

        public string original_price;
        public long original_price_amount_micros;

        public string subscriptionPeriod;
        public string freeTrialPeriod;
        public string introductoryPrice;
        public long introductoryPriceAmountMicros;
        public string introductoryPricePeriod;
        public int introductoryPriceCycles;
#pragma warning restore 649

        public string JsonSkuDetails { get; private set; }

        /// <summary>
        /// Creates a SkuDetails object from its JSON representation.
        /// </summary>
        public static bool FromJson(string jsonSkuDetails, out SkuDetails skuDetails)
        {
            try
            {
                skuDetails = JsonUtility.FromJson<SkuDetails>(jsonSkuDetails);
                skuDetails.JsonSkuDetails = jsonSkuDetails;
                return true;
            }
            catch (Exception)
            {
                // Error is logged at the caller side.
                skuDetails = null;
                return false;
            }
        }

        /// <summary>
        /// Creates a Java SkuDetails object.
        /// </summary>
        public AndroidJavaObject ToJava()
        {
            return new AndroidJavaObject(Constants.SkuDetailsClass, JsonSkuDetails);
        }
    }
}
