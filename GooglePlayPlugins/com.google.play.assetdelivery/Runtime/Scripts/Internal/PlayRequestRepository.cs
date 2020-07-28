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
    /// Repository for active requests related to Play Asset Delivery.
    /// </summary>
    internal class PlayRequestRepository
    {
        /// <summary>
        /// Contains all active requests, including the <see cref="PlayAssetPackRequestImpl"/> contained within active
        /// <see cref="PlayAssetBundleRequestImpl"/>.
        /// </summary>
        private Dictionary<string, PlayAssetPackRequestImpl> _requestsByName =
            new Dictionary<string, PlayAssetPackRequestImpl>();

        /// <summary>
        /// Contains all active asset bundle requests. Every entry should have a corresponding entry in _requestsByName
        /// for assetBundleRequest.PackRequest.
        /// </summary>
        private Dictionary<string, PlayAssetBundleRequestImpl> _assetBundleRequestsByName =
            new Dictionary<string, PlayAssetBundleRequestImpl>();

        public void AddAssetBundleRequest(PlayAssetBundleRequestImpl assetBundleRequest)
        {
            AddRequest(assetBundleRequest.PackRequest);
            _assetBundleRequestsByName.Add(assetBundleRequest.PackRequest.AssetPackName, assetBundleRequest);
        }

        public void AddRequest(PlayAssetPackRequestImpl request)
        {
            _requestsByName.Add(request.AssetPackName, request);
        }

        public void RemoveRequest(string name)
        {
            _requestsByName.Remove(name);
            _assetBundleRequestsByName.Remove(name);
        }

        public bool TryGetAssetBundleRequest(string name, out PlayAssetBundleRequestImpl assetBundleRequest)
        {
            return _assetBundleRequestsByName.TryGetValue(name, out assetBundleRequest);
        }

        public bool TryGetRequest(string name, out PlayAssetPackRequestImpl request)
        {
            return _requestsByName.TryGetValue(name, out request);
        }

        public IList<PlayAssetPackRequestImpl> GetRequestsWithStatus(AssetDeliveryStatus status)
        {
            return _requestsByName.Values.Where(request => request.Status == status).ToList();
        }

        public string[] GetActiveAssetPackNames()
        {
            return _requestsByName.Keys.ToArray();
        }

        public bool ContainsAssetBundleRequest(string name)
        {
            return _assetBundleRequestsByName.ContainsKey(name);
        }

        public bool ContainsRequest(string name)
        {
            return _requestsByName.ContainsKey(name);
        }
    }
}