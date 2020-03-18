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

using UnityEngine.Purchasing.Extension;

namespace Google.Play.Billing
{
    /// <summary>
    /// Extends IStoreConfiguration to provide additional configurations supported by Google Play Store.
    /// </summary>
    public interface IGooglePlayConfiguration : IStoreConfiguration
    {
        /// <summary>
        /// Enables Google Play deferred purchase support. If enabled, developers are also expected to set a
        /// DeferredPurchaseListener by calling
        /// <see cref="IGooglePlayStoreExtensions.SetDeferredPurchaseListener"/>. More details can be found at
        /// https://developer.android.com/google/play/billing/billing_library_overview#pending.
        ///
        /// Note: In Google Play Billing, the term used is "pending". To differentiate from Unity pending purchases,
        /// here we refer to Google Play Billing pending purchases as "deferred" purchases.
        /// </summary>
        void EnableDeferredPurchase();
    }
}

#endif // UNITY_PURCHASING