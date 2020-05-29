// Copyright 2019 Google LLC
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

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Translates PlayCore's AssetPackErrorCode and AssetPackStatus Java enums into their corresponding public-facing
    /// Unity enums.
    /// </summary>
    internal static class PlayCoreTranslator
    {
        private static class AssetPackErrorCode
        {
            public const int NoError = 0;
            public const int AppUnavailable = -1;
            public const int PackUnavailable = -2;
            public const int InvalidRequest = -3;
            public const int DownloadNotFound = -4;
            public const int ApiNotAvailable = -5;
            public const int NetworkError = -6;
            public const int AccessDenied = -7;
            public const int InsufficientStorage = -10;
            public const int PlayStoreNotFound = -11;
            public const int NetworkUnrestricted = -12;
            public const int AppNotOwned = -13;
            public const int InternalError = -100;
        }

        public static class AssetPackStatus
        {
            public const int Unknown = 0;
            public const int Pending = 1;
            public const int Downloading = 2;
            public const int Transferring = 3;
            public const int Completed = 4;
            public const int Failed = 5;
            public const int Canceled = 6;
            public const int WaitingForWifi = 7;
            public const int NotInstalled = 8;
        }

        private static readonly Dictionary<int, AssetDeliveryErrorCode> PlayCoreToAssetDeliveryErrors =
            new Dictionary<int, AssetDeliveryErrorCode>
            {
                {AssetPackErrorCode.NoError, AssetDeliveryErrorCode.NoError},
                {AssetPackErrorCode.AppUnavailable, AssetDeliveryErrorCode.AppUnavailable},
                {AssetPackErrorCode.PackUnavailable, AssetDeliveryErrorCode.BundleUnavailable},
                {AssetPackErrorCode.InvalidRequest, AssetDeliveryErrorCode.InternalError},
                {AssetPackErrorCode.ApiNotAvailable, AssetDeliveryErrorCode.InternalError},
                {AssetPackErrorCode.DownloadNotFound, AssetDeliveryErrorCode.InternalError},
                {AssetPackErrorCode.InternalError, AssetDeliveryErrorCode.InternalError},
                {AssetPackErrorCode.NetworkError, AssetDeliveryErrorCode.NetworkError},
                {AssetPackErrorCode.AccessDenied, AssetDeliveryErrorCode.AccessDenied},
                {AssetPackErrorCode.InsufficientStorage, AssetDeliveryErrorCode.InsufficientStorage},
                {AssetPackErrorCode.PlayStoreNotFound, AssetDeliveryErrorCode.PlayStoreNotFound},
                {AssetPackErrorCode.NetworkUnrestricted, AssetDeliveryErrorCode.NetworkUnrestricted},
                {AssetPackErrorCode.AppNotOwned, AssetDeliveryErrorCode.AppNotOwned},
            };

        private static readonly Dictionary<int, AssetDeliveryStatus> PlayCoreToAssetDeliveryStatuses
            =
            new Dictionary<int, AssetDeliveryStatus>
            {
                // Unity Asset Delivery requests are started immediately, so their status is Pending even if play core
                // has not yet set its status to Pending.
                {AssetPackStatus.NotInstalled, AssetDeliveryStatus.Pending},
                {AssetPackStatus.Pending, AssetDeliveryStatus.Pending},
                {AssetPackStatus.Downloading, AssetDeliveryStatus.Retrieving},
                {AssetPackStatus.Transferring, AssetDeliveryStatus.Retrieving},
                {AssetPackStatus.Completed, AssetDeliveryStatus.Available},
                {AssetPackStatus.WaitingForWifi, AssetDeliveryStatus.WaitingForWifi},
                {AssetPackStatus.Failed, AssetDeliveryStatus.Failed},
                {AssetPackStatus.Unknown, AssetDeliveryStatus.Failed},
                {AssetPackStatus.Canceled, AssetDeliveryStatus.Failed},
            };

        /// <summary>
        /// Translates Play Core's AssetPackErrorCode into its corresponding public-facing AssetDeliveryErrorCode.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided error code does not have a corresponding value in AssetDeliveryErrorCode.
        /// </exception>
        public static AssetDeliveryErrorCode TranslatePlayCoreErrorCode(int assetPackErrorCode)
        {
            AssetDeliveryErrorCode translatedErrorCode;
            if (!PlayCoreToAssetDeliveryErrors.TryGetValue(assetPackErrorCode, out translatedErrorCode))
            {
                throw new NotImplementedException("Unexpected error code: " + assetPackErrorCode);
            }

            return translatedErrorCode;
        }

        /// <summary>
        /// Translates Play Core's AssetPackStatus into its corresponding public-facing AssetDeliveryStatus.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided status does not have a corresponding value in AssetDeliveryStatus.
        /// </exception>
        public static AssetDeliveryStatus TranslatePlayCorePackStatus(int assetPackStatus)
        {
            AssetDeliveryStatus translatedStatus;
            if (!PlayCoreToAssetDeliveryStatuses.TryGetValue(assetPackStatus, out translatedStatus))
            {
                throw new NotImplementedException("Unexpected pack status: " + assetPackStatus);
            }

            return translatedStatus;
        }
    }
}
