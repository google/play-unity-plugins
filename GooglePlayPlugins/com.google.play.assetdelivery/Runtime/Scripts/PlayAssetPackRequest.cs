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

namespace Google.Play.AssetDelivery
{
    /// <summary>
    /// An object used to monitor the asynchronous retrieval of an asset pack via the Play Asset Delivery system.
    /// </summary>
    public abstract class PlayAssetPackRequest : CustomYieldInstruction
    {
        /// <summary>
        /// Progress between 0.0f and 1.0f indicating the overall download progress of this request.
        ///
        /// Note: If this value is equal to 1.0f, it does not mean that the request has completed,
        ///       only that the DOWNLOADING stage is finished.
        /// </summary>
        public float DownloadProgress { get; protected set; }

        /// <summary>
        /// The error that prevented this requested from completing successfully.
        /// In the case of a successful request, this will always be <see cref="AssetDeliveryErrorCode.NoError"/>.
        /// </summary>
        public AssetDeliveryErrorCode Error { get; protected set; }

        /// <summary>
        /// Returns true if this request is in the <see cref="AssetDeliveryStatus.Available"/> or
        /// <see cref="AssetDeliveryStatus.Failed"/> status, false otherwise.
        /// </summary>
        public bool IsDone { get; protected set; }

        /// <summary>
        /// An enum indicating the status of this request (downloading, downloaded, installed etc.)
        /// If the request completed successfully, this value should be <see cref="AssetDeliveryStatus.Available"/>.
        /// If an error occured, it should be <see cref="AssetDeliveryStatus.Failed"/>.
        /// </summary>
        public AssetDeliveryStatus Status { get; protected set; }

        /// <summary>
        /// Event indicating that the request is completed. If an event handler is registered after the operation has
        /// completed, and thus after this event has been invoked, then the handler will be called synchronously.
        /// </summary>
        public virtual event Action<PlayAssetPackRequest> Completed = delegate { };

        /// <summary>
        /// Implements CustomYieldInstruction's keepWaiting method,
        /// so that this request can be yielded on, in a coroutine, until it is done.
        /// </summary>
        public override bool keepWaiting
        {
            get { return !IsDone; }
        }

        /// <summary>
        /// Retrieves the location of an asset at the specified path within this asset pack.
        /// </summary>
        /// <param name="assetPath">The path within this asset pack pointing to the desired asset.</param>
        /// <returns>
        /// An <see cref="AssetLocation"/> object describing the path, offset and size of a the requested asset file
        /// within the asset pack.
        /// </returns>
        public abstract AssetLocation GetAssetLocation(string assetPath);

        /// <summary>
        /// Loads the AssetBundle located at the specified path within this asset pack.
        /// </summary>
        /// <param name="assetBundlePath">The path within this asset pack pointing to the desired AssetBundle.</param>
        /// <returns>
        /// An <a href="https://docs.unity3d.com/ScriptReference/AssetBundleCreateRequest.html">
        /// AssetBundleCreateRequest</a> that can be yielded on until the AssetBundle is loaded.
        /// </returns>
        public abstract AssetBundleCreateRequest LoadAssetBundleAsync(string assetBundlePath);

        /// <summary>
        /// Cancels the request if possible, setting Status to <see cref="AssetDeliveryStatus.Failed"/> and
        /// Error to <see cref="AssetDeliveryErrorCode.Canceled"/>.
        /// If the request is already <see cref="AssetDeliveryStatus.Available"/>
        /// or <see cref="AssetDeliveryStatus.Failed"/>, then the request is left unchanged.
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
