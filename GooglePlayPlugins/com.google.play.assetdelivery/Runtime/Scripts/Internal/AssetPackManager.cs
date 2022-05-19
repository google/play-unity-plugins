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
using Google.Play.Common;
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Wraps Play Core's AssetPackManager which manages downloads of asset packs.
    /// </summary>
    internal class AssetPackManager : IDisposable
    {
        private AndroidJavaObject _javaAssetPackManager;

        public AssetPackManager()
        {
            const string factoryClassName = PlayCoreConstants.AssetPackPackagePrefix + "AssetPackManagerFactory";

            using (var activity = UnityPlayerHelper.GetCurrentActivity())
            using (var assetPackManagerFactory = new AndroidJavaClass(factoryClassName))
            {
                _javaAssetPackManager = assetPackManagerFactory.CallStatic<AndroidJavaObject>("getInstance", activity);
            }

            if (_javaAssetPackManager == null)
            {
                throw new NullReferenceException("Play Core returned null AssetPackManager");
            }
        }

        /// <summary>
        /// Registers a listener that will be called when a pack download changes state.
        /// Listeners should be subsequently unregistered using <see cref="UnregisterListener"/>
        /// </summary>
        /// <param name="listener">
        /// An AndroidJavaProxy representing a java object of class:
        /// com.google.android.play.core.assetpacks.AssetPackStateUpdateListener
        /// </param>
        public void RegisterListener(AndroidJavaProxy listener)
        {
            _javaAssetPackManager.Call("registerListener", listener);
        }

        /// <summary>
        /// Calls Play Core to unregister a listener previously added using <see cref="RegisterListener"/>
        /// </summary>
        /// <param name="listener">
        /// An AndroidJavaProxy representing a java object of class:
        /// com.google.android.play.core.assetpacks.AssetPackStateUpdateListener
        /// </param>
        public void UnregisterListener(AndroidJavaProxy listener)
        {
            _javaAssetPackManager.Call("unregisterListener", listener);
        }

        /// <summary>
        /// Calls Play Core to start a download for the specified asset packs.
        /// </summary>
        /// <param name="assetPackNames">The asset packs to include in the download request.</param>
        /// <returns>
        /// A wrapped Play Core task, which will return a AndroidJavaObject representing an
        /// AssetPackStates. The caller is responsible for disposing this task.
        /// </returns>
        public PlayServicesTask<AndroidJavaObject> Fetch(params string[] assetPackNames)
        {
            using (var request = BuildAssetPackList(assetPackNames))
            {
                var javaTask = _javaAssetPackManager.Call<AndroidJavaObject>("fetch", request);
                return new PlayServicesTask<AndroidJavaObject>(javaTask);
            }
        }

        /// <summary>
        /// Calls Play Core to get the states of the specified packs.
        /// </summary>
        /// <returns>
        /// A wrapped Play Core task, which will return a AndroidJavaObject representing an
        /// AssetPackStates. The caller is responsible for disposing this task.
        /// </returns>
        public PlayServicesTask<AndroidJavaObject> GetPackStates(params string[] assetPackNames)
        {
            using (var packList = BuildAssetPackList(assetPackNames))
            {
                var javaTask = _javaAssetPackManager.Call<AndroidJavaObject>("getPackStates", packList);
                return new PlayServicesTask<AndroidJavaObject>(javaTask);
            }
        }

        /// <summary>
        /// Calls Play Core to get the location of the specified asset pack.
        /// This should contain the AssetBundle associated with the specified asset pack.
        /// </summary>
        /// <returns>
        /// The AssetPackLocation containing the contents of the specified asset pack, or null if the most up-to-date
        /// version of this asset pack isn't available on disk.
        /// </returns>
        public AssetPackLocation GetPackLocation(string assetPackName)
        {
            var javaPackLocation = _javaAssetPackManager.Call<AndroidJavaObject>("getPackLocation", assetPackName);
            return PlayCoreHelper.IsNull(javaPackLocation) ? null : new AssetPackLocation(javaPackLocation);
        }

        /// <summary>
        /// Calls Play Core to get the location of the specified asset within the specified asset pack.
        /// </summary>
        /// <returns>
        /// The AssetLocation of the specified asset or null if the most up-to-date
        /// version of this asset pack isn't available on disk.
        /// </returns>
        public AssetLocation GetAssetLocation(string assetPackName, string assetPath)
        {
            var javaAssetLocation =
                _javaAssetPackManager.Call<AndroidJavaObject>("getAssetLocation", assetPackName, assetPath);
            return PlayCoreHelper.IsNull(javaAssetLocation) ? null : new AssetLocationImpl(javaAssetLocation);
        }

        /// <summary>
        /// Requests to cancel the download for a list of packs.
        /// </summary>
        /// <returns>
        /// Returns a AndroidJavaObject representing an AssetPackStates which contains the new states
        /// of all specified assetPackNames.
        /// </returns>
        public AndroidJavaObject Cancel(params string[] assetPackNames)
        {
            using (var packList = BuildAssetPackList(assetPackNames))
            {
                var javaPackStates = _javaAssetPackManager.Call<AndroidJavaObject>("cancel", packList);
                return javaPackStates;
            }
        }

        /// <summary>
        /// Deletes the specified asset pack from internal storage of the app.
        /// Deleted packs will not be re-downloaded after update.
        /// If the asset pack is currently being downloaded or installed, this method will not cancel the process.
        /// </summary>
        /// <returns>
        /// A PlayServicesTask representing a Task{Void} that will only be successful if files were successfully
        /// deleted.
        /// </returns>
        public PlayServicesTask<AndroidJavaObject> RemovePack(string assetPackName)
        {
            var javaTask = _javaAssetPackManager.Call<AndroidJavaObject>("removePack", assetPackName);
            return new PlayServicesTask<AndroidJavaObject>(javaTask);
        }

        /// <summary>
        /// Calls Play Core to show a confirmation dialog for all downloads that are currently in the
        /// <see cref="AssetDeliveryStatus.WaitingForWifi"/> state. If the user accepts the dialog, then those
        /// packs are downloaded over cellular data.
        /// </summary>
        /// <returns>
        /// Returns a <see cref=" PlayServicesTask{ActivityResult}"/> that completes once the dialog has been accepted,
        /// denied or closed. A successful task result contains one of the following values:
        /// <ul>
        ///   <li><see cref="ActivityResult.ResultOk"/> if the user accepted.</li>
        ///   <li><see cref="ActivityResult.ResultCancelled"/> if the user denied or the dialog has been closed in
        ///       any other way (e.g. backpress).</li>
        /// </ul>
        /// </returns>
        public PlayServicesTask<int> ShowCellularDataConfirmation()
        {
            var task = _javaAssetPackManager.Call<AndroidJavaObject>("showCellularDataConfirmation",
                UnityPlayerHelper.GetCurrentActivity());
            return new PlayServicesTask<int>(task);
        }

        // Returns a list of asset pack names.
        private static AndroidJavaObject BuildAssetPackList(params string[] assetPackNames)
        {
            var assetPackList = new AndroidJavaObject("java.util.ArrayList");
            foreach (var packName in assetPackNames)
            {
                assetPackList.Call<bool>("add", packName);
            }

            return assetPackList;
        }

        public void Dispose()
        {
            _javaAssetPackManager.Dispose();
        }
    }
}