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

using System.Text;
using UnityEngine;

namespace Google.Play.AppUpdate.Internal
{
    /// <summary>
    /// Internal implementation that wraps Play Core's InstallState which represents the current state of an AppUpdate download.
    /// </summary>
    internal class AppUpdateState
    {
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
        /// Creates an AppUpdateState with the fields of the underlying AppUpdateInfo Java object, disposing the Java
        /// object in the process.
        /// </summary>
        /// <param name="javaAppUpdateInfo">
        /// A java object of class: com.google.android.play.core.appupdate.AppUpdateInfo that contains all the fields of
        /// the com.google.android.play.core.install.InstallState class except for the InstallErrorCode field.
        /// </param>
        public AppUpdateState(AndroidJavaObject javaAppUpdateInfo)
        {
            using (javaAppUpdateInfo)
            {
                BytesDownloaded = javaAppUpdateInfo.Call<long>("bytesDownloaded");
                TotalBytesToDownload = javaAppUpdateInfo.Call<long>("totalBytesToDownload");
                Status = javaAppUpdateInfo.Call<int>("installStatus");
            }
        }

        public override string ToString()
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendFormat("status: {0} ", Status);
            stateDescription.AppendFormat("bytes downloaded: {0} ", BytesDownloaded);
            stateDescription.AppendFormat("total downloaded: {0}", TotalBytesToDownload);

            return stateDescription.ToString();
        }
    }
}