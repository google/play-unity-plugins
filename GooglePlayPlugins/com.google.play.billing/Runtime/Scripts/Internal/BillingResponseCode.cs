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

namespace Google.Play.Billing.Internal
{
    /// <summary>
    /// Possible response codes.
    /// Public documentation can be found at
    /// https://developer.android.com/reference/com/android/billingclient/api/BillingClient.BillingResponseCode
    /// </summary>
    public enum BillingResponseCode
    {
        ServiceTimeout = -3,
        FeatureNotSupported = -2,
        ServiceDisconnected = -1,
        Ok = 0,
        UserCancelled = 1,
        ServiceUnavailable = 2,
        BillingUnavailable = 3,
        ItemUnavailable = 4,
        DeveloperError = 5,
        Error = 6,
        ItemAlreadyOwned = 7,
        ItemNotOwned = 8,
    }
}
