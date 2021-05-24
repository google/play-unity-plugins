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

using System;
using Google.Android.AppBundle.Editor.Internal.PlayServices;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Utility methods that use Java's "jar" command for zip creation/extraction since zip functionality
    /// isn't built into .NET 3.5.
    /// </summary>
    public class ZipUtils : IBuildTool
    {
        private readonly JavaUtils _javaUtils;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ZipUtils(JavaUtils javaUtils)
        {
            _javaUtils = javaUtils;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            return _javaUtils.Initialize(buildToolLogger);
        }

        /// <summary>
        /// Creates a ZIP file containing the specified file in the specified directory.
        /// </summary>
        /// <returns>null if the operation succeeded, or an error message if it failed.</returns>
        public virtual string CreateZipFile(string zipFilePath, string inputDirectoryName, string inputFileName)
        {
            if (inputFileName.Contains(" "))
            {
                throw new ArgumentException("Spaces are not supported for inputFileName.", "inputFileName");
            }

            // On windows, we can't support a directory with spaces, because jar does not support quotes around the
            // inputDirectoryName.
#if UNITY_EDITOR_WIN
            if (inputDirectoryName.Contains(" "))
            {
                throw new ArgumentException("Spaces are not supported for inputDirectoryName.", "inputDirectoryName");
            }
#else
            inputDirectoryName = CommandLine.QuotePath(inputDirectoryName);
#endif

            // Create zip file with options "0" (no per-file compression) and "M" (no JAR manifest file).
            var arguments = string.Format(
                "c0Mf {0} -C {1} {2}",
                CommandLine.QuotePath(zipFilePath),
                inputDirectoryName,
                inputFileName);
            var result = CommandLine.Run(_javaUtils.JarBinaryPath, arguments);
            return result.exitCode == 0 ? null : result.message;
        }

        /// <summary>
        /// Unzips the specified ZIP file into the specified output location.
        /// </summary>
        /// <returns>null if the operation succeeded, or an error message if it failed.</returns>
        public virtual string UnzipFile(string zipFilePath, string outputDirectoryName)
        {
            var arguments = string.Format("xf {0}", CommandLine.QuotePath(zipFilePath));
            var result = CommandLine.Run(_javaUtils.JarBinaryPath, arguments, outputDirectoryName);
            return result.exitCode == 0 ? null : result.message;
        }
    }
}