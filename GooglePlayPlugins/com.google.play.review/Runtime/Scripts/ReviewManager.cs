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

using Google.Play.Common;
using Google.Play.Review.Internal;

namespace Google.Play.Review
{
    /// <summary>
    /// Manages operations for requesting and launching the In-App Review flow.
    /// </summary>
    public class ReviewManager
    {
        /// <summary>
        /// Translation of Java ReviewErrorCode from:
        /// <a
        /// href="https://developer.android.com/reference/com/google/android/play/core/review/model/ReviewErrorCode"></a>
        /// </summary>
        private const int JavaReviewErrorCodePlayNotFound = -1;

#if !UNITY_EDITOR
        private readonly ReviewPlayCoreTaskManager _reviewPlayCoreTaskManager;
#endif

        public ReviewManager()
        {
#if !UNITY_EDITOR
            _reviewPlayCoreTaskManager = new ReviewPlayCoreTaskManager();
#endif
        }

        /// <summary>
        /// Retrieves all the needed information to launch the in-app review flow.
        /// Needs to be called before <see cref="LaunchReviewFlow"/> to obtain
        /// a <see cref="PlayReviewInfo"/> which is required to launch the
        /// in-app review flow.
        /// </summary>
        public PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> RequestReviewFlow()
        {
#if UNITY_EDITOR
            var operation = new ReviewAsyncOperation<PlayReviewInfo>();
            operation.SetResult(new PlayReviewInfo(null));
            return operation;
#else
            return RequestReviewFlowInternal();
#endif
        }

        /// <summary>
        /// Launches and displays the in-app review flow to the user.
        /// Returns a <see cref="PlayAsyncOperation"/> that will be marked IsDone when
        /// the in-app review dialog is closed.
        /// </summary>
        public PlayAsyncOperation<VoidResult, ReviewErrorCode> LaunchReviewFlow(
            PlayReviewInfo reviewInfo)
        {
#if UNITY_EDITOR
            var operation = new ReviewAsyncOperation<VoidResult>();
            operation.SetResult(new VoidResult());
            return operation;
#else
            return LaunchReviewFlowInternal(reviewInfo);
#endif
        }

#if !UNITY_EDITOR
        private PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> RequestReviewFlowInternal()
        {
            var operation = new ReviewAsyncOperation<PlayReviewInfo>();
            var requestFlowTask = _reviewPlayCoreTaskManager.RequestReviewFlow();
            requestFlowTask.RegisterOnSuccessCallback(reviewInfo =>
            {
                operation.SetResult(new PlayReviewInfo(reviewInfo));
                requestFlowTask.Dispose();
            });
            requestFlowTask.RegisterOnFailureCallback((reason, errorCode) =>
            {
                if (errorCode == JavaReviewErrorCodePlayNotFound) {
                    operation.SetError(ReviewErrorCode.PlayStoreNotFound);
                } else {
                    operation.SetError(ReviewErrorCode.ErrorRequestingFlow);
                }
                requestFlowTask.Dispose();
            });
            return operation;
        }

        private PlayAsyncOperation<VoidResult, ReviewErrorCode> LaunchReviewFlowInternal(PlayReviewInfo reviewInfo)
        {
            var operation = new ReviewAsyncOperation<VoidResult>();
            var requestFlowTask =
                _reviewPlayCoreTaskManager.LaunchReviewFlow(reviewInfo.GetReviewInfo());
            requestFlowTask.RegisterOnSuccessCallback(result =>
            {
                operation.SetResult(new VoidResult());
                requestFlowTask.Dispose();
            });
            requestFlowTask.RegisterOnFailureCallback((reason, errorCode) =>
            {
                operation.SetError(ReviewErrorCode.ErrorLaunchingFlow);
                requestFlowTask.Dispose();
            });
            return operation;
        }
#endif
    }
}