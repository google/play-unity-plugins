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

namespace Google.Play.Billing.Internal
{
    /// <summary>
    /// A collection of utils methods to process AndroidJavaObject returned by Google Play Billing Library.
    /// </summary>
    public class JniUtils
    {
        private GooglePlayBillingUtil _util;

        public JniUtils(GooglePlayBillingUtil util)
        {
            _util = util;
        }

        /// <summary>
        /// Returns the Android activity context of the Unity app.
        /// </summary>
        public static AndroidJavaObject GetUnityAndroidActivity()
        {
            return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>(
                "currentActivity");
        }

        /// <summary>
        /// Returns the Android application context of the Unity app.
        /// </summary>
        public static AndroidJavaObject GetApplicationContext()
        {
            return GetUnityAndroidActivity().Call<AndroidJavaObject>("getApplicationContext");
        }

        /// <summary>
        /// Parses the billing results returned by Google Play Billing Client.
        /// </summary>
        /// <returns> A response code that indicates the outcome of the billing result. </returns>
        public BillingResponseCode GetResponseCodeFromBillingResult(AndroidJavaObject billingResult)
        {
            var responseCode = billingResult.Call<int>("getResponseCode");
            var billingResponseCode = BillingResponseCode.Error;
            try
            {
                billingResponseCode =
                    (BillingResponseCode) Enum.Parse(typeof(BillingResponseCode), responseCode.ToString());
            }
            catch (ArgumentNullException)
            {
                _util.LogErrorFormat("Missing response code, return BillingResponseCode.Error.");
            }
            catch (ArgumentException)
            {
                _util.LogErrorFormat("Unknown response code {0}, return BillingResponseCode.Error.",
                    responseCode);
            }

            return billingResponseCode;
        }

        /// <summary>
        /// Parses the billing results returned by Google Play Billing Client.
        /// </summary>
        /// <returns> A debug message to provide detailed error messages to developers. </returns>
        public static string GetDebugMessageFromBillingResult(AndroidJavaObject billingResult)
        {
            return billingResult.Call<string>("getDebugMessage");
        }

        /// <summary>
        /// Parses the purchase results returned by Google Play Billing Client.
        /// </summary>
        /// <returns> A response code that indicates the outcome of the billing result. </returns>
        public BillingResponseCode GetResponseCodeFromQueryPurchasesResult(AndroidJavaObject javaPurchasesResult)
        {
            var billingResult =
                javaPurchasesResult.Call<AndroidJavaObject>(Constants.PurchasesResultGetBillingResultMethod);
            return GetResponseCodeFromBillingResult(billingResult);
        }

        /// <summary>
        /// Create a Java ArrayList of strings.
        /// </summary>
        public static AndroidJavaObject CreateJavaArrayList(params string[] inputs)
        {
            var javaList = new AndroidJavaObject("java.util.ArrayList");
            foreach (var input in inputs)
            {
                javaList.Call<bool>("add", input);
            }

            return javaList;
        }

        /// <summary>
        /// Parses the sku details results returned by the Google Play Billing Library.
        /// </summary>
        /// <returns>An IEnumerable of <cref="SkuDetails"/>. The IEnumerable could be empty.</returns>
        public IEnumerable<SkuDetails> ParseSkuDetailsResult(AndroidJavaObject billingResult,
            AndroidJavaObject skuDetailsList)
        {
            var responseCode = GetResponseCodeFromBillingResult(billingResult);
            if (responseCode != BillingResponseCode.Ok)
            {
                // TODO: retry getSkuDetails call.
                _util.LogErrorFormat(
                    "Failed to retrieve products information! Error code {0}, debug message: {1}.",
                    responseCode, GetDebugMessageFromBillingResult(billingResult));
                return Enumerable.Empty<SkuDetails>();
            }

            var parsedSkuDetailsList = new List<SkuDetails>();
            var size = skuDetailsList.Call<int>("size");
            for (var i = 0; i < size; i++)
            {
                var javaSkuDetails = skuDetailsList.Call<AndroidJavaObject>("get", i);
                // Google Play Store returns non-null, non-empty originalJson.
                var originalJson = javaSkuDetails.Call<string>(Constants.SkuDetailsGetOriginalJson);
                SkuDetails skuDetails;
                if (SkuDetails.FromJson(originalJson, out skuDetails))
                {
                    parsedSkuDetailsList.Add(skuDetails);
                }
                else
                {
                    _util.LogWarningFormat("Failed to parse skuDetails {0} ", originalJson);
                }
            }

            return parsedSkuDetailsList;
        }

        /// <summary>
        /// Parses the purchases result returned by the Google Play Billing Library.
        /// </summary>
        /// <returns>An IEnumerable of <cref="Purchase"/>. The IEnumerable could be empty.</returns>
        public IEnumerable<Purchase> ParseQueryPurchasesResult(AndroidJavaObject javaPurchasesResult)
        {
            var billingResult =
                javaPurchasesResult.Call<AndroidJavaObject>(Constants.PurchasesResultGetBillingResultMethod);
            var responseCode = GetResponseCodeFromBillingResult(billingResult);
            if (responseCode != BillingResponseCode.Ok)
            {
                _util.LogErrorFormat(
                    "Failed to retrieve purchase information! Error code {0}, debug message: {1}.",
                    responseCode, GetDebugMessageFromBillingResult(billingResult));
                return Enumerable.Empty<Purchase>();
            }

            return ParseJavaPurchaseList(
                javaPurchasesResult.Call<AndroidJavaObject>(Constants.PurchasesResultGetPurchasesListMethod));
        }

        /// <summary>
        /// Parses the Java list of purchases returned by the Google Play Billing Library.
        /// </summary>
        /// <returns>An IEnumerable of <cref="Purchase"/>. The IEnumerable could be empty.</returns>
        public IEnumerable<Purchase> ParseJavaPurchaseList(AndroidJavaObject javaPurchasesList)
        {
            var parsedPurchasesList = new List<Purchase>();
            var size = javaPurchasesList.Call<int>("size");
            for (var i = 0; i < size; i++)
            {
                var javaPurchase = javaPurchasesList.Call<AndroidJavaObject>("get", i);
                var originalJson = javaPurchase.Call<string>(Constants.PurchaseGetOriginalJsonMethod);
                var signature = javaPurchase.Call<string>(Constants.PurchaseGetSignatureMethod);
                Purchase purchase;
                if (Purchase.FromJson(originalJson, signature, out purchase))
                {
                    parsedPurchasesList.Add(purchase);
                }
                else
                {
                    _util.LogWarningFormat("Failed to parse purchase {0} ", originalJson);
                }
            }

            return parsedPurchasesList;
        }
    }
}
