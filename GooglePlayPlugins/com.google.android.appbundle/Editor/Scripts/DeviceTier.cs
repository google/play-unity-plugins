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

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// The device tier labels that can be recognized in an asset pack folder name.
    /// </summary>
    public class DeviceTier
    {
        /// <summary>
        /// Factory method.
        /// </summary>
        /// <param name="tier">The tier level.</param>>
        /// <exception cref="ArgumentException">Thrown if the level is a negative int.</exception>
        public static DeviceTier From(int tier)
        {
            return new DeviceTier(tier);
        }

        /// <summary>
        /// Returns the tier level.
        ///
        /// Device tier levels determines the priority of what variation of app content gets
        /// served to a specific device, for device-targeted content.
        /// Tiers are evaluated from the highest to the lowest level; the highest
        /// tier matching a given device is selected for that device.
        /// </summary>
        public int Tier { get; private set; }

        /// <summary>
        /// Implicit conversion from DeviceTier to int.
        /// </summary>
        public static implicit operator int(DeviceTier deviceTier)
        {
            return deviceTier.Tier;
        }

        /// <summary>
        /// Implicit conversion from int to DeviceTier.
        /// </summary>
        public static implicit operator DeviceTier(int tier)
        {
            return new DeviceTier(tier);
        }

        /// <summary>
        /// ToString conversion.
        /// </summary>
        public override string ToString()
        {
            return Convert.ToString(Tier);
        }

        /// <summary>
        /// Equal comparision override for DeviceTier objects.
        /// </summary>
        public override bool Equals(object other)
        {
            var otherTier = other as DeviceTier;
            if (otherTier != null)
            {
                return Tier == otherTier.Tier;
            }

            return false;
        }

        /// <summary>
        /// Creates hash code for DeviceTier object from underlying tier value.
        /// </summary>
        public override int GetHashCode()
        {
            return Tier.GetHashCode();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tier">The tier level.</param>>
        /// <exception cref="ArgumentException">Thrown if the level is a negative int.</exception>
        private DeviceTier(int tier)
        {
            if (tier < 0)
            {
                throw new ArgumentException(
                    String.Format("Tier levels are required to be non-negative integers, but found {0}.", tier));
            }

            Tier = tier;
        }
    }
}