// <copyright file="PlayServicesResolver.cs" company="Google Inc.">
// Copyright (C) 2015 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>

namespace Google.Android.AppBundle.Editor.Internal.PlayServices
{
    /// <summary>
    /// Stub Play Services resolver for compatibility with the Play Instant plugin.
    /// </summary>
    public static class PlayServicesResolver
    {
        /// <summary>
        /// Logger for this module.
        /// </summary>
        private static readonly Logger Logger = new Logger();

        /// <summary>
        /// Log a filtered message to Unity log, error messages are stored in
        /// PlayServicesSupport.lastError.
        /// </summary>
        /// <param name="message">String to write to the log.</param>
        /// <param name="level">Severity of the message, if this is below the currently selected
        /// Level property the message will not be logged.</param>
        internal static void Log(string message, LogLevel level = LogLevel.Info) {
            Logger.Log(message, level);
        }
    }
}
