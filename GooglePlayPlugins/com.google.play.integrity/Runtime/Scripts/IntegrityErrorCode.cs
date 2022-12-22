// Copyright 2021 Google LLC
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
    /// Error codes for Integrity API
    /// </summary>
    public enum IntegrityErrorCode
    {
        /// <summary>
        /// No error has occurred.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// Integrity API is not available.
        /// <para>
        /// Integrity API is not enabled, or the Play Store version might be old.
        /// Recommended actions:
        /// <list>
        /// <item>Make sure that Integrity API is enabled in Google Play Console.</item>
        /// <item>Ask the user to update Play Store.</item>
        /// </list>
        /// </para>
        /// </summary>
        ApiNotAvailable = 1,

        /// <summary>
        /// No Play Store app is found on device or not official version is installed.
        /// <para>
        /// Ask the user to install an official and recent version of Play Store.
        /// </para>
        /// </summary>
        PlayStoreNotFound = 2,

        /// <summary>
        /// No available network is found.
        /// <para>
        /// Ask the user to check for a connection.
        /// </para>
        /// </summary>
        NetworkError = 3,

        /// <summary>
        /// No Play Store account is found on device. Note that the Play Integrity API now supports
        /// unauthenticated requests. This error code is used only for older Play Store versions
        /// that lack support.
        /// <para>
        /// Ask the user to authenticate in Play Store.
        /// </para>
        /// </summary>
        PlayStoreAccountNotFound = 4,

        /// <summary>
        /// The calling app is not installed.
        /// <para>
        /// Something is wrong (possibly an attack). Non-actionable.
        /// </para>
        /// </summary>
        AppNotInstalled = 5,

        /// <summary>
        /// Play Services is not available or version is too old.
        /// <para>
        /// Ask the user to Install or Update Play Services.
        /// </para>
        /// </summary>
        PlayServicesNotFound = 6,

        /// <summary>
        /// The calling app UID (user id) does not match the one from Package Manager.
        /// <para>
        /// Something is wrong (possibly an attack). Non-actionable.
        /// </para>
        /// </summary>
        AppUidMismatch = 7,

        /// <summary>
        /// The calling app is making too many requests to the API and hence is throttled.
        /// <para>
        /// Retry with an exponential backoff.
        /// </para>
        /// </summary>
        TooManyRequests = 8,

        /// <summary>
        /// Binding to the service in the Play Store has failed. This can be due to having an old Play
        /// Store version installed on the device.
        /// <para>
        /// Ask the user to update Play Store.
        /// </para>
        /// </summary>
        CannotBindToService = 9,

        /// <summary>
        /// Unknown internal Google server error.
        /// <para>
        /// Retry with an exponential backoff. Consider filing a bug if fails consistently.
        /// </para>
        /// </summary>
        GoogleServerUnavailable = 10,

        /// <summary>
        /// Nonce length is too short. The nonce must be a minimum of 16 bytes (before base64 encoding) to allow for
        /// better security.
        /// <para>
        /// Retry with a longer nonce.
        /// </para>
        /// </summary>
        NonceTooShort = 11,

        /// <summary>
        /// Nonce length is too long. The nonce must be less than 500 bytes before base64 encoding.
        /// <para>
        /// Retry with a shorter nonce.
        /// </para>
        /// </summary>
        NonceTooLong = 12,

        /// <summary>
        /// Nonce is not encoded as a base64 web-safe no-wrap string.
        /// <para>
        /// Retry with correct nonce format.
        /// </para>
        /// </summary>
        NonceIsNotBase64 = 13,

        /// <summary>
        /// The Play Store needs to be updated.
        /// <para>
        /// The Play Store needs to be updated.
        /// </para>
        /// </summary>
        PlayStoreVersionOutdated = 14,

        /// <summary>
        /// Play Services needs to be updated.
        /// <para>
        /// Ask the user to update Google Play Services.
        /// </para>
        /// </summary>
        PlayServicesVersionOutdated = 15,

        /// <summary>
        /// The provided cloud project number is invalid.
        /// <para>
        /// Use the cloud project number which can be found in Project info in your Google Cloud
        /// Console for the cloud project where Play Integrity API is enabled.
        /// </para>
        /// </summary>
        CloudProjectNumberIsInvalid = 16,

        /// <summary>
        /// There was a transient error in the client device.
        /// <para>
        /// Retry with an exponential backoff.
        /// </para>
        /// <para>
        /// Introduced in Integrity Play Core version 1.1.0 (prior versions returned a token with
        /// empty Device Integrity Verdict). If the error persists after a few retries, you should
        /// assume that the device has failed integrity checks and act accordingly.
        /// </para>
        /// </summary>
        ClientTransientError = 17,

        /// <summary>
        /// Unknown internal error.
        /// <para>
        /// Retry with an exponential backoff. Consider filing a bug if fails consistently.
        /// </para>
        /// </summary>
        InternalError = 100,
    }
}
