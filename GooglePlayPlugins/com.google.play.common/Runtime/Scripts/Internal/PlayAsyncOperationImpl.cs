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

namespace Google.Play.Common.Internal
{
    /// <summary>
    /// Provides implementation for <see cref="PlayAsyncOperation{TResult,TError}"/>.
    /// </summary>
    public abstract class PlayAsyncOperationImpl<TResult, TError> : PlayAsyncOperation<TResult, TError>
    {
        private TResult _result;

        public override event Action<PlayAsyncOperation<TResult, TError>> Completed
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

        public void SetResult(TResult result)
        {
            _result = result;
            IsDone = true;
            InvokeCompletedEvent();
        }

        public void SetError(TError errorCode)
        {
            Error = errorCode;
            IsDone = true;
            InvokeCompletedEvent();
        }

        protected override TResult GetResultImpl()
        {
            return _result;
        }
    }
}