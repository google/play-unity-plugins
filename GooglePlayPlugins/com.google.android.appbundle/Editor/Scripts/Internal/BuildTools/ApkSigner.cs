// Copyright 2018 Google LLC
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

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Provided methods on 2017.4 and earlier that call the Android SDK build tool "apksigner" to verify whether an APK
    /// complies with <a href="https://source.android.com/security/apksigning/v2">APK Signature Scheme V2</a>.
    /// </summary>
    // TODO: Needed for 1.x API compatibility. Should be removed with 2.x.
    public class ApkSigner : IBuildTool
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ApkSigner(AndroidBuildTools androidBuildTools, JavaUtils javaUtils)
        {
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            return true;
        }
    }
}