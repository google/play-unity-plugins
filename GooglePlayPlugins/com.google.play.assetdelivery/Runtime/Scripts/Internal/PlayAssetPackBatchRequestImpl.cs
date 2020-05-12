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
using System.Linq;

namespace Google.Play.AssetDelivery.Internal
{
    public class PlayAssetPackBatchRequestImpl : PlayAssetPackBatchRequest
    {
        private readonly HashSet<string> _completedPackNames = new HashSet<string>();

        internal PlayAssetPackBatchRequestImpl(IEnumerable<PlayAssetPackRequestImpl> requests)
        {
            Requests = requests.ToDictionary(request => request.AssetPackName,
                request => request as PlayAssetPackRequest);

            foreach (var entry in Requests)
            {
                entry.Value.Completed += OnChildPackCompleted;
            }
        }

        public override event Action<PlayAssetPackBatchRequest> Completed
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

        public void OnInitializedInPlayCore()
        {
            foreach (var request in Requests.Values)
            {
                var requestImpl = request as PlayAssetPackRequestImpl;
                if (requestImpl == null)
                {
                    throw new ArgumentException("Request is not a PlayAssetPackRequestImpl");
                }

                requestImpl.OnInitializedInPlayCore();
            }
        }

        public void OnInitializationErrorOccurred(AssetDeliveryErrorCode errorCode)
        {
            foreach (var request in Requests.Values)
            {
                var requestImpl = request as PlayAssetPackRequestImpl;
                if (requestImpl == null)
                {
                    throw new ArgumentException("Request is not a PlayAssetPackRequestImpl");
                }

                requestImpl.OnErrorOccured(errorCode);
            }
        }

        private void OnChildPackCompleted(PlayAssetPackRequest request)
        {
            var requestImpl = request as PlayAssetPackRequestImpl;
            if (requestImpl == null)
            {
                throw new ArgumentException("Request is not a PlayAssetPackRequestImpl");
            }

            _completedPackNames.Add(requestImpl.AssetPackName);
            if (_completedPackNames.Count == Requests.Count && !IsDone)
            {
                IsDone = true;
                InvokeCompletedEvent();
            }
        }
    }
}