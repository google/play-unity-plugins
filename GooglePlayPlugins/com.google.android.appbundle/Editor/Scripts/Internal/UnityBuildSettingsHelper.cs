// Copyright 2021 Google LLC
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
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Helper class for retrieving values set in Unity's build settings window.
    /// </summary>
    public static class UnityBuildSettingsHelper
    {
        /// <summary>
        /// An exception indicating that something went wrong while calling methods via reflection.
        /// </summary>
        public class ReflectionException : Exception
        {
            public ReflectionException(string message) : base(message)
            {
            }

            public ReflectionException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }

        /// <summary>
        /// The options specified in Unity's build settings window under "Compression Method".
        /// The values match the values returned by EditorUserBuildSettings.GetCompressionType().
        /// </summary>
        private enum UnityCompressionType
        {
            Default = 0,
            Lz4 = 2,
            Lz4Hc = 3
        }

        /// <summary>
        /// A list of expected Exceptions that may occur while using Type.GetMethod and MethodInfo.Invoke to call
        /// methods via reflection.
        /// </summary>
        private static readonly HashSet<Type> ExpectedReflectionExceptions = new HashSet<Type>
        {
            typeof(ArgumentException),
            typeof(AmbiguousMatchException),
            typeof(TargetInvocationException),
            typeof(TargetParameterCountException),
            typeof(MethodAccessException),
            typeof(InvalidOperationException),
            typeof(NotSupportedException)
        };

        /// <summary>
        /// Gets the BuildOptions value corresponding to the compression specified in build settings,
        /// or null if "Default" is selected.
        /// </summary>
        public static BuildOptions? GetCompressionBuildOption()
        {
            switch (GetCompressionType())
            {
                case UnityCompressionType.Default:
                    return null;
                case UnityCompressionType.Lz4:
                    return BuildOptions.CompressWithLz4;
                case UnityCompressionType.Lz4Hc:
                    return BuildOptions.CompressWithLz4HC;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns the current compression specified in Unity's build settings window.
        /// </summary>
        /// <exception cref="ReflectionException">
        /// Thrown if any of the ExpectedReflectionExceptions are encountered while getting the compression type.
        /// </exception>
        private static UnityCompressionType GetCompressionType()
        {
            try
            {
                var getCompressionMethod = typeof(EditorUserBuildSettings).GetMethod("GetCompressionType",
                    BindingFlags.NonPublic | BindingFlags.Static);

                if (getCompressionMethod == null)
                {
                    throw new ReflectionException("GetCompressionType method could not be found.");
                }

                object result = getCompressionMethod.Invoke(null, new object[]
                {
                    BuildTargetGroup.Android,
                });

                return (UnityCompressionType)result;
            }
            catch (Exception e)
            {
                if (ExpectedReflectionExceptions.Contains(e.GetType()))
                {
                    throw new ReflectionException("Failed to call GetCompressionType.", e);
                }

                throw;
            }
        }
    }
}