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
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Provides methods that call the JDK tool "jarsigner" to sign an Android App Bundle.
    /// </summary>
    public class JarSigner : IBuildTool
    {
        private static readonly string AndroidDebugKeystore = Path.Combine(".android", "debug.keystore");

        private readonly JavaUtils _javaUtils;

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

            return true;
        }

        /// <summary>
        /// Returns true if JarSigner will sign the app with the Android debug keystore, false otherwise.
        /// </summary>
        public virtual bool UseDebugKeystore()
        {
            return string.IsNullOrEmpty(_keystoreName) || string.IsNullOrEmpty(_keyaliasName);
        }

        /// <summary>
        /// Synchronously calls the jarsigner tool to sign the specified ZIP file.
        /// This can be used to sign Android App Bundles.
        /// </summary>
        /// <returns>An error message if there was a problem running jarsigner, or null if successful.</returns>
        public virtual string SignZip(string zipFilePath)
        {
            string keystoreName;
            string keystorePass;
            string keyaliasName;
            string keyaliasPass;

            if (UseDebugKeystore())
            {
                Debug.Log("No keystore and/or no keyalias specified. Signing using Android debug keystore.");
                var homePath =
                    Application.platform == RuntimePlatform.WindowsEditor
                        ? Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")
                        : Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrEmpty(homePath))
                {
                    return "Failed to locate directory that contains Android debug keystore.";
                }

                keystoreName = Path.Combine(homePath, AndroidDebugKeystore);
                keystorePass = "android";
                keyaliasName = "androiddebugkey";
                keyaliasPass = "android";
            }
            else
            {
                keystoreName = _keystoreName;
                keystorePass = _keystorePass;
                keyaliasName = _keyaliasName;
                keyaliasPass = _keyaliasPass;
            }

            if (!File.Exists(keystoreName))
            {
                return string.Format("Failed to locate keystore file: {0}", keystoreName);
            }

            var arguments = string.Format(
                "-keystore {0} {1} {2}",
                CommandLine.QuotePath(keystoreName),
                CommandLine.QuotePath(zipFilePath),
                CommandLine.QuotePath(keyaliasName));

            var promptToPasswordDictionary = new Dictionary<string, string>
            {
                {"Enter Passphrase for keystore:", keystorePass},
                // Example keyalias password prompt: "Enter key password for myalias:"
                {"Enter key password for .+:", keyaliasPass}
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
