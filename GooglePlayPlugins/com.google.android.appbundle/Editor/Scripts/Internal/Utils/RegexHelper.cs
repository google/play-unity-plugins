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

using System.Text.RegularExpressions;

namespace Google.Android.AppBundle.Editor.Internal.Utils
{
    /// <summary>
    /// Provides Regex utilities.
    /// </summary>
    public static class RegexHelper
    {
        /// <summary>
        /// Creates a Regex for the specified pattern and options, setting "Compiled" if possible.
        /// </summary>
        public static Regex CreateCompiled(string pattern, RegexOptions options = RegexOptions.None)
        {
#if !NET_2_0_SUBSET
            options |= RegexOptions.Compiled;
#endif
            return new Regex(pattern, options);
        }
    }
}