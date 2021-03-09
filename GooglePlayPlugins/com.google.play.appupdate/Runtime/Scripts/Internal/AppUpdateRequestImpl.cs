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

namespace Google.Play.AppUpdate.Internal
{
    /// <summary>
    /// Internal implementation for <see cref="AppUpdateRequest"/>.
    /// </summary>
    internal class AppUpdateRequestImpl : AppUpdateRequest
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public AppUpdateRequestImpl()
        {
            Status = AppUpdateStatus.Unknown;
            Error = AppUpdateErrorCode.NoError;
        }

        public override event Action<AppUpdateRequest> Completed
        {
            add
            {
                if (IsDone)
                {
                    value.Invoke(this);
                    return;
                }

                base.Completed += value;
            }
            remove { base.Completed -= value; }
        }

        /// <summary>
        /// Updates the current AppUpdateRequest according to fields received from the latest AppUpdateState.
        /// </summary>
        /// <param name="status">The latest <see cref="AppUpdateStatus"/> received.</param>
        /// <param name="bytesDownloaded">The number of bytes downloaded so far.</param>
        /// <param name="totalBytesToDownload">The total bytes to download.</param>
        public void UpdateState(AppUpdateStatus status, long bytesDownloaded, long totalBytesToDownload)
        {
            Status = status;
            // Due to b/160606605 totalBytesToDownload can be 0 both before and after the download.
            if (totalBytesToDownload == 0L)
            {
                bool finishedDownloading = status == AppUpdateStatus.Downloaded;
                DownloadProgress = finishedDownloading ? 1f : 0f;
            }
            else
            {
                BytesDownloaded = (ulong) bytesDownloaded;
                TotalBytesToDownload = (ulong) totalBytesToDownload;
                DownloadProgress = bytesDownloaded / (float) totalBytesToDownload;
            }

            if (Status == AppUpdateStatus.Downloaded)
            {
                OnUpdateDownloaded();
            }
        }

        /// <summary>
        /// Set request Status to a corresponding AppUpdateStatus which maps to the ActivityResult code returned from
        /// an update user confirmation dialog.
        /// </summary>
        /// <param name="resultCode">The ActivityResult code returned by user confirmation dialog.</param>
        public void SetUpdateActivityResult(int resultCode)
        {
            Status = AppUpdatePlayCoreTranslator.TranslateUpdateActivityResult(resultCode);
            switch (Status)
            {
                case AppUpdateStatus.Pending:
                    break;
                case AppUpdateStatus.Canceled:
                    OnErrorOccurred(AppUpdateErrorCode.ErrorUserCanceled);
                    break;
                case AppUpdateStatus.Failed:
                    OnErrorOccurred(AppUpdateErrorCode.ErrorUpdateFailed);
                    break;
                default:
                    OnErrorOccurred(AppUpdateErrorCode.ErrorUnknown);
                    break;
            }
        }

        /// <summary>
        /// Handles request state changes and invoke completed events when an update is downloaded.
        /// </summary>
        public void OnUpdateDownloaded()
        {
            Status = AppUpdateStatus.Downloaded;
            Error = AppUpdateErrorCode.NoError;
            DownloadProgress = 1f;
            if (IsDone)
            {
                return;
            }

            IsDone = true;
            InvokeCompletedEvent();
        }

        /// <summary>
        /// Handles request state changes and invoke completed events when an error occurs during an update request.
        /// </summary>
        public void OnErrorOccurred(AppUpdateErrorCode errorCode)
        {
            if (IsDone)
            {
                return;
            }

            IsDone = true;
            Error = errorCode;
            Status = AppUpdateStatus.Failed;
            InvokeCompletedEvent();
        }
    }
}