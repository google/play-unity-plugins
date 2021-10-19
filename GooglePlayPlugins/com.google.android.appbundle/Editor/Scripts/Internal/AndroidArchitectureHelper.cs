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

using UnityEditor;

namespace Google.Android.AppBundle.Editor.Internal
{
    /// <summary>
    /// Provides utilities for checking whether this version of Unity supports building ARM64 libraries.
    /// </summary>
    public static class AndroidArchitectureHelper
    {
        /// <summary>
        /// Indicates the current status of architecture-related build settings.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// No issues with selected architectures.
            /// </summary>
            Ok,

            /// <summary>
            /// ARMv7 is disabled.
            /// </summary>
            ArmV7Disabled,

            /// <summary>
            /// IL2CPP is disabled, also implying that ARM64 is unavailable.
            /// </summary>
            Il2CppDisabled,

            /// <summary>
            /// IL2CPP is enabled, but ARM64 is disabled.
            /// </summary>
            Arm64Disabled
        }

        /// <summary>
        /// Returns true if this version of the Unity Editor has native support for building an Android App Bundle,
        /// and false otherwise.
        /// </summary>
        public static Status ArchitectureStatus
        {
            get
            {
                if (!IsArchitectureEnabled(AndroidArchitecture.ARMv7))
                {
                    return Status.ArmV7Disabled;
                }

                if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) != ScriptingImplementation.IL2CPP)
                {
                    return Status.Il2CppDisabled;
                }

                return IsArchitectureEnabled(AndroidArchitecture.ARM64)
                    ? Status.Ok
                    : Status.Arm64Disabled;
            }
        }

        /// <summary>
        /// Enables an IL2CPP build of ARMv7 and ARM64 without affecting whether X86 is enabled.
        /// IL2CPP is required since Unity's Mono scripting backend doesn't support ARM64.
        /// </summary>
        public static void FixTargetArchitectures()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures |= AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        }

        /// <summary>
        /// Enable the IL2CPP scripting backend and all Android architectures supported by IL2CPP builds.
        /// </summary>
        public static void EnableIl2CppBuildArchitectures()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
        }

        /// <summary>
        /// Enable the Mono scripting backend and all Android architectures supported by Mono builds.
        /// Note: Unity 2019.4.31+ only supports x86 for IL2CPP builds.
        /// </summary>
        public static void EnableMonoBuildArchitectures()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
#if UNITY_2019_3_OR_NEWER
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
#else
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.X86;
#endif
        }

        private static bool IsArchitectureEnabled(AndroidArchitecture androidArchitecture)
        {
            // Note: Enum.HasFlag() was introduced in .NET 4.x
            return (PlayerSettings.Android.targetArchitectures & androidArchitecture) == androidArchitecture;
        }
    }
}
