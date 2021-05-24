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
using Google.Play.AppUpdate.Internal;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AppUpdate
{
    // Note: This wrapper around Java AppUpdateInfo exists to hide the AndroidJavaObject from the public API.
    /// <summary>
    /// Contains information about in-app update availability and installation progress.
    /// </summary>
    public class AppUpdateInfo
    {
        private readonly AndroidJavaObject _javaAppUpdateInfo;

        /// <summary>
        /// Returns the app version code of an available or in-progress update, or an arbitrary value if no update
        /// is available.
        /// </summary>
        public int AvailableVersionCode
        {
            get { return _javaAppUpdateInfo.Call<int>("availableVersionCode"); }
        }

        /// <summary>
        /// Returns update availability.
        /// </summary>
        public UpdateAvailability UpdateAvailability
        {
            get
            {
                return AppUpdatePlayCoreTranslator.TranslatePlayCoreUpdateAvailabilities(
                    _javaAppUpdateInfo.Call<int>("updateAvailability"));
            }
        }

        /// <summary>
        /// Returns update status.
        /// </summary>
        public AppUpdateStatus AppUpdateStatus
        {
            get
            {
                return AppUpdatePlayCoreTranslator.TranslatePlayCoreUpdateStatus(
                    _javaAppUpdateInfo.Call<int>("installStatus"));
            }
        }

        /// <summary>
        /// Returns the number of days that the Google Play Store app on the user's device has known about an
        /// available update, or null if update or staleness information is unavailable.
        /// </summary>
        public int? ClientVersionStalenessDays
        {
            get
            {
                var clientVersionStalenessDaysJavaObject =
                    _javaAppUpdateInfo.Call<AndroidJavaObject>("clientVersionStalenessDays");
                if (PlayCoreHelper.IsNull(clientVersionStalenessDaysJavaObject))
                {
                    return null;
                }

                // Convert the java.lang.Integer object to an int primitive.
                return clientVersionStalenessDaysJavaObject.Call<int>("intValue");
            }
        }

        /// <summary>
        /// Returns the priority for this update, as defined by the developer using the "inAppUpdatePriority" field
        /// of the Google Play Developer API's
        /// <a href="https://developers.google.com/android-publisher/api-ref/rest/v3/edits.tracks">edits.track</a>.
        /// </summary>
        public int UpdatePriority
        {
            get { return _javaAppUpdateInfo.Call<int>("updatePriority"); }
        }

        /// <summary>
        /// Returns the number of bytes already downloaded for the update.
        /// </summary>
        public ulong BytesDownloaded
        {
            get { return _javaAppUpdateInfo.Call<ulong>("bytesDownloaded"); }
        }

        /// <summary>
        /// Returns the total number of bytes to download for the update.
        /// </summary>
        public ulong TotalBytesToDownload
        {
            get { return _javaAppUpdateInfo.Call<ulong>("totalBytesToDownload"); }
        }

        /// <summary>
        /// Returns true if an update with the specified options is allowed, false otherwise.
        /// </summary>
        /// <param name="appUpdateOptions">The type of update to perform.</param>
        public bool IsUpdateTypeAllowed(AppUpdateOptions appUpdateOptions)
        {
            return _javaAppUpdateInfo.Call<bool>("isUpdateTypeAllowed", appUpdateOptions.GetJavaAppUpdateOptions());
        }

        /// <summary>
        /// Returns a debug string containing all fields of this class.
        /// </summary>
        public override string ToString()
        {
            var infoDescription = new StringBuilder();
            infoDescription.AppendFormat("version={0} ", AvailableVersionCode);
            infoDescription.AppendFormat("status={0} ", AppUpdateStatus);
            infoDescription.AppendFormat("availability={0} ", UpdateAvailability);
            infoDescription.AppendFormat("downloaded={0} ", BytesDownloaded);
            infoDescription.AppendFormat("priority={0} ", UpdatePriority);
            infoDescription.AppendFormat("staleness={0} ", ClientVersionStalenessDays);
            infoDescription.AppendFormat("totalBytes={0} ", TotalBytesToDownload);
            infoDescription.AppendFormat("immediateAllowed={0}",
                IsUpdateTypeAllowed(AppUpdateOptions.ImmediateAppUpdateOptions()));
            infoDescription.AppendFormat("flexibleAllowed={0}",
                IsUpdateTypeAllowed(AppUpdateOptions.FlexibleAppUpdateOptions()));
            return infoDescription.ToString();
        }

        internal AppUpdateInfo(AndroidJavaObject javaAppUpdateInfo)
        {
            _javaAppUpdateInfo = javaAppUpdateInfo;
        }

        internal AndroidJavaObject GetJavaUpdateInfo()
        {
            return _javaAppUpdateInfo;
        }
    }
}