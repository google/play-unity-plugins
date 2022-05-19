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
    /// Internal implementation of <see cref="AppUpdateManager"/> that wraps Play Core's AppUpdateManager methods.
    /// </summary>
    internal class AppUpdateManagerPlayCore : IDisposable
    {
        private readonly AndroidJavaObject _javaAppUpdateManager;

        internal AppUpdateManagerPlayCore()
        {
            const string factoryClassName =
                PlayCoreConstants.PlayCorePackagePrefix + "appupdate.AppUpdateManagerFactory";

            using (var activity = UnityPlayerHelper.GetCurrentActivity())
            using (var managerFactory = new AndroidJavaClass(factoryClassName))
            {
                _javaAppUpdateManager = managerFactory.CallStatic<AndroidJavaObject>("create", activity);
            }

            if (_javaAppUpdateManager == null)
            {
                throw new NullReferenceException("Play Core returned null AppUpdateManager");
            }

            PlayCoreEventHandler.CreateInScene();
        }

        /// <summary>
        /// Registers a listener for this app that receives state changes for self-update operations.
        /// Listeners should be subsequently cleared using <see cref="UnregisterListener"/>.
        /// </summary>
        /// <param name="listener">
        /// An AndroidJavaProxy representing a java object of class:
        /// com.google.android.play.core.appupdate.InstallStateUpdatedListener
        /// </param>
        public void RegisterListener(AndroidJavaProxy listener)
        {
            _javaAppUpdateManager.Call("registerListener", listener);
        }

        /// <summary>
        /// Removes the specified listener previously added using <see cref="RegisterListener"/>.
        /// </summary>
        public void UnregisterListener(AndroidJavaProxy listener)
        {
            _javaAppUpdateManager.Call("unregisterListener", listener);
        }

        /// <summary>
        /// Returns a <see cref=" PlayServicesTask{TAndroidJava}"/> which returns an AppUpdateInfo
        /// AndroidJavaObject on the registered on success callback.
        /// </summary>
        /// <returns>
        /// A wrapped Play Core task, which will return a AndroidJavaObject representing an
        /// AppUpdateInfo. The caller is responsible for disposing this task.
        /// </returns>
        public PlayServicesTask<AndroidJavaObject> GetAppUpdateInfo()
        {
            var javaTask = _javaAppUpdateManager.Call<AndroidJavaObject>("getAppUpdateInfo");
            return new PlayServicesTask<AndroidJavaObject>(javaTask);
        }

        /// <summary>
        /// Starts the desired update flow indicated by <param name="appUpdateOptions"> asynchronously.
        /// </summary>
        /// <param name="appUpdateInfo">The on success result of <see cref="GetAppUpdateInfo"/>.</param>
        /// <param name="appUpdateOptions"><see cref="AppUpdateOptions"/> contains update type options.</param>
        /// <returns>
        /// A <see cref=" PlayServicesTask<int>"/> which gives an int result
        /// once the dialog has been accepted, denied or closed.
        /// A successful task result contains one of the following values:
        /// <ul>
        ///   <li><see cref="ActivityResult.ResultOk"/> -1 if the user accepted.</li>
        ///   <li><see cref="ActivityResult.ResultCancelled"/> 0 if the user denied or the dialog has been closed in
        ///       any other way (e.g. backpress).</li>
        ///   <li><see cref="ActivityResult.ResultFailed"/> 1 if the activity created to achieve has failed.</li>
        /// </ul>
        /// </returns>
        public PlayServicesTask<int> StartUpdateFlow(AppUpdateInfo appUpdateInfo, AppUpdateOptions appUpdateOptions)
        {
            var javaTask =
                _javaAppUpdateManager.Call<AndroidJavaObject>("startUpdateFlow",
                    appUpdateInfo.GetJavaUpdateInfo(), UnityPlayerHelper.GetCurrentActivity(),
                    appUpdateOptions.GetJavaAppUpdateOptions());
            return new PlayServicesTask<int>(javaTask);
        }

        /// <summary>
        /// Triggers the completion of the update for a flexible update flow.
        /// </summary>
        /// <returns>
        /// A wrapped Play Core task, which will return an AndroidJavaObject of void result.
        /// </returns>
        public PlayServicesTask<VoidResult> CompleteUpdate()
        {
            var javaTask = _javaAppUpdateManager.Call<AndroidJavaObject>("completeUpdate");
            return new PlayServicesTask<VoidResult>(javaTask);
        }

        public void Dispose()
        {
            _javaAppUpdateManager.Dispose();
        }
    }
}