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
using Google.Play.Common;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.Review.Internal
{
    /// <summary>
    /// Provides two <see cref="PlayCoreTask{TAndroidJava}"/> methods in order to request the launch of
    /// the in-app review dialog.
    /// </summary>
    internal class ReviewPlayCoreTaskManager : IDisposable
    {
        private readonly AndroidJavaObject _javaReviewManager;

        internal ReviewPlayCoreTaskManager()
        {
            const string factoryClassName =
                PlayCoreConstants.PlayCorePackagePrefix + "review.ReviewManagerFactory";

            using (var activity = UnityPlayerHelper.GetCurrentActivity())
            using (var managerFactory = new AndroidJavaClass(factoryClassName))
            {
                _javaReviewManager = managerFactory.CallStatic<AndroidJavaObject>("create", activity);
            }

            PlayCoreEventHandler.CreateInScene();
        }

        /// <summary>
        /// Returns a <see cref="PlayCoreTask{TAndroidJava}"/> which returns a ReviewInfo
        /// AndroidJavaObject on the registered on success callback.
        /// </summary>
        public PlayCoreTask<AndroidJavaObject> RequestReviewFlow()
        {
            var javaTask = _javaReviewManager.Call<AndroidJavaObject>("requestReviewFlow");
            return new PlayCoreTask<AndroidJavaObject>(javaTask);
        }

        /// <summary>
        /// Returns a <see cref="PlayCoreTask{TAndroidJava}"/> when the user completes the review or
        /// the dialog is dismissed.
        /// </summary>
        /// <param name="reviewInfo">The on success result of <see cref="RequestReviewFlow"/>.</param>
        public PlayCoreTask<AndroidJavaObject> LaunchReviewFlow(AndroidJavaObject reviewInfo)
        {
            var javaTask =
                _javaReviewManager.Call<AndroidJavaObject>("launchReviewFlow",
                    UnityPlayerHelper.GetCurrentActivity(), reviewInfo);
            return new PlayCoreTask<AndroidJavaObject>(javaTask);
        }

        public void Dispose()
        {
            _javaReviewManager.Dispose();
        }
    }
}
