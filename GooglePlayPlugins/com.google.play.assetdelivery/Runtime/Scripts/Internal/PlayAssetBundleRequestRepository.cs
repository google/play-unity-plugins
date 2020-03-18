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

using System.Collections.Generic;
using System.Linq;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Repository for active PlayAssetBundleRequests.
    /// </summary>
    internal class PlayAssetBundleRequestRepository
    {
        // Contains all active requests.
        private Dictionary<string, PlayAssetBundleRequestImpl> _requestsByName =
            new Dictionary<string, PlayAssetBundleRequestImpl>();

        public void AddRequest(PlayAssetBundleRequestImpl request)
        {
            _requestsByName.Add(request.MainAssetBundleName, request);
        }

        public void RemoveRequest(string name)
        {
            _requestsByName.Remove(name);
        }

        public bool TryGetRequest(string name, out PlayAssetBundleRequestImpl request)
        {
            return _requestsByName.TryGetValue(name, out request);
        }

        public IList<PlayAssetBundleRequestImpl> GetRequestsWithStatus(AssetDeliveryStatus status)
        {
            return _requestsByName.Values.Where(request => request.Status == status).ToList();
        }

        public string[] GetActiveAssetBundleNames()
        {
            return _requestsByName.Keys.ToArray();
        }

        public bool ContainsRequest(string name)
        {
            return _requestsByName.ContainsKey(name);
        }
    }
}