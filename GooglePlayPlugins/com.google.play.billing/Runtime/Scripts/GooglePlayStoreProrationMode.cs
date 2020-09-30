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

namespace Google.Play.Billing
{
    /// <summary>
    /// Proration modes during SKU replacement supported by Google Play. Please refer to
    /// https://developer.android.com/google/play/billing/subs#change for instructions and examples.
    ///
    /// Reserved enum: 4. Note that the Deferred mode is not supported here because it is not compatible with the
    /// Unity IAP. More details can be found at: https://issuetracker.google.com/168990998.
    /// </summary>
    public enum GooglePlayStoreProrationMode
    {
        Unknown = 0,

        /// <summary>
        /// Replacement takes effect immediately, and the remaining time will be prorated and credited to the user. This
        /// is the current default behavior.
        /// </summary>
        ImmediateWithTimeProration = 1,

        /// <summary>
        /// Replacement takes effect immediately, and the billing cycle remains the same. The price for the remaining
        /// period will be charged. This option is only available for subscription upgrade.
        /// </summary>
        ImmediateAndChargeProratedPrice = 2,

        /// <summary>
        /// Replacement takes effect immediately, and the new price will be charged on next recurrence time. The billing
        /// cycle stays the same.
        /// </summary>
        ImmediateWithoutProration = 3,
    }
}
