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
using System.Linq;
using UnityEditor;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Build tool for determining whether this is a release build and displaying any relevant errors if so.
    /// </summary>
    public class ReleaseBuildHelper : IBuildTool
    {
        private const string Arm64RequiredDescription =
            "ARM64 libraries must be included in any APK or Android App Bundle published on Play.";

        private const string Il2CppRequiredDescription =
            "\n\nNote: the IL2CPP Scripting Backend is required to build ARM64 libraries.";

        private readonly JarSigner _jarSigner;

        public ReleaseBuildHelper(JarSigner jarSigner)
        {
            _jarSigner = jarSigner;
        }

        // TODO: add check for PlayerSettings.productName
        public virtual bool Initialize(BuildToolLogger buildToolLogger)
        {
            // Try to determine whether this is intended to be a release build.
            if (!_jarSigner.UseCustomKeystore || EditorUserBuildSettings.development)
            {
                // Seems like a debug build, so no need to do release build checks.
                return true;
            }

            if (!HasIconForTargetGroup(BuildTargetGroup.Unknown) && !HasIconForTargetGroup(BuildTargetGroup.Android))
            {
                buildToolLogger.DisplayErrorDialog(
                    "Failed to locate a Default Icon or an Android Icon for this project. " +
                    "Check Player Settings to set an icon");
                return false;
            }

            string message;
            switch (AndroidArchitectureHelper.ArchitectureStatus)
            {
                case AndroidArchitectureHelper.Status.Ok:
                    return true;
                case AndroidArchitectureHelper.Status.ArmV7Disabled:
                    message = "ARMv7 and " + Arm64RequiredDescription + Il2CppRequiredDescription;
                    break;
                case AndroidArchitectureHelper.Status.Il2CppDisabled:
                    message = Arm64RequiredDescription + Il2CppRequiredDescription;
                    break;
                case AndroidArchitectureHelper.Status.Arm64Disabled:
                    message = Arm64RequiredDescription;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            message += "\n\nClick \"OK\" to enable an IL2CPP build for ARMv7 and ARM64 architectures.";
            if (buildToolLogger.DisplayActionableErrorDialog(message))
            {
                AndroidArchitectureHelper.FixTargetArchitectures();
            }

            return false;
        }

        private static bool HasIconForTargetGroup(BuildTargetGroup buildTargetGroup)
        {
            var icons = PlayerSettings.GetIconsForTargetGroup(buildTargetGroup);
            return icons != null && icons.Any(icon => icon != null);
        }
    }
}