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

namespace Google.Play.AppUpdate
{
    /// <summary>
    /// In-app update API errors.
    /// </summary>
    public enum AppUpdateErrorCode
    {
        /// <summary>
        /// No error has occurred.
        /// </summary>
        NoError = 0,

        /// <summary>
        /// No error occurred; however, some update types are allowed and others are disallowed.
        /// </summary>
        NoErrorPartiallyAllowed = 1,

        /// <summary>
        /// An unknown error has occurred.
        /// </summary>
        ErrorUnknown = 2,

        /// <summary>
        /// The API isn't available on this device.
        /// </summary>
        ErrorApiNotAvailable = 3,

        /// <summary>
        /// The request that was sent by the app is malformed.
        /// </summary>
        ErrorInvalidRequest = 4,

        /// <summary>
        /// The update is unavailable to this user or device.
        /// </summary>
        ErrorUpdateUnavailable = 5,

        /// <summary>
        /// The update isn't allowed due to the current device state, for example low battery or low disk space.
        /// </summary>
        ErrorUpdateNotAllowed = 6,

        /// <summary>
        /// The update hasn't been fully downloaded yet.
        /// </summary>
        ErrorDownloadNotPresent = 7,

        /// <summary>
        /// The update is already in progress and there is no UI flow to resume.
        /// </summary>
        ErrorUpdateInProgress = 8,

        /// <summary>
        /// An internal error happened in the Play Store.
        /// </summary>
        ErrorInternalError = 9,

        /// <summary>
        /// The update was cancelled by the user.
        /// </summary>
        ErrorUserCanceled = 10,

        /// <summary>
        /// The in-app update failed, for example due to an interrupted network connection.
        /// </summary>
        ErrorUpdateFailed = 11,

        /// <summary>
        /// The Play Store app is either not installed or not the official version.
        /// </summary>
        ErrorPlayStoreNotFound = 12,

        /// <summary>
        /// The app isn't owned by any user on this device. An app is "owned" if it has been
        /// acquired from the Play Store.
        /// </summary>
        ErrorAppNotOwned = 13,
    }
}
