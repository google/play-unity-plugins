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

namespace Google.Play.Billing.Internal
{
    /// <summary>
    /// Sku type supported by Google Play Store.
    /// </summary>
    public sealed class SkuType
    {
        private readonly string _description;

        public static readonly SkuType Unknown = new SkuType("unknown");
        public static readonly SkuType InApp = new SkuType("inapp");
        public static readonly SkuType Subs = new SkuType("subs");

        private SkuType(string description)
        {
            _description = description;
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
