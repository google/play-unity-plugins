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

using System.Linq;
using Google.Android.AppBundle.Editor.Internal;
using Google.Android.AppBundle.Editor.Internal.BuildTools;
using Google.Android.AppBundle.Editor.Internal.Utils;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal
{
    /// <summary>
    /// Build tool for determining whether there are any issues blocking the creation of an instant app.
    /// Building a persistent app may be allowed, depending on how this class is configured.
    /// </summary>
    public class PlayInstantBuildHelper : IBuildTool
    {
        private readonly bool _isInstantRequired;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PlayInstantBuildHelper(bool isInstantRequired)
        {
            _isInstantRequired = isInstantRequired;
        }

        public bool IsInstantBuild { get; private set; }

        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            IsInstantBuild = PlayInstantBuildSettings.IsInstantBuildType();
            if (!IsInstantBuild)
            {
                if (!_isInstantRequired)
                {
                    // Check passed: this is a persistent app build invoked by a builder that supports persistent apps.
                    return true;
                }

                // This builder only works for instant apps, however the build isn't currently set to instant.
                Debug.LogError("Build halted since selected build type is \"Installed\"");
                var instantRequiredMessage = string.Format(
                    "The currently selected Android build type is \"Installed\".\n\n" +
                    "Click \"{0}\" to open the \"{1}\" window where the build type can be changed to \"Instant\".",
                    WindowUtils.OkButtonText, BuildSettingsWindow.WindowTitle);
                if (buildToolLogger.DisplayActionableErrorDialog(instantRequiredMessage))
                {
                    BuildSettingsWindow.ShowWindow();
                }

                return false;
            }

            // This is an instant app build. Verify that instant-specific required policies are enabled.
            var failedPolicies = PlayInstantSettingPolicy.GetRequiredPolicies()
                .Where(policy => !policy.IsCorrectState())
                .Select(policy => policy.Name).ToArray();
            if (failedPolicies.Length == 0)
            {
                return true;
            }

            Debug.LogErrorFormat(
                "Build halted due to incompatible settings: {0}", string.Join(", ", failedPolicies));
            var failedPoliciesMessage = string.Format(
                "{0}\n\nClick \"{1}\" to open the settings window and make required changes.",
                string.Join("\n\n", failedPolicies),
                WindowUtils.OkButtonText);
            if (buildToolLogger.DisplayActionableErrorDialog(failedPoliciesMessage))
            {
                PlayerSettingsWindow.ShowWindow();
            }

            return false;
        }
    }
}