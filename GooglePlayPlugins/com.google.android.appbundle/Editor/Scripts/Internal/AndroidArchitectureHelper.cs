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

// In the Unity 2017 series ARM64 support was introduced in 2017.4.16.
// It might seem preferable to modify targetArchitectures using reflection, but the field is extern.
// NOTE: this supports up to UNITY_2017_4_50 and will have to be extended if more versions are released.

#if UNITY_2018_1_OR_NEWER || UNITY_2017_4_16 || UNITY_2017_4_17 || UNITY_2017_4_18 || UNITY_2017_4_19 || UNITY_2017_4_20 || UNITY_2017_4_21 || UNITY_2017_4_22 || UNITY_2017_4_23 || UNITY_2017_4_24 || UNITY_2017_4_25 || UNITY_2017_4_26 || UNITY_2017_4_27 || UNITY_2017_4_28 || UNITY_2017_4_29 || UNITY_2017_4_30 || UNITY_2017_4_31 || UNITY_2017_4_32 || UNITY_2017_4_33 || UNITY_2017_4_34 || UNITY_2017_4_35 || UNITY_2017_4_36 || UNITY_2017_4_37 || UNITY_2017_4_38 || UNITY_2017_4_39 || UNITY_2017_4_40 || UNITY_2017_4_41 || UNITY_2017_4_42 || UNITY_2017_4_43 || UNITY_2017_4_44 || UNITY_2017_4_45 || UNITY_2017_4_46 || UNITY_2017_4_47 || UNITY_2017_4_48 || UNITY_2017_4_49 || UNITY_2017_4_50
#define GOOGLE_ANDROID_HAS_64_BIT_SUPPORT
#else
using System;
#endif

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
        /// Returns true if this version of Unity can build for the Play Store: either it can provide 64 bit libraries
        /// or it falls under the Unity 5.6 or older exemption described
        /// <a href="https://android-developers.googleblog.com/2019/01/get-your-apps-ready-for-64-bit.html">here</a>.
        /// </summary>
        public static bool UnityVersionSupported
        {
            get
            {
#if GOOGLE_ANDROID_HAS_64_BIT_SUPPORT || !UNITY_2017_1_OR_NEWER
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Returns true if this version of the Unity Editor has native support for building an Android App Bundle,
        /// and false otherwise.
        /// </summary>
        public static Status ArchitectureStatus
        {
            get
            {
#if GOOGLE_ANDROID_HAS_64_BIT_SUPPORT
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
#else
                // Don't bother checking that AndroidTargetDevice is FAT or ARMv7.
                return Status.Ok;
#endif
            }
        }

        /// <summary>
        /// Enables an IL2CPP build of ARMv7 and ARM64 without affecting whether X86 is enabled.
        /// IL2CPP is required since Unity's Mono scripting backend doesn't support ARM64.
        /// </summary>
        public static void FixTargetArchitectures()
        {
#if GOOGLE_ANDROID_HAS_64_BIT_SUPPORT
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures |= AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
#else
            throw new Exception("Not supported on this version of Unity.");
#endif
        }

        /// <summary>
        /// Enable the IL2CPP scripting backend and all Android architectures supported by IL2CPP builds.
        /// </summary>
        public static void EnableIl2CppBuildArchitectures()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
#if GOOGLE_ANDROID_HAS_64_BIT_SUPPORT
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
#else
            PlayerSettings.Android.targetDevice = AndroidTargetDevice.FAT;
#endif
        }

        /// <summary>
        /// Enable the Mono scripting backend and all Android architectures supported by Mono builds.
        /// </summary>
        public static void EnableMonoBuildArchitectures()
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
#if UNITY_2019_3_OR_NEWER
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
#elif GOOGLE_ANDROID_HAS_64_BIT_SUPPORT
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.X86;
#else
            PlayerSettings.Android.targetDevice = AndroidTargetDevice.FAT;
#endif
        }

#if GOOGLE_ANDROID_HAS_64_BIT_SUPPORT
        private static bool IsArchitectureEnabled(AndroidArchitecture androidArchitecture)
        {
            // Note: Enum.HasFlag() was introduced in .NET 4.x
            return (PlayerSettings.Android.targetArchitectures & androidArchitecture) == androidArchitecture;
        }
#endif
    }
}