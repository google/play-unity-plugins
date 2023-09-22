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
// limitations under the License.

using System;
using System.Collections.Generic;

namespace Google.Play.Integrity.Internal
{
    /// <summary>
    /// Translates PlayCore's IntegrityErrorCode Java enum into its corresponding public-facing Unity enum.
    /// </summary>
    internal static class PlayCoreTranslator
    {
        private static class JavaIntegrityErrorCode
        {
            public const int NoError = 0;
            public const int ApiNotAvailable = -1;
            public const int PlayStoreNotFound = -2;
            public const int NetworkError = -3;
            public const int PlayStoreAccountNotFound = -4;
            public const int AppNotInstalled = -5;
            public const int PlayServicesNotFound = -6;
            public const int AppUidMismatch = -7;
            public const int TooManyRequests = -8;
            public const int CannotBindToService = -9;
            public const int NonceTooShort = -10;
            public const int NonceTooLong = -11;
            public const int GoogleServerUnavailable = -12;
            public const int NonceIsNotBase64 = -13;
            public const int PlayStoreVersionOutdated = -14;
            public const int PlayServicesVersionOutdated = -15;
            public const int CloudProjectNumberIsInvalid = -16;
            public const int ClientTransientError = -17;
            public const int InternalError = -100;
        }

        private static class JavaStandardIntegrityErrorCode
        {
            public const int NoError = 0;
            public const int ApiNotAvailable = -1;
            public const int PlayStoreNotFound = -2;
            public const int NetworkError = -3;
            public const int AppNotInstalled = -5;
            public const int PlayServicesNotFound = -6;
            public const int AppUidMismatch = -7;
            public const int TooManyRequests = -8;
            public const int CannotBindToService = -9;
            public const int GoogleServerUnavailable = -12;
            public const int PlayStoreVersionOutdated = -14;
            public const int PlayServicesVersionOutdated = -15;
            public const int CloudProjectNumberIsInvalid = -16;
            public const int RequestHashTooLong = -17;
            public const int ClientTransientError = -18;
            public const int IntegrityTokenProviderInvalid = -19;
            public const int InternalError = -100;
        }

        private static readonly Dictionary<int, IntegrityErrorCode> PlayCoreToIntegrityErrors =
            new Dictionary<int, IntegrityErrorCode>
            {
                { JavaIntegrityErrorCode.NoError, IntegrityErrorCode.NoError },
                { JavaIntegrityErrorCode.ApiNotAvailable, IntegrityErrorCode.ApiNotAvailable },
                { JavaIntegrityErrorCode.PlayStoreNotFound, IntegrityErrorCode.PlayStoreNotFound },
                { JavaIntegrityErrorCode.NetworkError, IntegrityErrorCode.NetworkError },
                { JavaIntegrityErrorCode.PlayStoreAccountNotFound, IntegrityErrorCode.PlayStoreAccountNotFound },
                { JavaIntegrityErrorCode.AppNotInstalled, IntegrityErrorCode.AppNotInstalled },
                { JavaIntegrityErrorCode.PlayServicesNotFound, IntegrityErrorCode.PlayServicesNotFound },
                { JavaIntegrityErrorCode.AppUidMismatch, IntegrityErrorCode.AppUidMismatch },
                { JavaIntegrityErrorCode.TooManyRequests, IntegrityErrorCode.TooManyRequests },
                { JavaIntegrityErrorCode.CannotBindToService, IntegrityErrorCode.CannotBindToService },
                { JavaIntegrityErrorCode.NonceTooShort, IntegrityErrorCode.NonceTooShort },
                { JavaIntegrityErrorCode.NonceTooLong, IntegrityErrorCode.NonceTooLong },
                { JavaIntegrityErrorCode.GoogleServerUnavailable, IntegrityErrorCode.GoogleServerUnavailable },
                { JavaIntegrityErrorCode.NonceIsNotBase64, IntegrityErrorCode.NonceIsNotBase64 },
                { JavaIntegrityErrorCode.PlayStoreVersionOutdated, IntegrityErrorCode.PlayStoreVersionOutdated },
                { JavaIntegrityErrorCode.PlayServicesVersionOutdated, IntegrityErrorCode.PlayServicesVersionOutdated },
                { JavaIntegrityErrorCode.InternalError, IntegrityErrorCode.InternalError },
                { JavaIntegrityErrorCode.CloudProjectNumberIsInvalid, IntegrityErrorCode.CloudProjectNumberIsInvalid },
                { JavaIntegrityErrorCode.ClientTransientError, IntegrityErrorCode.ClientTransientError },
            };

