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
using Google.Play.AppUpdate.Internal;
using Google.Play.Common;

namespace Google.Play.AppUpdate
{
    /// <summary>
    /// Manages operations for requesting and launching an in-app update flow.
    /// </summary>
    public class AppUpdateManager
    {
        private readonly AppUpdateManagerInternal _appUpdateManagerInternal;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AppUpdateManager()
        {
            _appUpdateManagerInternal = new AppUpdateManagerInternal();
        }

        /// <summary>
        /// Makes a network call to get in-app update information. This must be called before
        /// <see cref="StartUpdate"/> to obtain an <see cref="AppUpdateInfo"/>, which is needed to start an  update.
        /// </summary>
        /// <returns>
        /// A <see cref="PlayAsyncOperation{AppUpdateInfo, AppUpdateErrorCode}"/> that returns
        /// <see cref="AppUpdateInfo"/> on successful callback or <see cref="AppUpdateErrorCode"/> on failure callback.
        /// </returns>
        public PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> GetAppUpdateInfo()
        {
            return _appUpdateManagerInternal.GetAppUpdateInfoInternal();
        }

        /// <summary>
        /// Starts an in-app update flow for the specified update type. Before calling this method use
        /// <see cref="GetAppUpdateInfo"/> to get an <see cref="AppUpdateInfo"/> object.
        /// </summary>
        /// <param name="appUpdateInfo">The result of a successful call to <see cref="GetAppUpdateInfo"/>.</param>
        /// <param name="appUpdateOptions">The type of update flow and additional options.</param>
        /// <returns>
        /// An <see cref="AppUpdateRequest"/> that can be used to monitor the requested in-app update flow.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public AppUpdateRequest StartUpdate(AppUpdateInfo appUpdateInfo, AppUpdateOptions appUpdateOptions)
        {
            return _appUpdateManagerInternal.StartUpdateInternal(appUpdateInfo, appUpdateOptions);
        }

        /// <summary>
        /// Asynchronously requests to complete a flexible in-app update flow that was started via
        /// <see cref="StartUpdate"/>. This should be called after the <see cref="AppUpdateRequest"/>
        /// succeeds. After calling this function the system installer will close the app,
        /// perform the update, and then restart the app at the updated version.
        /// </summary>
        /// <returns>
        /// A <see cref="PlayAsyncOperation{VoidResult, AppUpdateErrorCode}"/> that returns an
        /// <see cref="AppUpdateErrorCode"/> on failure.
        /// If the update succeeds, the app restarts and this callback should never fire.
        /// </returns>
        public PlayAsyncOperation<VoidResult, AppUpdateErrorCode> CompleteUpdate()
        {
            return _appUpdateManagerInternal.CompleteUpdateInternal();
        }
    }
}