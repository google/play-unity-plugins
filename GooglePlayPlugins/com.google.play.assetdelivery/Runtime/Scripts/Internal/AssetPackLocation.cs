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
using Google.Play.Core.Internal;
using UnityEngine;

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Wraps Play Core's AssetPackLocation which represents the location of an asset pack on disk.
    /// </summary>
    internal class AssetPackLocation
    {
        /// <summary>
        /// The path to the directory which will contain the contents of an asset pack, if the asset pack is available
        /// on disk.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Returns whether the pack is installed as an APK or unpackaged into a folder on the filesystem.
        /// </summary>
        public AssetPackStorageMethod PackStorageMethod { get; private set; }

        /// <summary>
        /// Creates an AssetPackLocation with all the fields of the underlying Java object, disposing the Java
        /// object in the process.
        /// </summary>
        public AssetPackLocation(AndroidJavaObject packLocation)
        {
            using (packLocation)
            {
                // Called with AndroidJavaObject instead of string because path may be null.
                var javaPathString = packLocation.Call<AndroidJavaObject>("path");
                Path = PlayCoreHelper.ConvertJavaString(javaPathString);
                PackStorageMethod = (AssetPackStorageMethod) packLocation.Call<int>("packStorageMethod");
            }
        }

        public override string ToString()
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendFormat("path: {0}\n", Path);
            stateDescription.AppendFormat("packStorageMethod: {0}\n", PackStorageMethod);
            return stateDescription.ToString();
        }
    }
}