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
using Google.Play.AppUpdate.Internal;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AppUpdate
{
    /// <summary>
    /// Options used to configure an in-app update, including <see cref="AppUpdateType"/>.
    /// </summary>
    public class AppUpdateOptions
    {
        private readonly AndroidJavaObject _javaAppUpdateOptions;

        /// <summary>
        /// Creates an <see cref="AppUpdateOptions"/> configured for the Immediate <see cref="AppUpdateType"/>.
        /// </summary>
        /// <param name="allowAssetPackDeletion">Specifies whether the app installer should be allowed to delete some
        /// asset packs from the app's internal storage, in case of insufficient device storage. Defaults to false.
        /// </param>
        public static AppUpdateOptions ImmediateAppUpdateOptions(bool allowAssetPackDeletion = false)
        {
            return new AppUpdateOptions(AppUpdateType.Immediate, allowAssetPackDeletion);
        }

        /// <summary>
        /// Creates an <see cref="AppUpdateOptions"/> configured for the Flexible <see cref="AppUpdateType"/>.
        /// </summary>
        /// <param name="allowAssetPackDeletion">Specifies whether the app installer should be allowed to delete some
        /// asset packs from the app's internal storage, in case of insufficient device storage. Defaults to false.
        /// </param>
        public static AppUpdateOptions FlexibleAppUpdateOptions(bool allowAssetPackDeletion = false)
        {
            return new AppUpdateOptions(AppUpdateType.Flexible, allowAssetPackDeletion);
        }

        /// <summary>
        /// Returns the type of in-app update flow.
        /// </summary>
        public AppUpdateType AppUpdateType
        {
            get
            {
                return AppUpdatePlayCoreTranslator.TranslatePlayCoreAppUpdateType(
                    _javaAppUpdateOptions.Call<int>("appUpdateType"));
            }
        }

        /// <summary>
        /// Returns whether the app installer should be allowed to delete some asset packs from the app's internal
        /// storage before attempting to update the app, for example if disk space is limited.
        /// </summary>
        public bool AllowAssetPackDeletion
        {
            get { return _javaAppUpdateOptions.Call<bool>("allowAssetPackDeletion"); }
        }

        internal AndroidJavaObject GetJavaAppUpdateOptions()
        {
            return _javaAppUpdateOptions;
        }

        private AppUpdateOptions(AppUpdateType appUpdateType, bool allowAssetPackDeletion)
        {
            const string appUpdateOptionsClassName =
                PlayCoreConstants.PlayCorePackagePrefix + "appupdate.AppUpdateOptions";

            using (var appUpdateOptionsClass = new AndroidJavaClass(appUpdateOptionsClassName))
            {
                var builder = appUpdateOptionsClass.CallStatic<AndroidJavaObject>(
                    "newBuilder", (int) appUpdateType);
                builder.Call<AndroidJavaObject>("setAllowAssetPackDeletion",
                    allowAssetPackDeletion);
                _javaAppUpdateOptions = builder.Call<AndroidJavaObject>("build");
                if (_javaAppUpdateOptions == null)
                {
                    throw new NullReferenceException("Play Core returned null AppUpdateOptions");
                }
            }
        }
    }
}