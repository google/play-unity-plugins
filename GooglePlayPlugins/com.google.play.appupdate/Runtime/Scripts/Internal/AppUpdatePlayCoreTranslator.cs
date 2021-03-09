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
using System.Collections.Generic;
using Google.Play.Core.Internal;

namespace Google.Play.AppUpdate.Internal
{
    /// <summary>
    /// Translates PlayCore's In-App Update Related Java enums into their corresponding public-facing Unity enums.
    /// </summary>
    public static class AppUpdatePlayCoreTranslator
    {
        /// <summary>
        /// Based on PlayCore's InstallErrorCode Java enum
        /// </summary>
        private static class PlayCoreInstallErrorCode
        {
            public const int NoError = 0;
            public const int NoErrorPartiallyAllowed = 1;
            public const int ErrorUnknown = -2;
            public const int ErrorApiNotAvailable = -3;
            public const int ErrorInvalidRequest = -4;
            public const int ErrorInstallUnavailable = -5;
            public const int ErrorInstallNotAllowed = -6;
            public const int ErrorDownloadNotPresent = -7;
            public const int ErrorInstallInProgress = -8;
            public const int ErrorPlayStoreNotFound = -9;
            public const int ErrorAppNotOwned = -10;
            public const int ErrorInternalError = -100;
        }

        /// <summary>
        /// Based on PlayCore's InstallStatus Java enum
        /// </summary>
        private static class PlayCoreUpdateStatus
        {
            public const int Unknown = 0;
            public const int Pending = 1;
            public const int Downloading = 2;
            public const int Downloaded = 11;
            public const int Installing = 3;
            public const int Installed = 4;
            public const int Failed = 5;
            public const int Canceled = 6;
        }

        /// <summary>
        /// Based on PlayCore's UpdateAvailability Java enum
        /// </summary>
        private static class PlayCoreUpdateAvailability
        {
            public const int Unknown = 0;
            public const int UpdateNotAvailable = 1;
            public const int UpdateAvailable = 2;
            public const int DeveloperTriggeredUpdateInProgress = 3;
        }

        /// <summary>
        /// Based on PlayCore's AppUpdateType Java enum
        /// </summary>
        private static class PlayCoreAppUpdateType
        {
            public const int Flexible = 0;
            public const int Immediate = 1;
        }


        private static readonly Dictionary<int, AppUpdateErrorCode> PlayCoreToAppUpdateErrorCode =
            new Dictionary<int, AppUpdateErrorCode>
            {
                {PlayCoreInstallErrorCode.NoError, AppUpdateErrorCode.NoError},
                {PlayCoreInstallErrorCode.NoErrorPartiallyAllowed, AppUpdateErrorCode.NoErrorPartiallyAllowed},
                {PlayCoreInstallErrorCode.ErrorUnknown, AppUpdateErrorCode.ErrorUnknown},
                {PlayCoreInstallErrorCode.ErrorApiNotAvailable, AppUpdateErrorCode.ErrorApiNotAvailable},
                {PlayCoreInstallErrorCode.ErrorInvalidRequest, AppUpdateErrorCode.ErrorInvalidRequest},
                {PlayCoreInstallErrorCode.ErrorInstallUnavailable, AppUpdateErrorCode.ErrorUpdateUnavailable},
                {PlayCoreInstallErrorCode.ErrorInstallNotAllowed, AppUpdateErrorCode.ErrorUpdateNotAllowed},
                {PlayCoreInstallErrorCode.ErrorDownloadNotPresent, AppUpdateErrorCode.ErrorDownloadNotPresent},
                {PlayCoreInstallErrorCode.ErrorInstallInProgress, AppUpdateErrorCode.ErrorUpdateInProgress},
                {PlayCoreInstallErrorCode.ErrorPlayStoreNotFound, AppUpdateErrorCode.ErrorPlayStoreNotFound},
                {PlayCoreInstallErrorCode.ErrorAppNotOwned, AppUpdateErrorCode.ErrorAppNotOwned},
                {PlayCoreInstallErrorCode.ErrorInternalError, AppUpdateErrorCode.ErrorInternalError}
            };

        private static readonly Dictionary<int, AppUpdateStatus> PlayCoreToUpdateStatuses =
            new Dictionary<int, AppUpdateStatus>
            {
                {PlayCoreUpdateStatus.Unknown, AppUpdateStatus.Unknown},
                {PlayCoreUpdateStatus.Pending, AppUpdateStatus.Pending},
                {PlayCoreUpdateStatus.Downloading, AppUpdateStatus.Downloading},
                {PlayCoreUpdateStatus.Downloaded, AppUpdateStatus.Downloaded},
                {PlayCoreUpdateStatus.Installing, AppUpdateStatus.Installing},
                {PlayCoreUpdateStatus.Installed, AppUpdateStatus.Installed},
                {PlayCoreUpdateStatus.Failed, AppUpdateStatus.Failed},
                {PlayCoreUpdateStatus.Canceled, AppUpdateStatus.Canceled}
            };

