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
using Google.Play.Common;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AppUpdate.Internal
{
    /// <summary>
    /// Internal implementation of <see cref="AppUpdateManager"/> that wraps Play Core Java's methods through
    /// <see cref="AppUpdateManagerPlayCore"/> and holds the <see cref="AppUpdateHandler"/>, which is responsible for
    /// monitoring OnStateUpdateEvents to forward state changes to <see cref="AppUpdateRequest"/> object.
    /// </summary>
    internal class AppUpdateManagerInternal
    {
        private readonly AppUpdateManagerPlayCore _appUpdateManagerPlayCore;
        private readonly AppUpdateHandler _appUpdateHandler;
        private AppUpdateRequestImpl _appUpdateRequest;

        internal AppUpdateManagerInternal()
        {
            _appUpdateManagerPlayCore = new AppUpdateManagerPlayCore();
            _appUpdateHandler = AppUpdateHandler.CreateInScene(this);
            _appUpdateHandler.OnStateUpdateEvent += ProcessStateUpdate;
            PlayCoreEventHandler.CreateInScene();
        }

        /// <summary>
        /// Wrapper around Play Core Java's getAppUpdateInfo() method.
        /// </summary>
        /// <returns>
        /// A wrapped Play Core task, which will return an AndroidJavaObject representing an AppUpdateInfo.
        /// The caller is responsible for disposing this task.
        /// </returns>
        public PlayServicesTask<AndroidJavaObject> GetAppUpdateInfo()
        {
            return _appUpdateManagerPlayCore.GetAppUpdateInfo();
        }

        /// <summary>
        /// An internal implementation that obtains an <see cref="AndroidJavaObject"/> of AppUpdateInfo from Play Core
        /// Java's getAppUpdateInfo() method and converts it to an internal <see cref="AppUpdateInfo"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="PlayAsyncOperation{AppUpdateInfo, AppUpdateErrorCode}"/> that
        /// returns <see cref="AppUpdateInfo"/> on successful callback
        /// or returns <see cref="AppUpdateErrorCode"/> on failure callback.
        /// </returns>
        public PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> GetAppUpdateInfoInternal()
        {
            var appUpdateAsyncOperation = new AppUpdateAsyncOperation<AppUpdateInfo>();
            var appUpdateInfoTask = GetAppUpdateInfo();
            appUpdateInfoTask.RegisterOnSuccessCallback(javaUpdateInfo =>
            {
                appUpdateAsyncOperation.SetResult(new AppUpdateInfo(javaUpdateInfo));
                appUpdateInfoTask.Dispose();
            });
            appUpdateInfoTask.RegisterOnFailureCallback((reason, errorCode) =>
            {
                appUpdateAsyncOperation.SetError(AppUpdatePlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                appUpdateInfoTask.Dispose();
            });
            return appUpdateAsyncOperation;
        }

        /// <summary>
        /// Wrapper around Play Core Java's registerListener() method that receives state changes for self-update operations.
        /// Listener should be subsequently cleared using <see cref="UnregisterListener"/>.
        /// </summary>
        /// <param name="listener">An AndroidJavaProxy representing a java object of class:
        /// com.google.android.play.core.appupdate.InstallStateUpdatedListener
        /// </param>
        public void RegisterListener(AndroidJavaProxy listener)
        {
            _appUpdateManagerPlayCore.RegisterListener(listener);
        }

        /// <summary>
        /// Wrapper around Play Core Java's unregisterListener() method that removes the specified listener previously
        /// added using <see cref="RegisterListener"/>.
        /// </summary>
        /// <param name="listener">An AndroidJavaProxy representing a java object of class:
        /// com.google.android.play.core.appupdate.InstallStateUpdatedListener
        /// </param>
        public void UnregisterListener(AndroidJavaProxy listener)
        {
            _appUpdateManagerPlayCore.UnregisterListener(listener);
        }

        /// <summary>
        /// Creates an <see cref="AppUpdateRequest"/> that can be used to monitor the update progress and start the
        /// desired update flow indicated by <see cref="appUpdateOptions"/>.
        /// </summary>
        /// <param name="appUpdateInfo">The on success result of <see cref="GetAppUpdateInfo"/>.</param>
        /// <param name="appUpdateOptions"><see cref="AppUpdateOptions"/> contains update type options.</param>
        /// <returns>
        /// A <see cref="AppUpdateRequest"/> which can be used to monitor the asynchronous request of an update flow.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws if any of the the provided parameters are null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Throws if there is already an <see cref="AppUpdateRequest"/> in progress.
        /// </exception>
        public AppUpdateRequest StartUpdateInternal(AppUpdateInfo appUpdateInfo, AppUpdateOptions appUpdateOptions)
        {
            if (appUpdateInfo == null)
            {
                throw new ArgumentNullException("appUpdateInfo");
            }

            if (appUpdateOptions == null)
            {
                throw new ArgumentNullException("appUpdateOptions");
            }

            if (_appUpdateRequest != null)
            {
                throw new InvalidOperationException(
                    "Another update flow is already in progress.");
            }

            _appUpdateRequest = new AppUpdateRequestImpl();

            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateNotAvailable)
            {
                _appUpdateRequest.OnErrorOccurred(AppUpdateErrorCode.ErrorUpdateUnavailable);
                return _appUpdateRequest;
            }

            if (!appUpdateInfo.IsUpdateTypeAllowed(appUpdateOptions))
            {
                _appUpdateRequest.OnErrorOccurred(AppUpdateErrorCode.ErrorUpdateNotAllowed);
                return _appUpdateRequest;
            }

            _appUpdateRequest.Completed += (req) => { _appUpdateRequest = null; };
            InitiateRequest(_appUpdateRequest, appUpdateInfo, appUpdateOptions);
            return _appUpdateRequest;
        }

        /// <summary>
        /// An internal implementation that wraps Play Core Java's completeUpdate() method that triggers the completion
        /// of the update for a flexible update flow.
        /// </summary>
        /// <returns>
        /// A <see cref="PlayAsyncOperation{VoidResult, AppUpdateErrorCode}"/> that returns an
        /// <see cref="AppUpdateErrorCode"/> on failure.
        /// If the update succeeds, the app restarts and this callback never fires.
        /// </returns>
        public PlayAsyncOperation<VoidResult, AppUpdateErrorCode> CompleteUpdateInternal()
        {
            var completeUpdateAsyncOperation = new AppUpdateAsyncOperation<VoidResult>();
            var completeUpdateTask = _appUpdateManagerPlayCore.CompleteUpdate();
            completeUpdateTask.RegisterOnSuccessCallback(voidResult =>
            {
                completeUpdateAsyncOperation.SetResult(voidResult);
                completeUpdateTask.Dispose();
            });
            completeUpdateTask.RegisterOnFailureCallback((reason, errorCode) =>
            {
                completeUpdateAsyncOperation.SetError(
                    AppUpdatePlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                completeUpdateTask.Dispose();
            });
            return completeUpdateAsyncOperation;
        }

        private void InitiateRequest(AppUpdateRequestImpl request, AppUpdateInfo appUpdateInfo,
            AppUpdateOptions appUpdateOptions)
        {
            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
            {
                // If an update is already in progress, it would skip user confirmation dialog and
                // the ActivityResult for user confirmation should be set to ResultOk.
                request.SetUpdateActivityResult(ActivityResult.ResultOk);
            }

            if (request.IsDone && appUpdateOptions.AppUpdateType == AppUpdateType.Flexible)
            {
                request.OnUpdateDownloaded();
            }
            else
            {
                var requestFlowTask = _appUpdateManagerPlayCore.StartUpdateFlow(appUpdateInfo, appUpdateOptions);

                requestFlowTask.RegisterOnSuccessCallback(resultCode =>
                {
                    // Set user confirmation dialog result for the update request.
                    request.SetUpdateActivityResult(resultCode);
                    requestFlowTask.Dispose();
                });

                requestFlowTask.RegisterOnFailureCallback((reason, errorCode) =>
                {
                    Debug.LogErrorFormat("Update flow request failed: {0}", errorCode);
                    request.OnErrorOccurred(AppUpdatePlayCoreTranslator.TranslatePlayCoreErrorCode(errorCode));
                    requestFlowTask.Dispose();
                });
            }
        }

        private void ProcessStateUpdate(AppUpdateState newState)
        {
            if (_appUpdateRequest != null && newState != null)
            {
                AppUpdateRequest(_appUpdateRequest, newState);
            }
        }

        private void AppUpdateRequest(AppUpdateRequestImpl request, AppUpdateState newState)
        {
            if (AppUpdatePlayCoreTranslator.TranslatePlayCoreErrorCode(newState.ErrorCode) !=
                AppUpdateErrorCode.NoError)
            {
                request.OnErrorOccurred(request.Error);
                return;
            }

            request.UpdateState(AppUpdatePlayCoreTranslator.TranslatePlayCoreUpdateStatus(newState.Status),
                newState.BytesDownloaded,
                newState.TotalBytesToDownload);
        }
    }
}