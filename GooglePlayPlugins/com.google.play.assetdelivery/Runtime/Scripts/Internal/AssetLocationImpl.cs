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

namespace Google.Play.AssetDelivery.Internal
{
    /// <summary>
    /// Provides an internal implementation for AssetLocation.
    /// </summary>
    public class AssetLocationImpl : AssetLocation
    {
        /// <summary>
        /// Creates an AssetPackLocation with all the fields of the underlying Java object, disposing the Java
        /// object in the process.
        /// </summary>
        public AssetLocationImpl(AndroidJavaObject packLocation)
        {
            using (packLocation)
            {
                Path = packLocation.Call<string>("path");

                // Cast to uint because Unity AssetBundle loading APIs expect uint offset.
                Offset = (ulong) packLocation.Call<long>("offset");
                Size = (ulong) packLocation.Call<long>("size");
            }
        }

        public override string ToString()
        {
            var stateDescription = new StringBuilder();
            stateDescription.AppendFormat("path: {0}\n", Path);
            stateDescription.AppendFormat("offset: {0}\n", Offset);
            stateDescription.AppendFormat("size: {0}\n", Size);
            return stateDescription.ToString();
        }
    }
}