        private static readonly Dictionary<int, UpdateAvailability> PlayCoreToUpdateAvailabilities =
            new Dictionary<int, UpdateAvailability>
            {
                {PlayCoreUpdateAvailability.Unknown, UpdateAvailability.Unknown},
                {PlayCoreUpdateAvailability.UpdateNotAvailable, UpdateAvailability.UpdateNotAvailable},
                {PlayCoreUpdateAvailability.UpdateAvailable, UpdateAvailability.UpdateAvailable},
                {
                    PlayCoreUpdateAvailability.DeveloperTriggeredUpdateInProgress,
                    UpdateAvailability.DeveloperTriggeredUpdateInProgress
                }
            };

        private static readonly Dictionary<int, AppUpdateType> PlayCoreToAppUpdateType =
            new Dictionary<int, AppUpdateType>
            {
                {PlayCoreAppUpdateType.Immediate, AppUpdateType.Immediate},
                {PlayCoreAppUpdateType.Flexible, AppUpdateType.Flexible},
            };

        /// <summary>
        /// Translates Play Core's InstallErrorCode into its corresponding public-facing AppUpdateErrorCode.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided error code does not have a corresponding value in Unity AppUpdateErrorCode.
        /// </exception>
        public static AppUpdateErrorCode TranslatePlayCoreErrorCode(int playCoreErrorCode)
        {
            AppUpdateErrorCode appUpdateErrorCode;
            if (!PlayCoreToAppUpdateErrorCode.TryGetValue(playCoreErrorCode, out appUpdateErrorCode))
            {
                throw new NotImplementedException("Unexpected error code: " + appUpdateErrorCode);
            }

            return appUpdateErrorCode;
        }

        /// <summary>
        /// Translates Play Core's InstallStatus into its corresponding public-facing InstallStatus.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided status does not have a corresponding value in Unity AppUpdateStatus.
        /// </exception>
        public static AppUpdateStatus TranslatePlayCoreUpdateStatus(int playCoreUpdateStatus)
        {
            AppUpdateStatus appUpdateStatus;
            if (!PlayCoreToUpdateStatuses.TryGetValue(playCoreUpdateStatus, out appUpdateStatus))
            {
                throw new NotImplementedException("Unexpected status: " + appUpdateStatus);
            }

            return appUpdateStatus;
        }

        /// <summary>
        /// Translates Play Core's UpdateAvailability into its corresponding public-facing UpdateAvailability.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided availability does not have a corresponding value in Unity UpdateAvailability.
        /// </exception>
        public static UpdateAvailability TranslatePlayCoreUpdateAvailabilities(int playCoreAvailability)
        {
            UpdateAvailability updateAvailability;
            if (!PlayCoreToUpdateAvailabilities.TryGetValue(playCoreAvailability, out updateAvailability))
            {
                throw new NotImplementedException("Unexpected update availability: " + playCoreAvailability);
            }

            return updateAvailability;
        }

        /// <summary>
        /// Translates Play Core's ActivityResult into its corresponding public-facing AppUpdateStatus.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided Activity resultCode does not have a corresponding value in Unity AppUpdateStatus.
        /// </exception>
        public static AppUpdateStatus TranslateUpdateActivityResult(int resultCode)
        {
            AppUpdateStatus dialogResult;
            switch (resultCode)
            {
                case ActivityResult.ResultOk:
                    // In the flexible flow, ActivityResult.ResultOk indicates the user accepted the request to update and the update is pending.
                    // In the immediate flow, ActivityResult.ResultOk should never be received since it indicates the user accepted the request and the app is already updated.
                    dialogResult = AppUpdateStatus.Pending;
                    break;
                case ActivityResult.ResultCancelled:
                    dialogResult = AppUpdateStatus.Canceled;
                    break;
                case ActivityResult.ResultFailed:
                    dialogResult = AppUpdateStatus.Failed;
                    break;
                default:
                    throw new NotImplementedException("Unexpected activity result: " + resultCode);
            }

            return dialogResult;
        }

        /// <summary>
        /// Translates Play Core's AppUpdateType into its corresponding public-facing AppUpdateType.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided AppUpdateType does not have a corresponding value in Unity AppUpdateType.
        /// </exception>
        public static AppUpdateType TranslatePlayCoreAppUpdateType(int playCoreAppUpdateType)
        {
            AppUpdateType appUpdateType;
            if (!PlayCoreToAppUpdateType.TryGetValue(playCoreAppUpdateType, out appUpdateType))
            {
                throw new NotImplementedException("Unexpected appUpdateType: " + appUpdateType);
            }

            return appUpdateType;
        }
    }
}