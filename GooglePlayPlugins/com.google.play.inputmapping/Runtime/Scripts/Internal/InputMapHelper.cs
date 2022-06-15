// Copyright 2021 Google LLC
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
// limitations under the License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("Google.Play.InputMapping.Tests")]

namespace Google.Play.InputMapping.Internal
{
    /// <summary>
    /// Contains helper methods for manipulating InputMaps.
    /// </summary>
    internal static class InputMapHelper
    {
        /// <summary>
        /// Converts an C# PlayInputMap to an AndroidJavaObject backed by a Java InputMap.
        /// </summary>
        public static AndroidJavaObject ConvertToJavaObject(PlayInputMap inputMap)
        {
            using (var converter = new InputMapToJavaConverter())
            {
                return converter.ConvertToJavaObject(inputMap);
            }
        }

        /// <summary>
        /// Parses the fields of an AndroidJavaObject representing an InputMap, and creates a C# PlayInputMap with matching fields.
        /// </summary>
        public static PlayInputMap ConvertToInputMap(AndroidJavaObject javaInputMap)
        {
            using (var javaInputGroupsList = javaInputMap.Call<AndroidJavaObject>("inputGroups"))
            using (var javaMouseSettings = javaInputMap.Call<AndroidJavaObject>("mouseSettings"))
            {
                var inputGroups = ConvertToList(javaInputGroupsList, ConvertToInputGroup);
                var mouseSettings = ConvertToMouseSettings(javaMouseSettings);
                return PlayInputMap.Create(inputGroups, mouseSettings);
            }
        }

        private static PlayMouseSettings ConvertToMouseSettings(AndroidJavaObject javaMouseSettings)
        {
            var allowMouseSensitivityAdjustment = javaMouseSettings.Call<bool>("allowMouseSensitivityAdjustment");
            var invertMouseMovement = javaMouseSettings.Call<bool>("invertMouseMovement");
            return PlayMouseSettings.Create(allowMouseSensitivityAdjustment, invertMouseMovement);
        }

        private static PlayInputGroup ConvertToInputGroup(AndroidJavaObject javaInputGroup)
        {
            using (var javaInputActionsList = javaInputGroup.Call<AndroidJavaObject>("inputActions"))
            {
                var groupLabel = javaInputGroup.Call<string>("groupLabel");
                var inputActions = ConvertToList(javaInputActionsList, ConvertToInputAction);
                return PlayInputGroup.Create(groupLabel, inputActions);
            }
        }

        private static PlayInputAction ConvertToInputAction(AndroidJavaObject javaInputAction)
        {
            using (var javaInputControls = javaInputAction.Call<AndroidJavaObject>("inputControls"))
            {
                var actionLabel = javaInputAction.Call<string>("actionLabel");
                var uniqueId = javaInputAction.Call<int>("uniqueId");
                var inputControls = ConvertToInputControls(javaInputControls);
                return PlayInputAction.Create(actionLabel, uniqueId, inputControls);
            }
        }

        private static PlayInputControls ConvertToInputControls(AndroidJavaObject javaInputControls)
        {
            using (var javaKeycodes = javaInputControls.Call<AndroidJavaObject>("keycodes"))
            using (var javaMouseActions = javaInputControls.Call<AndroidJavaObject>("mouseActions"))
            {
                var androidKeycodes = ConvertToList(javaKeycodes, ConvertToInt);
                var mouseActions = ConvertToList(javaMouseActions, ConvertToMouseAction);
                return PlayInputControls.Create(androidKeycodes, mouseActions);
            }
        }

        private static PlayMouseAction ConvertToMouseAction(AndroidJavaObject javaMouseActionInteger)
        {
            var mouseActionInt = ConvertToInt(javaMouseActionInteger);
            try
            {
                return (PlayMouseAction)mouseActionInt;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("The integer " + mouseActionInt +
                                            " cannot be cast to a valid PlayMouseAction");
            }
        }

        private static int ConvertToInt(AndroidJavaObject javaInteger)
        {
            return javaInteger.Call<int>("intValue");
        }

        private static List<T> ConvertToList<T>(AndroidJavaObject javaList, Func<AndroidJavaObject, T> convertElement)
        {
            return javaList.Call<AndroidJavaObject[]>("toArray").Select(convertElement).ToList();
        }
    }
}