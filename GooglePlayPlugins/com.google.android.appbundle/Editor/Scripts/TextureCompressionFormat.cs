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

namespace Google.Android.AppBundle.Editor
{
    /// <summary>
    /// The texture compression formats that can be recognized in an asset pack folder name.
    /// </summary>
    public enum TextureCompressionFormat
    {
        /// <summary>
        /// No texture compression format targeting was selected or is applicable.
        /// </summary>
        Default,

        /// <summary>
        /// Adaptive Scalable Texture Compression.
        /// See https://www.khronos.org/registry/OpenGL/extensions/OES/OES_texture_compression_astc.txt.
        /// </summary>
        Astc,

        /// <summary>
        /// S3 Texture Compression.
        /// See https://www.khronos.org/registry/OpenGL/extensions/EXT/EXT_texture_compression_s3tc.txt.
        /// </summary>
        Dxt1,

        /// <summary>
        /// Ericsson Texture Compression.
        /// See https://en.wikipedia.org/wiki/Ericsson_Texture_Compression.
        /// </summary>
        Etc1,

        /// <summary>
        /// Ericsson Texture Compression 2.
        /// See https://www.khronos.org/registry/OpenGL/specs/es/3.0/es_spec_3.0.pdf (Appendix C).
        /// </summary>
        Etc2,

        /// <summary>
        /// PowerVR Texture Compression.
        /// See https://www.khronos.org/registry/OpenGL/extensions/IMG/IMG_texture_compression_pvrtc.txt.
        /// </summary>
        Pvrtc
    }
}