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
using System.Text;
using Google.Play.Common;
using Google.Play.Common.Internal;
using UnityEngine;

namespace Google.Play.Instant
{
    /// <summary>
    /// Provides methods that an instant app can use to store a persistent cookie or that an installed app
    /// can use to retrieve the cookie that it persisted as an instant app.
    ///
    /// This is a C# implementation of some of the Java methods available in
    /// <a href="https://developers.google.com/android/reference/com/google/android/gms/instantapps/PackageManagerCompat.html">
    /// Google Play Services' PackageManagerCompat class</a>.
    /// </summary>
    public static class CookieApi
    {
        /// <summary>
        /// An exception thrown by the methods of <see cref="CookieApi"/> if there is a failure while
        /// making a call to Google Play Services.
        /// </summary>
        public class CookieApiException : Exception
        {
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            /// <param name="innerException">The underlying cause of this exception.</param>
            public CookieApiException(string message, Exception innerException) : base(message, innerException)
            {
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            public CookieApiException(string message) : base(message)
            {
            }
        }

        // Constants specific to PackageManagerCompat.
        private const string Authority = "com.google.android.gms.instantapps.provider.api";
        private const string ContentAuthority = "content://" + Authority + "/";
        private const string KeyCookie = "cookie";
        private const string KeyResult = "result";
        private const string KeyUid = "uid";
        private const string MethodGetInstantAppCookie = "getInstantAppCookie";
        private const string MethodGetInstantAppCookieMaxSize = "getInstantAppCookieMaxSize";
        private const string MethodSetInstantAppCookie = "setInstantAppCookie";

        private static bool _verifiedContentProvider;

        /// <summary>
        /// Gets the maximum size in bytes of the cookie data an instant app can store on the device.
        /// </summary>
        /// <exception cref="CookieApiException">Thrown if there is a failure to obtain the size.</exception>
        public static int GetInstantAppCookieMaxSizeBytes()
        {
            using (var extrasBundle = new AndroidJavaObject(AndroidConstants.BundleClass))
            using (var resultBundle = CallMethod(MethodGetInstantAppCookieMaxSize, extrasBundle))
            {
                return resultBundle.Call<int>(AndroidConstants.BundleMethodGetInt, KeyResult);
            }
        }

        /// <summary>
        /// Gets the instant app cookie for this app as a string. Assumes that the cookie was encoded with UTF-8,
        /// for example by <see cref="SetInstantAppCookie"/>. See <see cref="GetInstantAppCookieBytes"/>
        /// for more details.
        /// </summary>
        /// <exception cref="CookieApiException">Thrown if there is a failure to obtain the cookie.</exception>
        public static string GetInstantAppCookie()
        {
            var cookieBytes = GetInstantAppCookieBytes();
            return cookieBytes == null ? null : Encoding.UTF8.GetString(cookieBytes);
        }

        /// <summary>
        /// Gets the instant app cookie for this app as bytes. Non instant apps and apps that were instant but
        /// were upgraded to normal apps can still access this API. For instant apps this cookie is cached for
        /// some time after uninstall while for normal apps the cookie is deleted after the app is uninstalled.
        /// The cookie is always present while the app is installed.
        /// </summary>
        /// <exception cref="CookieApiException">Thrown if there is a failure to obtain the cookie.</exception>
        public static byte[] GetInstantAppCookieBytes()
        {
            using (var extrasBundle = new AndroidJavaObject(AndroidConstants.BundleClass))
            {
                extrasBundle.Call(AndroidConstants.BundleMethodPutInt, KeyUid, ProcessGetMyUid());
                using (var resultBundle = CallMethod(MethodGetInstantAppCookie, extrasBundle))
                {
                    return resultBundle.Call<byte[]>(AndroidConstants.BundleMethodGetByteArray, KeyResult);
                }
            }
        }

        /// <summary>
        /// Sets the instant app cookie as a string for the calling app. The cookie string will be encoded with UTF-8
        /// and can be recalled using <see cref="GetInstantAppCookie"/>. See <see cref="SetInstantAppCookieBytes"/>
        /// for more details.
        /// Note: the length of the cookie string may not be directly comparable to the size limit indicated by
        /// <see cref="GetInstantAppCookieMaxSizeBytes"/>.
        /// </summary>
        /// <param name="cookie">The cookie string.</param>
        /// <returns>True if the cookie was set. False if cookie is too large or I/O fails.</returns>
        /// <exception cref="CookieApiException">Thrown if there is a failure to set the cookie.</exception>
        public static bool SetInstantAppCookie(string cookie)
        {
            var cookieBytes = cookie == null ? null : Encoding.UTF8.GetBytes(cookie);
            return SetInstantAppCookieBytes(cookieBytes);
        }

