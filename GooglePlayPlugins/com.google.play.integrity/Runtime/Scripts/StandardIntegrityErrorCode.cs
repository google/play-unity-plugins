// Copyright 2023 Google LLC
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

namespace Google.Play.Integrity
{
    /// <summary>
    /// Error codes for Standard Integrity API.
    /// </summary>
    public enum StandardIntegrityErrorCode
    {
        NoError = 0,

        /// <summary>
        /// Standard Integrity API is not available.
        /// 
        /// <para>
        /// Standard Integrity API is not enabled, or the Play Store version might be old.
        /// Recommended actions:
        /// <list>
        /// <item>Make sure to be allow-listed to use Standard Integrity API.</item>
        /// <item>Make sure that Integrity API is enabled in Google Play Console.</item>
        /// <item>Ask the user to update Play Store..</item>
        /// </list>
        /// </para>
        /// </summary>
        ApiNotAvailable = -1,

        /// <summary>
        /// No Play Store app is found on device or not official version is installed.
        /// 
        /// <para>Ask the user to install an official and recent version of Play Store.</para>
        /// </summary>
        PlayStoreNotFound = -2,

        /// <summary>
        /// No available network is found.
        /// 
        /// <para>Ask the user to check for a connection.</para>
        /// </summary>
        NetworkError = -3,

        /// <summary>
        /// The calling app is not installed.
        /// 
        /// <para>Something is wrong (possibly an attack). Non-actionable.</para>
        /// </summary>
        AppNotInstalled = -5,

        /// <summary>
        /// Play Services is not available or version is too old.
        /// 
        /// <para>Ask the user to Install or Update Play Services.</para>
        /// </summary>
        PlayServicesNotFound = -6,

        /// <summary>
        /// The calling app UID (user id) does not match the one from Package Manager.
        /// 
        /// <para>Something is wrong (possibly an attack). Non-actionable.</para>
        /// </summary>
        AppUidMismatch = -7,

        /// <summary>
        /// The calling app is making too many requests to the API and hence is throttled.
        /// 
        /// <para>Retry with an exponential backoff.</para>
        /// </summary>
        TooManyRequests = -8,

        /// <summary>
        /// Binding to the service in the Play Store has failed. This can be due to having an old Play Store version
        /// installed on the device or device memory is overloaded.
        /// 
        /// <para>Ask the user to update Play Store.</para>
        /// <para>Retry with an exponential backoff.</para>
        /// </summary>
        CannotBindToService = -9,

        /// <summary>
        /// Unknown internal Google server error.
        /// 
        /// <para>Retry with an exponential backoff. Consider filing a bug if fails consistently.</para>
        /// </summary>
        GoogleServerUnavailable = -12,

        /// <summary>
        /// The Play Store needs to be updated.
        /// 
        /// <para>Ask the user to update the Google Play Store.</para>
        /// </summary>
        PlayStoreVersionOutdated = -14,

        /// <summary>
        /// Play Services needs to be updated.
        /// <para>Ask the user to update Google Play Services.</para>
        /// </summary>
        PlayServicesVersionOutdated = -15,

        /// <summary>
        /// The provided cloud project number is invalid.
        ///
        /// <para> Use the cloud project number which can be found in Project info in your Google Cloud Console for the
        /// cloud project where Play Integrity API is enabled.</para>
        /// </summary>
        CloudProjectNumberIsInvalid = -16,

        /// <summary>
        /// The provided request hash is too long. The request hash length must be less than 500 bytes.
        ///
        /// <para>Retry with a shorter request hash.</para>
        /// </summary>
        RequestHashTooLong = -17,

        /// <summary>
        /// There was a transient error in the client device.
        ///
        /// <para>Retry with an exponential backoff.</para>
        /// <para>If the error persists after a few retries, you should assume
        /// that the device has failed integrity checks and act accordingly.
        /// </para>
        /// </summary>
        ClientTransientError = -18,

        /// <summary>
        /// The <see cref="StandardIntegrityTokenProvider"/> is invalid (e.g.
        /// it is outdated). This error can be returned only for <see cref="StandardIntegrityTokenProvider.request"/>
        ///
        /// <para>Request a new integrity token provider by calling
        /// <see cref="StandardIntegrityManager.prepareIntegrityToken"/></para>
        /// </summary>
        IntegrityTokenProviderInvalid = -19,

        /// <summary>
        /// Unknown internal error.
        /// 
        /// <para>Retry with an exponential backoff. Consider filing a bug if fails consistently.</para>
        /// </summary>
        InternalError = -100,
    }
}