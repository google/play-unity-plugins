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

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering;

namespace Google.Play.Instant.Editor.Internal
{
    /// <summary>
    /// Enumerates Unity setting policies that affect whether an instant app can successfully be built. Some settings
    /// are required and must be configured in a certain way, where others are simply recommended. Each policy can
    /// report whether it is configured correctly and if not, provides a delegate to change to the preferred state.
    /// </summary>
    public class PlayInstantSettingPolicy
    {
        private const string MultithreadedRenderingDescription =
            "Pre-Oreo devices do not support instant apps using EGL shared contexts.";

        private const string ApkSizeReductionDescription = "This setting reduces APK size.";

        public delegate bool IsCorrectStateDelegate();

        public delegate bool ChangeStateDelegate();

        private PlayInstantSettingPolicy(
            string name,
            string description,
            IsCorrectStateDelegate isCorrectState,
            ChangeStateDelegate changeState)
        {
            Name = name;
            Description = description;
            IsCorrectState = isCorrectState;
            ChangeState = changeState;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public IsCorrectStateDelegate IsCorrectState { get; private set; }

        public ChangeStateDelegate ChangeState { get; private set; }

        /// <summary>
        /// Returns the policies that are required to build an instant app.
        /// </summary>
        public static IEnumerable<PlayInstantSettingPolicy> GetRequiredPolicies()
        {
            return new List<PlayInstantSettingPolicy>
            {
                new PlayInstantSettingPolicy(
                    "Build Target should be Android",
                    null,
                    () => EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android,
                    () => EditorUserBuildSettings.SwitchActiveBuildTarget(
                        BuildTargetGroup.Android, BuildTarget.Android)),

                // We require Gradle for 2018.1+, but hide this policy for 2019.1+ since Gradle is the only option.
#if UNITY_2018_1_OR_NEWER && !UNITY_2019_1_OR_NEWER
                new PlayInstantSettingPolicy(
                    "Android build system should be Gradle",
                    "Required for IPostGenerateGradleAndroidProject and APK Signature Scheme v2.",
                    () => EditorUserBuildSettings.androidBuildSystem == AndroidBuildSystem.Gradle,
                    () =>
                    {
                        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
                        return true;
                    }),
#endif

                new PlayInstantSettingPolicy(
                    "Graphics API should be OpenGLES2 only",
                    "Pre-Oreo devices only support instant apps using GLES2. " +
                    "Removing unused Graphics APIs reduces APK size.",
                    () =>
                    {
                        var graphicsDeviceTypes = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
                        return !PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android) &&
                               graphicsDeviceTypes.Length == 1 &&
                               graphicsDeviceTypes[0] == GraphicsDeviceType.OpenGLES2;
                    },
                    () =>
                    {
                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] {GraphicsDeviceType.OpenGLES2});
                        return true;
                    }),

                new PlayInstantSettingPolicy(
                    "Split Application Binary should be disabled",
                    "Instant apps don't support APK Expansion (OBB) Files.",
                    () => !PlayerSettings.Android.useAPKExpansionFiles,
                    () =>
                    {
                        PlayerSettings.Android.useAPKExpansionFiles = false;
                        return true;
                    }),

#if UNITY_2017_2_OR_NEWER
                new PlayInstantSettingPolicy(
                    "Android Multithreaded Rendering should be disabled",
                    MultithreadedRenderingDescription,
                    () => !PlayerSettings.GetMobileMTRendering(BuildTargetGroup.Android),
                    () =>
                    {
                        PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, false);
                        return true;
                    })
#else
                new PlayInstantSettingPolicy(
                    "Mobile Multithreaded Rendering should be disabled",
                    MultithreadedRenderingDescription,
                    () => !PlayerSettings.mobileMTRendering,
                    () =>
                    {
                        PlayerSettings.mobileMTRendering = false;
                        return true;
                    })
#endif
            };
        }

        /// <summary>
        /// Returns the policies that are recommended when building an instant app, e.g. to reduce APK size.
        /// </summary>
        public static IEnumerable<PlayInstantSettingPolicy> GetRecommendedPolicies()
        {
            var policies = new List<PlayInstantSettingPolicy>
            {
                new PlayInstantSettingPolicy(
                    "Android minSdkVersion should be 21",
                    "Lower than 21 is fine, though 21 is the minimum supported by Google Play Instant.",
                    // TODO: consider prompting if strictly greater than 21 to say that 21 enables wider reach
                    () => (int) PlayerSettings.Android.minSdkVersion >= 21,
                    () =>
                    {
                        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
                        return true;
                    })
            };

            // For the purpose of switching to .NET 2.0 Subset, it's sufficient to check #if NET_2_0. However, it would
            // be confusing if the option disappeared after clicking the Update button, so we also check NET_2_0_SUBSET.
            // For background on size reduction benefits see https://docs.unity3d.com/Manual/dotnetProfileSupport.html
#if NET_2_0 || NET_2_0_SUBSET
            policies.Add(new PlayInstantSettingPolicy(
                "API Compatibility Level should be \".NET 2.0 Subset\"",
                ApkSizeReductionDescription,
                () => PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android) ==
                      ApiCompatibilityLevel.NET_2_0_Subset,
                () =>
                {
                    PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android,
                        ApiCompatibilityLevel.NET_2_0_Subset);
                    return true;
                }));
#endif

            // Unity 2017.1 added experimental support for .NET 4.6, however .NET Standard 2.0 is unavailable.
            // Therefore, we only provide this policy option starting with Unity 2018.1.
            // Our #if check includes NET_STANDARD_2_0 for the reason we check NET_2_0_SUBSET above.
            // https://blogs.unity3d.com/2018/03/28/updated-scripting-runtime-in-unity-2018-1-what-does-the-future-hold/
#if UNITY_2018_1_OR_NEWER && (NET_4_6 || NET_STANDARD_2_0)
            policies.Add(new PlayInstantSettingPolicy(
                "API Compatibility Level should be \".NET Standard 2.0\"",
                ApkSizeReductionDescription,
                () => PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android) ==
                      ApiCompatibilityLevel.NET_Standard_2_0,
                () =>
                {
                    PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android,
                        ApiCompatibilityLevel.NET_Standard_2_0);
                    return true;
                }));
#endif

            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android) == ScriptingImplementation.IL2CPP)
            {
                // This will reduce APK size, but may cause runtime issues if needed components are removed.
                // See https://docs.unity3d.com/Manual/IL2CPP-BytecodeStripping.html
                policies.Add(new PlayInstantSettingPolicy(
                    "IL2CPP builds should strip engine code",
                    "This setting reduces APK size, but may cause runtime issues.",
                    () => PlayerSettings.stripEngineCode,
                    () =>
                    {
                        PlayerSettings.stripEngineCode = true;
                        return true;
                    }));
            }

#if UNITY_2018_3_OR_NEWER
            policies.Add(new PlayInstantSettingPolicy(
                "Release builds should use managed code stripping",
                ApkSizeReductionDescription,
                // Note: in some alpha/beta builds of 2018.3 ManagedStrippingLevel.High isn't defined (use "Normal").
                () => PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android) == ManagedStrippingLevel.High,
                () =>
                {
                    PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.High);
                    return true;
                }));
#endif

            return policies;
        }
    }
}