        private static readonly Dictionary<int, StandardIntegrityErrorCode> PlayCoreToStandardIntegrityErrors =
            new Dictionary<int, StandardIntegrityErrorCode>
            {
                { JavaIntegrityErrorCode.NoError, StandardIntegrityErrorCode.NoError },
                { JavaIntegrityErrorCode.ApiNotAvailable, StandardIntegrityErrorCode.ApiNotAvailable },
                { JavaIntegrityErrorCode.PlayStoreNotFound, StandardIntegrityErrorCode.PlayStoreNotFound },
                { JavaIntegrityErrorCode.NetworkError, StandardIntegrityErrorCode.NetworkError },
                { JavaIntegrityErrorCode.AppNotInstalled, StandardIntegrityErrorCode.AppNotInstalled },
                { JavaIntegrityErrorCode.PlayServicesNotFound, StandardIntegrityErrorCode.PlayServicesNotFound },
                { JavaIntegrityErrorCode.AppUidMismatch, StandardIntegrityErrorCode.AppUidMismatch },
                { JavaIntegrityErrorCode.TooManyRequests, StandardIntegrityErrorCode.TooManyRequests },
                { JavaIntegrityErrorCode.CannotBindToService, StandardIntegrityErrorCode.CannotBindToService },
                { JavaIntegrityErrorCode.GoogleServerUnavailable, StandardIntegrityErrorCode.GoogleServerUnavailable },
                {
                    JavaIntegrityErrorCode.PlayStoreVersionOutdated, StandardIntegrityErrorCode.PlayStoreVersionOutdated
                },
                {
                    JavaIntegrityErrorCode.PlayServicesVersionOutdated,
                    StandardIntegrityErrorCode.PlayServicesVersionOutdated
                },
                {
                    JavaIntegrityErrorCode.CloudProjectNumberIsInvalid,
                    StandardIntegrityErrorCode.CloudProjectNumberIsInvalid
                },
                { JavaStandardIntegrityErrorCode.RequestHashTooLong, StandardIntegrityErrorCode.RequestHashTooLong },
                { JavaStandardIntegrityErrorCode.ClientTransientError, StandardIntegrityErrorCode.ClientTransientError },
                { JavaStandardIntegrityErrorCode.IntegrityTokenProviderInvalid, StandardIntegrityErrorCode.IntegrityTokenProviderInvalid },
                { JavaIntegrityErrorCode.InternalError, StandardIntegrityErrorCode.InternalError },
            };

        /// <summary>
        /// Translates Play Core's IntegrityErrorCode into its corresponding public-facing IntegrityErrorCode.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided Java error code does not have a corresponding value in IntegrityErrorCode.
        /// </exception>
        public static IntegrityErrorCode TranslatePlayCoreErrorCode(int javaIntegrityErrorCode)
        {
            IntegrityErrorCode translatedErrorCode;
            if (!PlayCoreToIntegrityErrors.TryGetValue(javaIntegrityErrorCode, out translatedErrorCode))
            {
                throw new NotImplementedException("Unexpected error code: " + javaIntegrityErrorCode);
            }

            return translatedErrorCode;
        }

        /// <summary>
        /// Translates Play Core's StandardIntegrityErrorCode into its corresponding public-facing
        /// StandardIntegrityErrorCode.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// Throws if the provided Java error code does not have a corresponding value in StandardIntegrityErrorCode.
        /// </exception>
        public static StandardIntegrityErrorCode TranslatePlayCoreStandardIntegrityErrorCode(
            int javaStandardIntegrityErrorCode)
        {
            StandardIntegrityErrorCode translatedErrorCode;
            if (!PlayCoreToStandardIntegrityErrors.TryGetValue(javaStandardIntegrityErrorCode, out translatedErrorCode))
            {
                throw new NotImplementedException("Unexpected error code: " + javaStandardIntegrityErrorCode);
            }

            return translatedErrorCode;
        }
    }
}