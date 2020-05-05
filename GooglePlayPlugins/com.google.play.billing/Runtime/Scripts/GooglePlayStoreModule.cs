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
using Google.Play.Billing.Internal;
using UnityEngine;
using UnityEngine.Purchasing.Extension;

namespace Google.Play.Billing
{
    /// <summary>
    /// Implements AbstractPurchasingModule. This is needed to config IStore implementation when using Unity IAP on
    /// Android platform.
    /// </summary>
    public class GooglePlayStoreModule : AbstractPurchasingModule
    {
        public const string StoreName = "GooglePlay";

        private IStore _storeInstance;

        private static GooglePlayStoreModule _moduleInstance;
        private static GooglePlayBillingUtil _util;

        /// <summary>
        /// Creates an instance of this module for Android platform. This method throws an exception on Non-Android
        /// platforms.
        ///
        /// This method is not thread-safe.
        /// </summary>
        public static AbstractPurchasingModule Instance()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                throw new NotImplementedException(
                    "Google Play Billing Service is only implemented for Android devices.");
            }

            if (_moduleInstance == null)
            {
                _moduleInstance = new GooglePlayStoreModule();
                var gameObject = new GameObject("GooglePlayBillingUtil");
                // Hide the object so that it it won't be accessible (to make sure it won't be deleted accidentally).
                gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                _util = gameObject.AddComponent<GooglePlayBillingUtil>();
            }

            return _moduleInstance;
        }

        public override void Configure()
        {
            RegisterStore(StoreName, InstantiateGooglePlayBilling());
        }

        private IStore InstantiateGooglePlayBilling()
        {
            if (_storeInstance == null)
            {
                _storeInstance = new GooglePlayStoreImpl(_util);
                m_Binder.RegisterExtension((IGooglePlayStoreExtensions) _storeInstance);
                m_Binder.RegisterConfiguration((IGooglePlayConfiguration) _storeInstance);
            }

            return _storeInstance;
        }
    }
}

#endif // UNITY_PURCHASING