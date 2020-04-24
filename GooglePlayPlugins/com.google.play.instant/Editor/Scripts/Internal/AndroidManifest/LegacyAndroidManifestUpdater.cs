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

using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace Google.Play.Instant.Editor.Internal.AndroidManifest
{
    /// <summary>
    /// An IAndroidManifestUpdater for Unity versions 2017 and earlier that saves manifest changes to
    /// the file Assets/Plugins/Android/AndroidManifest.xml
    /// </summary>
    public class LegacyAndroidManifestUpdater : IAndroidManifestUpdater
    {
        private const string AndroidManifestAssetsDirectory = "Assets/Plugins/Android/";
        private const string AndroidManifestAssetsPath = AndroidManifestAssetsDirectory + "AndroidManifest.xml";

        public string SwitchToInstant()
        {
            XDocument doc;
            if (File.Exists(AndroidManifestAssetsPath))
            {
                Debug.LogFormat("Loading existing file {0}", AndroidManifestAssetsPath);
                doc = XDocument.Load(AndroidManifestAssetsPath);
            }
            else
            {
                Debug.Log("Creating new manifest file");
                doc = AndroidManifestHelper.CreateManifestXDocument();
            }

            var errorMessage =
                AndroidManifestHelper.ConvertManifestToInstant(doc, PlayInstantBuildConfig.PlayGamesEnabled);
            if (errorMessage != null)
            {
                return errorMessage;
            }

            if (!Directory.Exists(AndroidManifestAssetsDirectory))
            {
                Directory.CreateDirectory(AndroidManifestAssetsDirectory);
            }

            doc.Save(AndroidManifestAssetsPath);

            Debug.LogFormat("Successfully updated {0}", AndroidManifestAssetsPath);
            return null;
        }

        public void SwitchToInstalled()
        {
            if (!File.Exists(AndroidManifestAssetsPath))
            {
                Debug.LogFormat("Nothing to do for {0} since file does not exist", AndroidManifestAssetsPath);
                return;
            }

            Debug.LogFormat("Loading existing file {0}", AndroidManifestAssetsPath);
            var doc = XDocument.Load(AndroidManifestAssetsPath);
            AndroidManifestHelper.ConvertManifestToInstalled(doc);
            doc.Save(AndroidManifestAssetsPath);
            Debug.LogFormat("Successfully updated {0}", AndroidManifestAssetsPath);
        }
    }
}