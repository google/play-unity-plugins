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
    /// Provides methods that call the Android SDK build tool "apksigner" to verify whether an APK complies with
    /// <a href="https://source.android.com/security/apksigning/v2">APK Signature Scheme V2</a> and to re-sign
    /// the APK if not. Instant apps require Signature Scheme V2 starting with Android O, however some Unity versions
    /// do not produce compliant APKs. Without this "adb install --ephemeral" on an Android O device will fail with
    /// "INSTALL_PARSE_FAILED_NO_CERTIFICATES: No APK Signature Scheme v2 signature in ephemeral package".
    /// </summary>
    public class ApkSigner : IBuildTool
    {
        private static readonly string AndroidDebugKeystore = Path.Combine(".android", "debug.keystore");

        private readonly AndroidBuildTools _androidBuildTools;
        private readonly JavaUtils _javaUtils;

        private string _keystoreName;
        private string _keystorePass;
        private string _keyaliasName;
        private string _keyaliasPass;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApkSigner(AndroidBuildTools androidBuildTools, JavaUtils javaUtils)
        {
            _androidBuildTools = androidBuildTools;
            _javaUtils = javaUtils;
        }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            if (!_androidBuildTools.Initialize(buildToolLogger) || !_javaUtils.Initialize(buildToolLogger))
            {
                return false;
            }

            if (GetApkSignerJarPath() == null)
            {
                buildToolLogger.DisplayErrorDialog(
                    "Unable to locate apksigner. Check that a recent version of Android SDK " +
                    "Build-Tools is installed and check the Console log for more details on the error.");
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
        /// Returns true if ApkSigner will sign the app with the Android debug keystore, false otherwise.
        /// </summary>
        public virtual bool UseDebugKeystore()
        {
            return string.IsNullOrEmpty(_keystoreName) || string.IsNullOrEmpty(_keyaliasName);
        }

        /// <summary>
        /// Synchronously calls the apksigner tool to verify whether the specified APK uses APK Signature Scheme V2.
        /// </summary>
        /// <returns>true if the specified APK uses APK Signature Scheme V2, false otherwise</returns>
        public virtual bool Verify(string apkPath)
        {
            var arguments = string.Format(
                "-jar {0} verify {1}",
                CommandLine.QuotePath(GetApkSignerJarPath()),
                CommandLine.QuotePath(apkPath));

            var result = CommandLine.Run(_javaUtils.JavaBinaryPath, arguments);
            if (result.exitCode == 0)
            {
                Debug.Log("Verified APK Signature Scheme V2.");
                return true;
            }

            // Logging at info level since the most common failure (V2 signature missing) is normal.
            Debug.LogFormat("APK Signature Scheme V2 verification failed: {0}", result.message);
            return false;
        }

        /// <summary>
        /// Synchronously calls the apksigner tool to sign the specified APK using APK Signature Scheme V2.
        /// </summary>
        /// <returns>An error message if there was a problem running apksigner, or null if successful.</returns>
        public virtual string SignApk(string apkFilePath)
        {
            return SignFile(apkFilePath, string.Empty);
        }

        /// <summary>
        /// Synchronously calls the apksigner tool to sign the specified ZIP file using APK Signature Scheme V1,
        /// simulating jarsigner behavior. This can be used to sign Android App Bundles.
        /// </summary>
        /// <returns>An error message if there was a problem running apksigner, or null if successful.</returns>
        public virtual string SignZip(string zipFilePath)
        {
            return SignFile(zipFilePath, "--min-sdk-version 1 --v1-signing-enabled true --v2-signing-enabled false ");
        }

        private string SignFile(string filePath, string additionalArguments)
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

            // Sign the file {4} using key {2} contained in keystore file {1} using additional arguments {3}.
            // ApkSignerResponder will provide passwords using stdin; this is the default for apksigner
            // so there is no need to specify "--ks-pass" or "--key-pass" arguments.
            // ApkSignerResponder will encode the passwords with UTF8, so we specify "--pass-encoding utf-8" here.
            var arguments = string.Format(
                "-jar {0} sign --ks {1} --ks-key-alias {2} --pass-encoding utf-8 {3}{4}",
                CommandLine.QuotePath(GetApkSignerJarPath()),
                CommandLine.QuotePath(keystoreName),
                keyaliasName,
                additionalArguments,
                CommandLine.QuotePath(filePath));

            var promptToPasswordDictionary = new Dictionary<string, string>
            {
                // Example keystore password prompt: "Keystore password for signer #1: "
                {"Keystore password for signer", keystorePass},
                // Example keyalias password prompt: "Key \"androiddebugkey\" password for signer #1: "
                {"Key .+ password for signer", keyaliasPass}
            };
            var responder = new ApkSignerResponder(promptToPasswordDictionary);
            var result = CommandLine.Run(_javaUtils.JavaBinaryPath, arguments, ioHandler: responder.AggregateLine);
            return result.exitCode == 0 ? null : result.message;
        }

        private string GetApkSignerJarPath()
        {
            var newestBuildToolsPath = _androidBuildTools.GetNewestBuildToolsPath();
            if (newestBuildToolsPath == null)
            {
                return null;
            }

            var apkSignerJarPath = Path.Combine(newestBuildToolsPath, Path.Combine("lib", "apksigner.jar"));
            if (File.Exists(apkSignerJarPath))
            {
                return apkSignerJarPath;
            }

            Debug.LogErrorFormat("Failed to locate apksigner.jar at path: {0}", apkSignerJarPath);
            return null;
        }

        /// <summary>
        /// Checks apksigner's stdout for password prompts and provides the associated password to apksigner's stdin.
        /// This is more secure than providing passwords on the command line (where passwords are visible to process
        /// listing tools like "ps") or using file-based password input (where passwords are written to disk).
        /// </summary>
        private class ApkSignerResponder : CommandLine.LineReader
        {
            private readonly Dictionary<Regex, string> _promptToPasswordDictionary;

            public ApkSignerResponder(Dictionary<string, string> promptToPasswordDictionary)
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
                var stdoutData = GetBufferedData(CommandLine.StandardOutputStreamDataHandle);
                var stdoutText = Aggregate(stdoutData).text;
                var password = _promptToPasswordDictionary
                    .Where(kvp => kvp.Key.IsMatch(stdoutText))
                    .Select(kvp => kvp.Value)
                    .FirstOrDefault();
                if (password == null)
                {
                    return;
                }

                Flush();
                // UTF8 to match "--pass-encoding utf-8" argument passed to apksigner.
                var passwordBytes = Encoding.UTF8.GetBytes(password + Environment.NewLine);
                stdin.BaseStream.Write(passwordBytes, 0, passwordBytes.Length);
                stdin.BaseStream.Flush();
            }
        }
    }
}