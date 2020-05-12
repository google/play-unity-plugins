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
using System.Collections.Generic;
using UnityEngine;

namespace Google.Play.AssetDelivery
{
    /// <summary>
    /// An object used to monitor the asynchronous retrieval of a batch of asset packs via the Play Asset Delivery
    /// system.
    /// </summary>
    public abstract class PlayAssetPackBatchRequest : CustomYieldInstruction
    {
        /// <summary>
        /// A dictionary of asset pack requests, keyed by name, that are present in this batch.
        /// </summary>
        public IDictionary<string, PlayAssetPackRequest> Requests;

        /// <summary>
        /// Returns true if all requests in this batch are in either the <see cref="AssetDeliveryStatus.Available"/> or
        /// <see cref="AssetDeliveryStatus.Failed"/> status, false otherwise.
        /// </summary>
        public bool IsDone { get; protected set; }

        /// <summary>
        /// Event indicating that all the requests in this batch are complete. If an event handler is registered after
        /// the operation has completed, and thus after this event has been invoked, then the handler will be called
        /// synchronously.
        /// </summary>
        public virtual event Action<PlayAssetPackBatchRequest> Completed = delegate { };

        /// <summary>
        /// Implements CustomYieldInstruction's keepWaiting method,
        /// so that this request can be yielded on, in a coroutine, until it is done.
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