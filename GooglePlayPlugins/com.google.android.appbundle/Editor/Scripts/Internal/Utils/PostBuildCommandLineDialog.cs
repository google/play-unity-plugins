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

using Google.Android.AppBundle.Editor.Internal.PlayServices;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.Utils
{
    /// <summary>
    /// A CommandLineDialog that potentially waits to run the specified command until after an AppDomain reset.
    ///
    /// Unity versions prior to 2018.3 reload all Editor scripts a few seconds after a call to BuildPlayer finishes.
    /// This reload also resets the AppDomain and aborts any actively running threads, including any threads managing a
    /// child process. This EditorWindow therefore waits to run the specified command until after the AppDomain is
    /// reset.
    /// </summary>
    public class PostBuildCommandLineDialog : CommandLineDialog
    {
        [SerializeField] public CommandLineParameters CommandLineParams;

        // Use two bool fields to detect when this script is reloaded and the AppDomain is reset.
        // See https://docs.unity3d.com/Manual/script-Serialization.html
        private static bool _nonserializedField;
        [SerializeField] private bool _serializedField;

        private void Awake()
        {
            CommandLineParams = new CommandLineParameters();
            _nonserializedField = false;
            _serializedField = false;
        }

        protected override void Update()
        {
#if UNITY_2018_3_OR_NEWER
            // Starting with Unity 2018.3 the AppDomain is not reset after building an APK.
            // In this case we run the command when the window is first shown.
            // Note: some beta versions of 2018.3 reset the AppDomain as in earlier versions of Unity.
            if (!_nonserializedField)
            {
                _nonserializedField = true;
                RunCommandAsync();
            }
#else
            PollForAppDomainReset();
#endif
            base.Update();
        }

        private void PollForAppDomainReset()
        {
            // This block is entered twice: when the window is initially shown and later after the AppDomain is reset.
            if (!_nonserializedField)
            {
                _nonserializedField = true;
                if (_serializedField)
                {
                    Debug.Log("The AppDomain has been reset.");
                    RunCommandAsync();
                }
                else
                {
                    bodyText += "Waiting for scripts to reload...\n\n";
                    Debug.Log("Waiting for the AppDomain reset...");
                    _serializedField = true;
                }
            }
        }

        private void RunCommandAsync()
        {
            RunAsync(
                CommandLineParams.FileName,
                CommandLineParams.Arguments,
                commandLineResult =>
                {
                    if (commandLineResult.exitCode == 0)
                    {
                        Close();
                    }
                    else
                    {
                        // After adding the button we need to scroll down a little more.
                        scrollPosition.y = Mathf.Infinity;
                        noText = "Close";
                        Repaint();
                    }
                },
                envVars: CommandLineParams.EnvironmentVariables);
        }

        /// <summary>
        /// Creates a dialog box which can display command line output.
        /// </summary>
        public static PostBuildCommandLineDialog CreateDialog(string title)
        {
            var window = (PostBuildCommandLineDialog) GetWindow(typeof(PostBuildCommandLineDialog), true, title);
            window.Initialize();
            return window;
        }
    }
}