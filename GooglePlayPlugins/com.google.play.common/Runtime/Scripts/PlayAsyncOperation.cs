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

namespace Google.Play.Common
{
    /// <summary>
    /// Represents an asynchronous operation that produces a result upon completion.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the operation.</typeparam>
    /// <typeparam name="TError">The type of error that can be encountered during this operation.</typeparam>
    public abstract class PlayAsyncOperation<TResult, TError> : CustomYieldInstruction
    {
        /// <summary>
        /// Whether or not the operation has finished.
        /// </summary>
        public bool IsDone { get; protected set; }

        /// <summary>
        /// Whether or not the operation completed without error.
        /// </summary>
        public abstract bool IsSuccessful { get; }

        /// <summary>
        /// The error that prevented this operation from completing successfully.
        /// In the case of a successful operation, this value can be ignored.
        /// </summary>
        public TError Error { get; protected set; }

        /// <summary>
        /// Event that is invoked upon operation completion. If an event handler is registered after the operation has
        /// completed, and thus after this event has been invoked, then the handler will be called synchronously.
        /// </summary>
        public virtual event Action<PlayAsyncOperation<TResult, TError>> Completed = delegate { };

        /// <summary>
        /// Implements the <a href="https://docs.unity3d.com/ScriptReference/CustomYieldInstruction-keepWaiting.html">CustomYieldInstruction.keepWaiting</a>
        /// property so that this request can be yielded on in a coroutine.
        /// </summary>
        public override bool keepWaiting
        {
            get { return !IsDone; }
        }

        /// <summary>
        /// Gets the result, if the operation is done.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws if GetResult is called before the operation is done, or if an error occured during the operation.
        /// </exception>
        public TResult GetResult()
        {
            if (!IsDone)
            {
                throw new InvalidOperationException("GetResult called before operation is done.");
            }

            if (!IsSuccessful)
            {
                throw new InvalidOperationException("GetResult called after operation failed with error: " + Error);
            }

            return GetResultImpl();
        }

        /// <summary>
        /// Invokes the <see cref="Completed"/> event.
        /// </summary>
        protected void InvokeCompletedEvent()
        {
            Completed.Invoke(this);
        }

        /// <summary>
        /// Returns the operation result.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        protected abstract TResult GetResultImpl();
    }
}
