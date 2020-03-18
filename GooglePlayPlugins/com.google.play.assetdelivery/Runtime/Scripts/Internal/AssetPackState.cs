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

using System.Text;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Wraps Play Core's AssetPackState which represents the current state of an AssetPack download.
    /// </summary>
    internal class AssetPackState
    {
        /// <summary>
        /// The name of the asset pack being downloaded.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The number of bytes downloaded so far.
        /// </summary>
        public long BytesDownloaded { get; private set; }

        /// <summary>
        /// The total number of bytes that need to be downloaded.
        /// </summary>
        public long TotalBytesToDownload { get; private set; }

        /// <summary>
        /// The status of the associated download session (downloading, downloaded, installed, etc).
        /// </summary>
        public int Status { get; private set; }

        /// <summary>
        /// The error code indicating the reason the download session failed.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Creates an AssetPackState with all the fields of the underlying Java object, disposing the Java
        /// object in the process.
        /// </summary>
        public AssetPackState(AndroidJavaObject javaAssetPackState)
        {
            using (javaAssetPackState)
            {
                Name = javaAssetPackState.Call<string>("name");
                BytesDownloaded = javaAssetPackState.Call<long>("bytesDownloaded");
                TotalBytesToDownload = javaAssetPackState.Call<long>("totalBytesToDownload");
                Status = javaAssetPackState.Call<int>("status");
                ErrorCode = javaAssetPackState.Call<int>("errorCode");
            }
        }

        public override string ToString()
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendFormat("name: {0}\n", Name);
            stateDescription.AppendFormat("status: {0}\n", Status);
            stateDescription.AppendFormat("error code: {0}\n", ErrorCode);
            stateDescription.AppendFormat("bytes downloaded: {0}\n", BytesDownloaded);
            stateDescription.AppendFormat("total downloaded: {0}\n", TotalBytesToDownload);

            return stateDescription.ToString();
        }
    }
}