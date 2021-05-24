// Copyright 2020 Google LLC
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
using System.Linq;
using System.Text.RegularExpressions;
using Google.Android.AppBundle.Editor.Internal.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Google.Play.Billing.Editor
{
    /// <summary>
    /// Helper class to address building issues of Google Play Billing Plugin.
    /// </summary>
    public class GooglePlayBillingBuildHelper
    {
        // Expected path of Play Billing AAR when Unity IAP is installed via .unitypackage.
        private const string UnityIapGoogleAndroidAarPath = "Assets/Plugins/UnityPurchasing/Bin/Android";

        private const string UnityIapGoogleAndroidAarBackupPath = "Assets/GooglePlayBillingBackup~";

        private const string UnityIapGoogleAidlAarFileName = "GoogleAIDL.aar";
        private const string UnityIapGoogleAidlAarMetaDataFileName = "GoogleAIDL.aar.meta";

        private static readonly Regex UnityIapGooglePlayBillingAarFileNameRegex =
            RegexHelper.CreateCompiled(@"^billing[-\w.]+\.aar$");

        private static readonly string UnityIapGoogleAidlAar =
            Path.Combine(UnityIapGoogleAndroidAarPath, UnityIapGoogleAidlAarFileName);

        private static readonly string UnityIapGoogleAidlAarBackup = Path.Combine(
            UnityIapGoogleAndroidAarBackupPath, UnityIapGoogleAidlAarFileName);

        private static readonly string UnityIapGoogleAidlAarMetaData =
            Path.Combine(UnityIapGoogleAndroidAarPath, UnityIapGoogleAidlAarMetaDataFileName);

        private static readonly string UnityIapGoogleAidlAarMetaDataBackup = Path.Combine(
            UnityIapGoogleAndroidAarBackupPath, UnityIapGoogleAidlAarMetaDataFileName);

        /// <summary>
        /// Returns whether there is a conflicting aar file that will cause failure when building the APK.
        /// </summary>
        public static bool HasConflictingGoogleAarFile()
        {
            return HasConflictingGoogleAidlFile() || HasConflictingGooglePlayBillingAarFile();
        }

        /// <summary>
        /// Returns whether there is a conflicting GoogleAIDL.aar file that will cause failure when building the APK.
        /// </summary>
        private static bool HasConflictingGoogleAidlFile()
        {
            return File.Exists(UnityIapGoogleAidlAar);
        }

        /// <summary>
        /// Returns whether there is a conflicting billing-version.aar file that will cause failure when building the
        /// APK.
        /// </summary>
        private static bool HasConflictingGooglePlayBillingAarFile()
        {
            return GetConflictingGooglePlayBillingAarFileName() != null;
        }

        /// <summary>
        /// Returns the conflicting billing-version.aar file name if it exists, or null otherwise.
        /// </summary>
        private static string GetConflictingGooglePlayBillingAarFileName()
        {
            var files = Directory.GetFiles(UnityIapGoogleAndroidAarPath);
            foreach (var file in files)
            {
                if (UnityIapGooglePlayBillingAarFileNameRegex.IsMatch(Path.GetFileName(file)))
                {
                    return Path.GetFileName(file);
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the conflicting billing-version.aar file name in backup path if it exists, or null otherwise.
        /// </summary>
        private static string GetConflictingGooglePlayBillingAarBackupFileName()
        {
            var files = Directory.GetFiles(UnityIapGoogleAndroidAarBackupPath);
            foreach (var file in files)
            {
                if (UnityIapGooglePlayBillingAarFileNameRegex.IsMatch(Path.GetFileName(file)))
                {
                    return Path.GetFileName(file);
                }
            }

            return null;
        }

        /// <summary>
        /// Remove the conflicting GoogleAIDL.aar and GooglePlayBilling.aar from Assets scope by moving it to a backup
        /// directory.
        /// </summary>
        /// <returns> true if operation succeed, otherwise false. </returns>
        /// TODO : display error message on UI.
        public static bool RemoveConflictingAarFiles()
        {
            if (!HasConflictingGoogleAidlFile() && !HasConflictingGooglePlayBillingAarFile())
            {
                return true;
            }

            try
            {
                Directory.CreateDirectory(UnityIapGoogleAndroidAarBackupPath);
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogErrorFormat(
                    "Permission denied when creating backup directory {0}. Please manually create the directory. Exception: {1}",
                    UnityIapGoogleAndroidAarBackupPath, ex);
                return false;
            }

            if (HasConflictingGoogleAidlFile())
            {
                try
                {
                    // Delete the files before moving. The operation is NO-OP if the file doesn't exist.
                    File.Delete(UnityIapGoogleAidlAarBackup);
                    File.Delete(UnityIapGoogleAidlAarMetaDataBackup);
                    File.Move(UnityIapGoogleAidlAar, UnityIapGoogleAidlAarBackup);
                    File.Move(UnityIapGoogleAidlAarMetaData, UnityIapGoogleAidlAarMetaDataBackup);
                    Debug.LogFormat("Successfully backed up GoogleAIDL.aar to {0}", UnityIapGoogleAndroidAarBackupPath);
                }
                catch (FileNotFoundException)
                {
                    // This could only be the .meta file as we checked for the .aar file in the if. Ignore it.
                    Debug.LogWarningFormat("GoogleAIDL.aar.meta file is not found. Skip backing it up.");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.LogErrorFormat(
                        "Permission denied when move GoogleAIDL.aar file to the backup directory. " +
                        "Please check the directory access of {0}. Exception: {1}.",
                        UnityIapGoogleAndroidAarBackupPath, ex);
                    return false;
                }
            }

            if (HasConflictingGooglePlayBillingAarFile())
            {
                var fileName = GetConflictingGooglePlayBillingAarFileName();
                var unityIapGooglePlayBillingAar = Path.Combine(UnityIapGoogleAndroidAarPath, fileName);
                var unityIapGooglePlayBillingAarMetaData =
                    Path.Combine(UnityIapGoogleAndroidAarPath, fileName + ".meta");
                var unityIapGooglePlayBillingAarBackup = Path.Combine(UnityIapGoogleAndroidAarBackupPath, fileName);
                var unityIapGooglePlayBillingAarMetaDataBackup =
                    Path.Combine(UnityIapGoogleAndroidAarBackupPath, fileName + ".meta");
                try
                {
                    // Delete the files before moving. The operation is NO-OP if the file doesn't exist.
                    File.Delete(unityIapGooglePlayBillingAarBackup);
                    File.Delete(unityIapGooglePlayBillingAarMetaDataBackup);
                    File.Move(unityIapGooglePlayBillingAar, unityIapGooglePlayBillingAarBackup);
                    File.Move(unityIapGooglePlayBillingAarMetaData, unityIapGooglePlayBillingAarMetaDataBackup);
                    Debug.LogFormat("Successfully backed up billing-version.aar to {0}",
                        UnityIapGoogleAndroidAarBackupPath);
                }
                catch (FileNotFoundException)
                {
                    // This could only be the .meta file as we checked for the .aar file in the if. Ignore it.
                    Debug.LogWarningFormat("GooglePlayBilling.aar.meta file is not found. Skip backing it up.");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.LogErrorFormat(
                        "Permission denied when move billing-version.aar file to the backup directory. " +
                        "Please check the directory access of {0}. Exception: {1}.",
                        UnityIapGoogleAndroidAarBackupPath, ex);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Restore the conflicting GoogleAIDL.aar and GooglePlayBilling.aar back to Assets scope by moving it back from
        /// the backup directory.
        /// </summary>
        /// <returns> true if operation succeed, otherwise false. </returns>
        /// TODO : display error message on UI.
        public static bool RestoreConflictingAarFiles()
        {
            if (HasConflictingGoogleAidlFile() && HasConflictingGooglePlayBillingAarFile())
            {
                return true;
            }

            bool fileRestored = false;

            if (File.Exists(UnityIapGoogleAidlAarBackup))
            {
                try
                {
                    File.Move(UnityIapGoogleAidlAarBackup, UnityIapGoogleAidlAar);
                    File.Move(UnityIapGoogleAidlAarMetaDataBackup, UnityIapGoogleAidlAarMetaData);
                    Debug.LogFormat("Successfully restored GoogleAIDL.aar to {0}", UnityIapGoogleAndroidAarPath);
                    fileRestored = true;
                }
                catch (FileNotFoundException)
                {
                    // This can only be the .meta file. It's OK as Unity will regenerate one.
                    Debug.LogWarningFormat("GoogleAIDL.aar.meta file is not found. Skip restoring it up.");
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat(
                        "Cannot restore GoogleAIDL.aar file due to exception {0}. Please manually re-import Unity IAP.",
                        ex);
                }
            }
            else
            {
                Debug.LogWarningFormat(
                    "Cannot find the backup GoogleAIDL.aar file. Restore failed! Please manually re-import Unity IAP.");
            }

            var fileName = GetConflictingGooglePlayBillingAarBackupFileName();
            if (fileName != null)
            {
                var unityIapGooglePlayBillingAar = Path.Combine(UnityIapGoogleAndroidAarPath, fileName);
                var unityIapGooglePlayBillingAarMetaData =
                    Path.Combine(UnityIapGoogleAndroidAarPath, fileName + ".meta");
                var unityIapGooglePlayBillingAarBackup = Path.Combine(UnityIapGoogleAndroidAarBackupPath, fileName);
                var unityIapGooglePlayBillingAarMetaDataBackup =
                    Path.Combine(UnityIapGoogleAndroidAarBackupPath, fileName + ".meta");
                try
                {
                    File.Move(unityIapGooglePlayBillingAarBackup, unityIapGooglePlayBillingAar);
                    File.Move(unityIapGooglePlayBillingAarMetaDataBackup, unityIapGooglePlayBillingAarMetaData);
                    Debug.LogFormat("Successfully restored GooglePlayBilling.aar to {0}", UnityIapGoogleAndroidAarPath);
                    fileRestored = true;
                }
                catch (FileNotFoundException)
                {
                    // This can only be the .meta file. It's OK as Unity will regenerate one.
                    Debug.LogWarningFormat("GooglePlayBilling.aar.meta file is not found. Skip restoring it up.");
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat(
                        "Cannot restore GooglePlayBilling.aar file due to exception {0}. Please manually re-import Unity IAP.",
                        ex);
                }
            }
            else
            {
                Debug.LogWarningFormat(
                    "Cannot find the backup GooglePlayBilling.aar file. Restore failed! Please manually re-import Unity IAP.");
            }

            // Only delete the directory if it is empty.
            if (!Directory.GetFileSystemEntries(UnityIapGoogleAndroidAarBackupPath).Any())
            {
                return fileRestored;
            }

            try
            {
                Directory.Delete(UnityIapGoogleAndroidAarBackupPath);
            }
            catch (IOException ex)
            {
                Debug.LogErrorFormat("Cannot delete {0} due to exception {1}.", UnityIapGoogleAndroidAarBackupPath,
                    ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.LogErrorFormat(
                    "Permission denied when deleting {0}. Please manually delete it. Exception: {1}.",
                    UnityIapGoogleAndroidAarBackupPath, ex);
            }

            return fileRestored;
        }
    }
}