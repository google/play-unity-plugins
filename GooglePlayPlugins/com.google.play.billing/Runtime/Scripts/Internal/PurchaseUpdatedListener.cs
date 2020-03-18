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
    /// Callback for purchase updates which happen when, for example, the user buys something within
    /// the app or initiates a purchase from Google Play Store.
    /// </summary>
    public class PurchasesUpdatedListener : AndroidJavaProxy
    {
        public event Action<AndroidJavaObject, AndroidJavaObject> OnPurchasesUpdated = delegate { };
        public PurchasesUpdatedListener() : base(Constants.PurchaseUpdatedListener)
        {
        }

        void onPurchasesUpdated(AndroidJavaObject billingResult, AndroidJavaObject purchasesList)
        {
            OnPurchasesUpdated.Invoke(billingResult, purchasesList);
        }
    }
}
