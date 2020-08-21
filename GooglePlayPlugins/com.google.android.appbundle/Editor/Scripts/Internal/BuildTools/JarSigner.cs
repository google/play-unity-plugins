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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Google.Android.AppBundle.Editor.Internal.PlayServices;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEditor;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Provides methods that call the JDK tool "jarsigner" to sign an Android App Bundle.
    /// </summary>
    public class JarSigner : IBuildTool
    {
        private readonly JavaUtils _javaUtils;

        private bool _useCustomKeystore;
        private string _keystoreName;
        private string _keystorePass;
        private string _keyaliasName;
        private string _keyaliasPass;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JarSigner(JavaUtils javaUtils)
        {
            _javaUtils = javaUtils;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            if (!_javaUtils.Initialize(buildToolLogger))
            {
                return false;
            }

            if (_javaUtils.JarSignerBinaryPath == null)
            {
                buildToolLogger.DisplayErrorDialog(
                    "Unable to locate jarsigner. Check that a recent version of the JDK " +
                    "is installed and check the Console log for more details on the error.");
                return false;
            }

            // Cache PlayerSettings on the main thread so they can be accessed from a background thread.
            _keystoreName = PlayerSettings.Android.keystoreName;
            _keystorePass = PlayerSettings.Android.keystorePass;
            _keyaliasName = PlayerSettings.Android.keyaliasName;
            _keyaliasPass = PlayerSettings.Android.keyaliasPass;
#if UNITY_2019_1_OR_NEWER
            _useCustomKeystore = PlayerSettings.Android.useCustomKeystore;
#else
            _useCustomKeystore = !string.IsNullOrEmpty(_keystoreName) && !string.IsNullOrEmpty(_keyaliasName);
#endif

            return true;
        }

        /// <summary>
        /// Returns true if the Android Player Settings have a Custom Keystore configured, false otherwise.
        /// </summary>
        public bool UseCustomKeystore
        {
            get { return _useCustomKeystore; }
        }

        /// <summary>
        /// Synchronously calls the jarsigner tool to sign the specified ZIP file using a custom keystore.
        /// This can be used to sign Android App Bundles.
        /// </summary>
        /// <returns>An error message if there was a problem running jarsigner, or null if successful.</returns>
        public string Sign(string zipFilePath)
        {
            if (!_useCustomKeystore)
            {
                throw new InvalidOperationException("Unexpected request to sign without a custom keystore");
            }

            if (string.IsNullOrEmpty(_keystoreName))
            {
                return "Unable to sign since the keystore file path is unspecified";
            }

            if (!File.Exists(_keystoreName))
            {
                return string.Format("Failed to locate keystore file: {0}", _keystoreName);
            }

            var arguments = string.Format(
                "-keystore {0} {1} {2}",
                CommandLine.QuotePath(_keystoreName),
                CommandLine.QuotePath(zipFilePath),
                CommandLine.QuotePath(_keyaliasName));

            var promptToPasswordDictionary = new Dictionary<string, string>
            {
                {"Enter Passphrase for keystore:", _keystorePass},
                // Example keyalias password prompt: "Enter key password for myalias:"
                {"Enter key password for .+:", _keyaliasPass}
            };
            var responder = new JarSignerResponder(promptToPasswordDictionary);
            var result = CommandLine.Run(_javaUtils.JarSignerBinaryPath, arguments, ioHandler: responder.AggregateLine);
            return result.exitCode == 0 ? null : result.message;
        }

        /// <summary>
        /// Checks jarsigner's stderr for password prompts and provides the associated password to jarsigner's stdin.
        /// This is more secure than providing passwords on the command line (where passwords are visible to process
        /// listing tools like "ps") or using file-based password input (where passwords are written to disk).
        /// </summary>
        private class JarSignerResponder : CommandLine.LineReader
        {
            private readonly Dictionary<Regex, string> _promptToPasswordDictionary;

            public JarSignerResponder(Dictionary<string, string> promptToPasswordDictionary)
            {
                _promptToPasswordDictionary =
                    promptToPasswordDictionary.ToDictionary(
                        kvp => RegexHelper.CreateCompiled(kvp.Key), kvp => kvp.Value);
                LineHandler += CheckAndRespond;
            }

            private void CheckAndRespond(Process process, StreamWriter stdin, CommandLine.StreamData data)
            {
                if (process.HasExited)
                {
                    return;
                }

                // The password prompt text won't have a trailing newline, so read ahead on stdout to locate it.
                var stderrData = GetBufferedData(CommandLine.StandardErrorStreamDataHandle);
                var stderrText = Aggregate(stderrData).text;
                var password = _promptToPasswordDictionary
                    .Where(kvp => kvp.Key.IsMatch(stderrText))
                    .Select(kvp => kvp.Value)
                    .FirstOrDefault();
                if (password == null)
                {
                    return;
                }

                Flush();
                // Use UTF8 to support non ASCII passwords
                var passwordBytes = Encoding.UTF8.GetBytes(password + Environment.NewLine);
                stdin.BaseStream.Write(passwordBytes, 0, passwordBytes.Length);
                stdin.BaseStream.Flush();
            }
        }
    }
}