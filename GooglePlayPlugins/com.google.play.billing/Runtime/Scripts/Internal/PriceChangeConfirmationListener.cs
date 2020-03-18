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
    /// Callback from the flow that confirms the change of price for an item subscribed by the user.
    /// </summary>
    public class PriceChangeConfirmationListener : AndroidJavaProxy
    {
        public event Action<AndroidJavaObject> OnPriceChangeConfirmationResult = delegate { };

        public PriceChangeConfirmationListener() : base(Constants.PriceChangeConfirmationListener)
        {
        }

        void onPriceChangeConfirmationResult(AndroidJavaObject billingResult)
        {
            OnPriceChangeConfirmationResult.Invoke(billingResult);
        }
    }
}
