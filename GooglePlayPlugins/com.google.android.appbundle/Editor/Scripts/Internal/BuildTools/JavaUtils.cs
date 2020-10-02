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

using System;
using System.IO;
using Google.Android.AppBundle.Editor.Internal.PlayServices;
using UnityEditor;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Build tool for finding the path to Java executables.
    /// </summary>
    public class JavaUtils : IBuildTool
    {
        private bool _isInitialized;
        private string _javaBinaryPath;
        private string _jarBinaryPath;
        private string _jarSignerBinaryPath;

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            if (_isInitialized)
            {
                return true;
            }

            var jdkPath = GetJdkPath();
            if (jdkPath == null)
            {
                buildToolLogger.DisplayErrorDialog(
                    "Failed to locate the JDK. Check Preferences -> External Tools to set the JDK path.");
                return false;
            }

            _javaBinaryPath = GetBinaryPath(jdkPath, "java");
            if (_javaBinaryPath == null)
            {
                buildToolLogger.DisplayErrorDialog(
                    "Failed to locate Java. Check Preferences -> External Tools to set the JDK path.");
                return false;
            }

            _jarBinaryPath = GetBinaryPath(jdkPath, "jar");
            if (_jarBinaryPath == null)
            {
                buildToolLogger.DisplayErrorDialog(
                    "Failed to locate Java's jar command. Check Preferences -> External Tools to set the JDK path.");
                return false;
            }

            _jarSignerBinaryPath = GetBinaryPath(jdkPath, "jarsigner");
            if (_jarSignerBinaryPath == null)
            {
                buildToolLogger.DisplayErrorDialog(
                    "Failed to locate Java's jarsigner command. Check Preferences -> External Tools to set the JDK path.");
                return false;
            }

            _isInitialized = true;
            return true;
        }

        /// <summary>
        /// Path to the java executable.
        /// </summary>
        public virtual string JavaBinaryPath
        {
            get
            {
                if (_javaBinaryPath == null)
                {
                    throw new BuildToolNotInitializedException(this);
                }

                return _javaBinaryPath;
            }
        }

        /// <summary>
        /// Path to the jar executable.
        /// </summary>
        public virtual string JarBinaryPath
        {
            get
            {
                if (_jarBinaryPath == null)
                {
                    throw new BuildToolNotInitializedException(this);
                }

                return _jarBinaryPath;
            }
        }

        /// <summary>
        /// Path to the jarsigner executable.
        /// </summary>
        public virtual string JarSignerBinaryPath
        {
            get
            {
                if (_jarSignerBinaryPath == null)
                {
                    throw new BuildToolNotInitializedException(this);
                }

                return _jarSignerBinaryPath;
            }
        }

        private static string GetBinaryPath(string jdkPath, string toolName)
        {
            var toolPath = Path.Combine(jdkPath, Path.Combine("bin", toolName + CommandLine.GetExecutableExtension()));
            if (File.Exists(toolPath))
            {
                return toolPath;
            }

            toolPath = CommandLine.FindExecutable(toolName);
            return File.Exists(toolPath) ? toolPath : null;
        }

        private static string GetJdkPath()
        {
#if UNITY_2019_3_OR_NEWER && UNITY_ANDROID
            var toolsJdkPath = UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath;
            if (!string.IsNullOrEmpty(toolsJdkPath) && Directory.Exists(toolsJdkPath))
            {
                Debug.LogFormat("Using tools JDK path: {0}", toolsJdkPath);
                return toolsJdkPath;
            }
#endif
            var embeddedJdkPath = GetEmbeddedJdkPath();
            if (!string.IsNullOrEmpty(embeddedJdkPath) && Directory.Exists(embeddedJdkPath))
            {
                Debug.LogFormat("Using embedded JDK path: {0}", embeddedJdkPath);
                return embeddedJdkPath;
            }

            var preferencesJdkPath = EditorPrefs.GetString("JdkPath");
            if (!string.IsNullOrEmpty(preferencesJdkPath) && Directory.Exists(preferencesJdkPath))
            {
                Debug.LogFormat("Using preferences JDK path: {0}", preferencesJdkPath);
                return preferencesJdkPath;
            }

            var environmentJdkPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentJdkPath) && Directory.Exists(environmentJdkPath))
            {
                Debug.LogFormat("Using environment JDK path: {0}", environmentJdkPath);
                return environmentJdkPath;
            }

            return null;
        }

        /// <summary>
        /// Returns the path to Unity's embedded version of OpenJDK, or null if not enabled.
        /// Unity 2018.3 added the option to use a version of OpenJDK that is installed with Unity.
        /// </summary>
        private static string GetEmbeddedJdkPath()
        {
            // Users switching between Unity versions may have JdkUseEmbedded present in their preferences even though
            // their current version of Unity does not support it. So we return null for other versions to avoid that
            // case.
#if !UNITY_2018_3_OR_NEWER || UNITY_2019_3_OR_NEWER
            return null;
#else
            // For Unity 2018.3 through 2019.2 construct the embedded JDK path as follows.
            if (EditorPrefs.GetInt("JdkUseEmbedded") != 1)
            {
                return null;
            }

            string pathRelativeToEditor;
            switch (Application.platform)
            {
                case RuntimePlatform.LinuxEditor:
                    pathRelativeToEditor = "Data/PlaybackEngines/AndroidPlayer/Tools/OpenJdk/Linux/";
                    break;
                case RuntimePlatform.OSXEditor:
                    pathRelativeToEditor = "PlaybackEngines/AndroidPlayer/Tools/OpenJdk/MacOS/";
                    break;
                case RuntimePlatform.WindowsEditor:
                    pathRelativeToEditor = "Data/PlaybackEngines/AndroidPlayer/Tools/OpenJdk/Windows/";
                    break;
                default:
                    throw new Exception(
                        "Cannot get embedded JDK path for unsupported platform: " + Application.platform);
            }

            var unityEditor = new FileInfo(EditorApplication.applicationPath);
            if (unityEditor.DirectoryName == null)
            {
                throw new Exception(
                    "Cannot get embedded JDK path. EditorApplication.applicationPath returned an unexpected value: " +
                    EditorApplication.applicationPath);
            }

            return Path.Combine(unityEditor.DirectoryName, pathRelativeToEditor);
#endif
        }
    }
}
