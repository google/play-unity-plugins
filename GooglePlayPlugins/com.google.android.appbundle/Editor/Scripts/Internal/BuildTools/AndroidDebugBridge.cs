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

using System.IO;
using Google.Android.AppBundle.Editor.Internal.PlayServices;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Provides methods for <a href="https://developer.android.com/studio/command-line/adb">adb</a>.
    /// </summary>
    public class AndroidDebugBridge : IBuildTool
    {
        private const string AdbName = "adb";
        private const string AdbDirectory = "platform-tools";
        private readonly AndroidSdk _androidSdk;
        private string _adbPath;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AndroidDebugBridge(AndroidSdk androidSdk)
        {
            _androidSdk = androidSdk;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            if (!_androidSdk.Initialize(buildToolLogger))
            {
                return false;
            }

            _adbPath = GetAdbPath();
            return _adbPath != null;
        }

        /// <summary>
        /// Launches the default activity in the specified package.
        /// Assumes that there is exactly one adb connected device.
        /// </summary>
        /// <param name="packageName">The name of the package to be launched e.g. "com.package.name".</param>
        /// <returns>An error message if the command failed or null if successful.</returns>
        public string LaunchApp(string packageName)
        {
            // Run the Monkey tool restricted to packageName and with 1 event, thereby launching the app.
            // See https://developer.android.com/studio/test/monkey.html
            return Run("shell monkey -p {0} 1", packageName);
        }

        /// <summary>
        /// Gets the path to the adb executable associated with the androidSdk object passed into Initialize().
        /// </summary>
        public string GetAdbPath()
        {
            var adbPath = Path.Combine(_androidSdk.RootPath,
                Path.Combine(AdbDirectory, AdbName + CommandLine.GetExecutableExtension()));
            if (File.Exists(adbPath))
            {
                return adbPath;
            }

            adbPath = CommandLine.FindExecutable(AdbName);
            return File.Exists(adbPath) ? adbPath : null;
        }

        private string Run(string adbCommand, params object[] args)
        {
            var adbCommandWithArgs = string.Format(adbCommand, args);
            var result = CommandLine.Run(_adbPath, adbCommandWithArgs);
            return result.exitCode == 0 ? null : result.message;
        }
    }
}