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
    /// Callback that notifies when a consumption operation finishes.
    /// </summary>
    public class ConsumeResponseListener : AndroidJavaProxy
    {
        private readonly string _skuId;

        public event Action<string, AndroidJavaObject> OnConsumeResponse = delegate { };

        public ConsumeResponseListener(string skuId) : base(Constants.ConsumeResponseListener)
        {
            _skuId = skuId;
        }

        void onConsumeResponse(AndroidJavaObject billingResult, string purchaseToken)
        {
            OnConsumeResponse.Invoke(_skuId, billingResult);
        }
    }
}
