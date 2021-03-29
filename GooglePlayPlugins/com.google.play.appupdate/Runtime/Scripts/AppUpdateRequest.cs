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
using UnityEngine;

namespace Google.Play.AppUpdate
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/CustomYieldInstruction.html">CustomYieldInstruction</a>
    /// used to monitor an ongoing in-app update.
    /// </summary>
    public abstract class AppUpdateRequest : CustomYieldInstruction
    {
        /// <summary>
        /// Event indicating that the request has completed. If an event handler is registered after the operation has
        /// completed, and thus after this event has been invoked, then the handler will be called synchronously.
        /// </summary>
        public virtual event Action<AppUpdateRequest> Completed = delegate { };

        /// <summary>
        /// Returns true if this request has finished, false otherwise.
        /// </summary>
        public bool IsDone { get; protected set; }

        /// <summary>
        /// Returns the status of this request, for example <see cref="AppUpdateStatus.Pending"/>.
        /// This will be <see cref="AppUpdateStatus.Failed"/> if an error occurred.
        /// </summary>
        public AppUpdateStatus Status { get; protected set; }

        /// <summary>
        /// Returns the error that prevented this request from completing successfully. This will be
        /// <see cref="AppUpdateErrorCode.NoError"/> in the case of a pending request or a successful update.
        /// </summary>
        public AppUpdateErrorCode Error { get; protected set; }

        /// <summary>
        /// Returns a value between 0 and 1 indicating the overall download progress of a flexible update flow.
        /// Note: If this value is 1, it doesn't mean that the update has completed, only that the
        /// Downloading stage has finished.
        ///
        /// Note: always returns 0 for an immediate update flow.
        /// </summary>
        public float DownloadProgress { get; protected set; }

        /// <summary>
        /// Returns the total number of bytes already downloaded for the update.
        /// </summary>
        public ulong BytesDownloaded { get; protected set; }

        /// <summary>
        /// Returns the total number of bytes to download for the update.
        /// </summary>
        public ulong TotalBytesToDownload { get; protected set; }

        /// <summary>
        /// Implements the <a href="https://docs.unity3d.com/ScriptReference/CustomYieldInstruction-keepWaiting.html">CustomYieldInstruction.keepWaiting</a>
        /// property so that this request can be yielded on in a coroutine.
        /// </summary>
        public override bool keepWaiting
        {
            get { return !IsDone; }
        }

        /// <summary>
        /// Invokes the <see cref="Completed"/> event.
        /// </summary>
        protected void InvokeCompletedEvent()
        {
            Completed.Invoke(this);
        }
    }
}
