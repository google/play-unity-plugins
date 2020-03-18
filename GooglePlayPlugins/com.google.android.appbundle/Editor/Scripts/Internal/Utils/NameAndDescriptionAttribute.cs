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

namespace Google.Android.AppBundle.Editor.Internal.Utils
{
    /// <summary>
    /// An attribute used to annotate enums with a displayable name and description.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class NameAndDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NameAndDescriptionAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// A human-readable string name for the enum.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A human-readable string description for the enum.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Returns the attribute associated with the specified enum.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the specified enum has no attribute.</exception>
        public static NameAndDescriptionAttribute GetAttribute<T>(T source)
        {
            var attribute = GetOptionalAttribute(source);
            if (attribute == null)
            {
                throw new ArgumentException(
                    "Unexpected missing NameAndDescriptionAttribute on enum " + source, "source");
            }

            return attribute;
        }

        /// <summary>
        /// Returns the attribute associated with the specified enum, or null if the specified enum has no attribute.
        /// </summary>
        public static NameAndDescriptionAttribute GetOptionalAttribute<T>(T source)
        {
            var fieldInfo = source.GetType().GetField(source.ToString());
            var attributes = (NameAndDescriptionAttribute[]) fieldInfo.GetCustomAttributes(
                typeof(NameAndDescriptionAttribute), false);
            if (attributes.Length == 0)
            {
                return null;
            }

            if (attributes.Length > 1)
            {
                throw new ArgumentException(
                    "Unexpected number of NameAndDescriptionAttributes on enum " + source, "source");
            }

            return attributes[0];
        }
    }
}