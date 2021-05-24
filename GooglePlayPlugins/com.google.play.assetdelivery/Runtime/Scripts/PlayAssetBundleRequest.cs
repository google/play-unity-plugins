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
using UnityEngine;

namespace Google.Play.AssetDelivery
{
    /// <summary>
    /// A <a href="https://docs.unity3d.com/ScriptReference/CustomYieldInstruction.html">CustomYieldInstruction</a>
    /// used to monitor the asynchronous retrieval of an asset pack containing an AssetBundle.
    /// </summary>
    public abstract class PlayAssetBundleRequest : CustomYieldInstruction
    {
        /// <summary>
        /// Progress between 0 and 1 indicating the overall download progress of this request.
        ///
        /// Note: If this value is equal to 1, it does not mean that the request has completed,
        /// only that the Downloading stage is finished.
        /// </summary>
        public virtual float DownloadProgress { get; protected set; }

        /// <summary>
        /// The error that prevented this requested from completing successfully.
        /// In the case of a successful request, this will always be <see cref="AssetDeliveryErrorCode.NoError"/>.
        /// </summary>
        public virtual AssetDeliveryErrorCode Error { get; protected set; }

        /// <summary>
        /// Returns true if this request is in the <see cref="AssetDeliveryStatus.Loaded"/> or
        /// <see cref="AssetDeliveryStatus.Failed"/> status, false otherwise.
        /// </summary>
        public bool IsDone { get; protected set; }

        /// <summary>
        /// An enum indicating the status of this request (downloading, downloaded, installed etc.)
        /// If the request completed successfully, this value should be <see cref="AssetDeliveryStatus.Loaded"/>.
        /// If an error occured, it should be <see cref="AssetDeliveryStatus.Failed"/>.
        /// </summary>
        public virtual AssetDeliveryStatus Status { get; protected set; }

        /// <summary>
        /// The AssetBundle loaded by this request.
        /// This corresponds to the AssetBundle name passed into this request.
        /// If Status is not <see cref="AssetDeliveryStatus.Loaded"/>, this value is null.
        /// </summary>
        public AssetBundle AssetBundle { get; protected set; }

        /// <summary>
        /// Event indicating that the request is completed. If an event handler is registered after the operation has
        /// completed, and thus after this event has been invoked, then the handler will be called synchronously.
        /// </summary>
        public virtual event Action<PlayAssetBundleRequest> Completed = delegate { };

        /// <summary>
        /// Implements the <a href="https://docs.unity3d.com/ScriptReference/CustomYieldInstruction-keepWaiting.html">CustomYieldInstruction.keepWaiting</a>
        /// property so that this request can be yielded on in a coroutine.
        /// </summary>
        public override bool keepWaiting
        {
            get { return !IsDone; }
        }

        /// <summary>
        /// Cancels the request if possible, setting Status to <see cref="AssetDeliveryStatus.Failed"/> and
        /// Error to <see cref="AssetDeliveryErrorCode.Canceled"/>.
        /// If the request is already <see cref="AssetDeliveryStatus.Loading"/>,
        /// <see cref="AssetDeliveryStatus.Loaded"/> or <see cref="AssetDeliveryStatus.Failed"/>, then
        /// the request is left unchanged.
        /// </summary>
        public abstract void AttemptCancel();

        /// <summary>
        /// Invokes the <see cref="Completed"/> event.
        /// </summary>
        protected void InvokeCompletedEvent()
        {
            Completed.Invoke(this);
        }
    }
}