        /// <summary>
        /// Sets the instant app cookie as bytes for the calling app. Non instant apps and apps that were instant
        /// but were upgraded to normal apps can still access this API. For instant apps this cookie is cached
        /// for some time after uninstall while for normal apps the cookie is deleted after the app is uninstalled.
        /// The cookie is always present while the app is installed. The cookie size is limited by
        /// <see cref="GetInstantAppCookieMaxSizeBytes"/>. If the provided cookie size is over the limit,
        /// this method returns false. Passing null or an empty array clears the cookie.
        /// </summary>
        /// <param name="cookie">The cookie bytes.</param>
        /// <returns>True if the cookie was set. False if cookie is too large or I/O fails.</returns>
        /// <exception cref="CookieApiException">Thrown if there is a failure to set the cookie.</exception>
        public static bool SetInstantAppCookieBytes(byte[] cookie)
        {
            using (var extrasBundle = new AndroidJavaObject(AndroidConstants.BundleClass))
            {
                extrasBundle.Call(AndroidConstants.BundleMethodPutInt, KeyUid, ProcessGetMyUid());
                extrasBundle.Call(AndroidConstants.BundleMethodPutByteArray, KeyCookie, cookie);
                using (var resultBundle = CallMethod(MethodSetInstantAppCookie, extrasBundle))
                {
                    return resultBundle.Call<bool>(AndroidConstants.BundleMethodGetBoolean, KeyResult);
                }
            }
        }

        private static void VerifyContentProvider()
        {
            if (_verifiedContentProvider)
            {
                return;
            }

            // Java: currentActivity.getPackageManager().resolveContentProvider("com.google.android.gms...", 0)
            using (var context = UnityPlayerHelper.GetCurrentActivity())
            using (var packageManager = context.Call<AndroidJavaObject>(AndroidConstants.ContextMethodGetPackageManager))
            using (var providerInfo = packageManager.Call<AndroidJavaObject>(
                AndroidConstants.PackageManagerMethodResolveContentProvider, Authority, 0))
            {
                if (!PlaySignatureVerifier.VerifyGooglePlayServices(packageManager))
                {
                    throw new CookieApiException("Failed to verify the signature of Google Play Services.");
                }

                if (providerInfo == null)
                {
                    throw new CookieApiException("Failed to resolve the instant apps content provider.");
                }

                var packageName = providerInfo.Get<string>(AndroidConstants.ProviderInfoFieldPackageName);
                if (!string.Equals(packageName, AndroidConstants.GooglePlayServicesPackageName))
                {
                    throw new CookieApiException(
                        string.Format("Package \"{0}\" is an invalid instant apps content provider.", packageName));
                }
            }

            Debug.Log("Verified instant apps content provider.");
            _verifiedContentProvider = true;
        }

        private static AndroidJavaObject CallMethod(string methodName, AndroidJavaObject extrasBundle)
        {
            VerifyContentProvider();
            AndroidJavaObject resultBundle;
            try
            {
                // Java: currentActivity.getContentResolver().call(Uri.parse("content://..."), methodName, null, extras)
                using (var context = UnityPlayerHelper.GetCurrentActivity())
                using (var contentResolver = context.Call<AndroidJavaObject>(AndroidConstants.ContextMethodGetContentResolver))
                using (var uriClass = new AndroidJavaClass(AndroidConstants.UriClass))
                using (var uri = uriClass.CallStatic<AndroidJavaObject>(AndroidConstants.UriMethodParse, ContentAuthority))
                {
                    resultBundle = contentResolver.Call<AndroidJavaObject>(
                        AndroidConstants.ContentResolverMethodCall, uri, methodName, null, extrasBundle);
                }
            }
            catch (AndroidJavaException ex)
            {
                throw new CookieApiException(
                    string.Format("Failed to call {0} on the instant apps content provider.", methodName), ex);
            }

            if (resultBundle == null)
            {
                // This should only happen if the content provider is unavailable.
                throw new CookieApiException(
                    string.Format("Null result calling {0} on the instant apps content provider.", methodName));
            }

            return resultBundle;
        }

        private static int ProcessGetMyUid()
        {
            // Java: Process.myUid()
            using (var processClass = new AndroidJavaClass(AndroidConstants.ProcessClass))
            {
                return processClass.CallStatic<int>(AndroidConstants.ProcessMethodMyUid);
            }
        }
    }
}