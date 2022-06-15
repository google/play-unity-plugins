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
using UnityEngine;

namespace Google.Play.InputMapping.Internal
{
    /// <summary>
    /// Provides methods for converting C# PlayInputMaps into AndroidJavaObjects.
    /// </summary>
    internal class InputMapToJavaConverter : IDisposable
    {
        private const string InputMapClassName = JavaConstants.DataModelPrefix + "InputMap";
        private const string InputGroupClassName = JavaConstants.DataModelPrefix + "InputGroup";
        private const string InputControlsClassName = JavaConstants.DataModelPrefix + "InputControls";
        private const string InputActionClassName = JavaConstants.DataModelPrefix + "InputAction";
        private const string MouseSettingsClassName = JavaConstants.DataModelPrefix + "MouseSettings";
        private const string ArraysClassName = "java.util.Arrays";
        private const string IntegerClassName = "java.lang.Integer";

        private readonly AndroidJavaClass _arraysClass;
        private readonly AndroidJavaClass _inputMapClass;
        private readonly AndroidJavaClass _inputGroupClass;
        private readonly AndroidJavaClass _inputControlsClass;
        private readonly AndroidJavaClass _inputActionClass;
        private readonly AndroidJavaClass _mouseSettingsClass;

        /// <summary>
        /// Constructor.
        /// </summary>
        public InputMapToJavaConverter()
        {
            _inputMapClass = new AndroidJavaClass(InputMapClassName);
            _inputGroupClass = new AndroidJavaClass(InputGroupClassName);
            _inputControlsClass = new AndroidJavaClass(InputControlsClassName);
            _inputActionClass = new AndroidJavaClass(InputActionClassName);
            _mouseSettingsClass = new AndroidJavaClass(MouseSettingsClassName);
            _arraysClass = new AndroidJavaClass(ArraysClassName);
        }

        /// <summary>
        /// Converts an C# PlayInputMap to an AndroidJavaObject backed by a Java PlayInputMap.
        /// </summary>
        public AndroidJavaObject ConvertToJavaObject(PlayInputMap inputMap)
        {
            if (inputMap == null)
            {
                throw new NullReferenceException("inputMap is null.");
            }

            using (var javaInputGroups = ConvertToJavaList(inputMap.InputGroups, Convert))
            using (var javaMouseSettings = Convert(inputMap.MouseSettings))
            {
                return _inputMapClass.CallStatic<AndroidJavaObject>("create", javaInputGroups, javaMouseSettings);
            }
        }

        private AndroidJavaObject Convert(PlayMouseSettings mouseSettings)
        {
            if (mouseSettings == null)
            {
                throw new NullReferenceException("mouseSettings is null.");
            }

            return _mouseSettingsClass.CallStatic<AndroidJavaObject>("create",
                mouseSettings.AllowMouseSensitivityAdjustment, mouseSettings.InvertMouseMovement);
        }

        private AndroidJavaObject Convert(PlayInputGroup inputGroup)
        {
            if (inputGroup == null)
            {
                throw new NullReferenceException("inputGroup is null.");
            }

            using (var javaInputActions = ConvertToJavaList(inputGroup.InputActions, Convert))
            {
                return _inputGroupClass.CallStatic<AndroidJavaObject>("create", inputGroup.GroupLabel,
                    javaInputActions);
            }
        }

        private AndroidJavaObject Convert(PlayInputAction inputAction)
        {
            if (inputAction == null)
            {
                throw new NullReferenceException("inputAction is null.");
            }

            using (var javaInputControls = Convert(inputAction.InputControls))
            {
                return _inputActionClass.CallStatic<AndroidJavaObject>("create", inputAction.ActionLabel,
                    inputAction.UniqueId, javaInputControls);
            }
        }

        private AndroidJavaObject Convert(PlayInputControls inputControls)
        {
            if (inputControls == null)
            {
                throw new NullReferenceException("inputControls is null.");
            }

            using (var javaKeycodes = ConvertToJavaList(inputControls.AndroidKeycodes, ConvertToJavaInteger))
            using (var javaMouseActions = ConvertToJavaList(inputControls.MouseActions, ConvertToJavaInteger))
            {
                return _inputControlsClass.CallStatic<AndroidJavaObject>("create", javaKeycodes, javaMouseActions);
            }
        }

        private AndroidJavaObject ConvertToJavaInteger(PlayMouseAction mouseAction)
        {
            return ConvertToJavaInteger((int) mouseAction);
        }

        private AndroidJavaObject ConvertToJavaInteger(int integer)
        {
            return new AndroidJavaObject(IntegerClassName, integer);
        }

        private AndroidJavaObject ConvertToJavaList<T>(IList<T> list,
            Func<T, AndroidJavaObject> convertToJavaObject)
        {
            if (list == null)
            {
                list = new List<T>();
            }

            AndroidJavaObject[] javaObjects = list.Select(convertToJavaObject).ToArray();

            // CallStatic takes a params array as it's second argument, so we need to wrap javaObjects in an object[].
            // Otherwise CallStatic will treat its elements as individual arguments instead of passing javaObjects
            // as a single argument.
            object[] paramsArray = {javaObjects};
            return _arraysClass.CallStatic<AndroidJavaObject>("asList", paramsArray);
        }

        /// <summary>
        /// Disposes the AndroidJavaClasses used during conversion.
        /// </summary>
        public void Dispose()
        {
            _inputMapClass.Dispose();
            _inputGroupClass.Dispose();
            _inputControlsClass.Dispose();
            _inputActionClass.Dispose();
            _mouseSettingsClass.Dispose();
        }
    }
}