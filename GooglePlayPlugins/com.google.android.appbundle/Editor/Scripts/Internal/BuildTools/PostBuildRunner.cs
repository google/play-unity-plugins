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

#if !UNITY_2018_3_OR_NEWER
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Google.Android.AppBundle.Editor.Internal.BuildTools
{
    /// <summary>
    /// Runs a task after a player build causes the application domain to reset and scripts to be reloaded.
    /// Starting with Unity 2018.3 player builds no longer reset the application domain.
    /// </summary>
    public static class PostBuildRunner
    {
        /// <summary>
        /// Class that provides an abstraction layer above <see cref="PostBuildTask"/> in order to capture
        /// the task's full type name, which is needed for task deserialization.
        /// </summary>
        [Serializable]
        private class TaskConfig
        {
            /// <summary>
            /// Location of the <see cref="PostBuildTask"/> serialized file.
            /// </summary>
            public string taskFilePath;

            /// <summary>
            /// The full type name of the class that was serialized at <see cref="taskFilePath"/>.
            /// </summary>
            public string taskTypeName;
        }

        private static readonly string PostBuildTaskConfigFilePath =
            Path.Combine("Library", "PlayPostBuildTaskConfig.json");

        private static float _progress;

        /// <summary>
        /// Serializes the specified task to disk for running after scripts are reloaded.
        /// </summary>
        public static void RunTask(PostBuildTask task)
        {
            if (File.Exists(PostBuildTaskConfigFilePath))
            {
                Debug.LogWarningFormat(
                    "Creating a new post-build task when one already exists: {0}", PostBuildTaskConfigFilePath);
            }

            var taskFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var taskConfig = new TaskConfig {taskFilePath = taskFilePath, taskTypeName = task.GetType().FullName};

            SerializeToJson(taskFilePath, task);
            SerializeToJson(PostBuildTaskConfigFilePath, taskConfig);

            _progress = 0.0f;
            // We don't unsubscribe from updates (except when canceling) because script reload will reset everything.
            EditorApplication.update += HandleUpdate;
        }

        [DidReloadScripts]
        private static void DidReloadScripts()
        {
            if (!File.Exists(PostBuildTaskConfigFilePath))
            {
                // In the common case the scripts reloaded because of a script change, etc. Nothing to do.
                return;
            }

            PostBuildTask postBuildTask;
            try
            {
                // Hide the progress bar that was shown in HandleUpdate().
                EditorUtility.ClearProgressBar();

                Debug.LogFormat("Loading post-build task config file: {0}", PostBuildTaskConfigFilePath);
                var taskConfigJsonText = File.ReadAllText(PostBuildTaskConfigFilePath);
                var taskConfig = JsonUtility.FromJson<TaskConfig>(taskConfigJsonText);
                var taskJsonText = File.ReadAllText(taskConfig.taskFilePath);
                var taskTypeName = taskConfig.taskTypeName;

                Debug.LogFormat("Loading and running post-build task for type: {0}", taskTypeName);
                var postBuildTaskObj = JsonUtility.FromJson(taskJsonText, Type.GetType(taskTypeName));
                postBuildTask = (PostBuildTask) postBuildTaskObj;
            }
            finally
            {
                // Remove the config file so that this task doesn't run again after the next application domain reset.
                File.Delete(PostBuildTaskConfigFilePath);
            }

            postBuildTask.RunPostBuildTask();
        }

        // Handler for EditorApplication.update callbacks.
        private static void HandleUpdate()
        {
            // Magic number to partially fill up the progress bar during the time that it takes scripts to reload.
            // Note that the best number varies by version, but better to finish with the progress bar less than
            // full on some versions than full long before scripts reload on other versions.
            _progress += 0.0005f;

            if (!EditorUtility.DisplayCancelableProgressBar("Waiting for scripts to reload...", null, _progress))
            {
                return;
            }

            Debug.Log("Canceling post-build task before scripts reload...");
            // Since the existence of the config file is the signal to run a post-build task, delete the file to cancel.
            File.Delete(PostBuildTaskConfigFilePath);
            EditorUtility.ClearProgressBar();
            EditorApplication.update -= HandleUpdate;
        }

        private static void SerializeToJson(string path, object obj)
        {
            var jsonText = JsonUtility.ToJson(obj);
            File.WriteAllText(path, jsonText);
        }
    }
}
#endif