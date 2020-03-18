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
using UnityEngine;

namespace Google.Play.Core.Internal
{
    /// <summary>
    /// A collection of helper methods for interfacing with the Play Core library.
    /// </summary>
    public static class PlayCoreHelper
    {
        /// <summary>
        /// Converts the specified AndroidJavaObject representing a "java.util.List" into a C# List.
        /// </summary>
        /// <typeparam name="TAndroidJava">
        /// The type of the objects in the returned list.
        /// Must be a primitive type or an AndroidJavaObject.
        /// </typeparam>
        /// <returns>A List with the contents of the provided Java list</returns>
        public static List<TAndroidJava> ConvertJavaList<TAndroidJava>(AndroidJavaObject javaList)
        {
            var count = javaList.Call<int>("size");

            var results = new List<TAndroidJava>();
            for (int i = 0; i < count; i++)
            {
                results.Add(javaList.Call<TAndroidJava>("get", i));
            }

            return results;
        }

        /// <summary>
        /// Converts the specified AndroidJavaObject representing a "java.util.Map" into a C# Dictionary.
        /// </summary>
        /// <typeparam name="TAndroidJavaKey">
        /// The type of the keys in the returned dictionary.
        /// Must be a primitive type or an AndroidJavaObject.
        /// </typeparam>
        /// <typeparam name="TAndroidJavaValue">
        /// The type of the values in the returned dictionary.
        /// Must be a primitive type or an AndroidJavaObject.
        /// </typeparam>
        /// <returns>A Dictionary with the contents of the provided Java map</returns>
        public static Dictionary<TAndroidJavaKey, TAndroidJavaValue> ConvertJavaMap<TAndroidJavaKey, TAndroidJavaValue>(
            AndroidJavaObject javaMap)
        {
            using (var entrySet = javaMap.Call<AndroidJavaObject>("entrySet"))
            using (var entrySetIterator = entrySet.Call<AndroidJavaObject>("iterator"))
            {
                var results = new Dictionary<TAndroidJavaKey, TAndroidJavaValue>();
                while (entrySetIterator.Call<bool>("hasNext"))
                {
                    using (var entry = entrySetIterator.Call<AndroidJavaObject>("next"))
                    {
                        var key = entry.Call<TAndroidJavaKey>("getKey");
                        var value = entry.Call<TAndroidJavaValue>("getValue");
                        results.Add(key, value);
                    }
                }

                return results;
            }
        }

        /// <summary>
        /// Converts the specified AndroidJavaObject representing a "java.lang.String" into a C# string.
        /// </summary>
        public static string ConvertJavaString(AndroidJavaObject javaString)
        {
            return IsNull(javaString) ? null : javaString.Call<string>("toString");
        }

        /// <summary>
        /// Returns true if the specified <see cref="AndroidJavaObject"/> is null or if the underlying Java object is
        /// null, false otherwise.
        /// </summary>
        public static bool IsNull(AndroidJavaObject javaObject)
        {
            return javaObject == null || javaObject.GetRawObject().ToInt32() == 0;
        }
    }
}