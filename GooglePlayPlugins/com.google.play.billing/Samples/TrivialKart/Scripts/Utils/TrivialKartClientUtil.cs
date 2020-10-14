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

using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class TrivialKartClientUtil
{
    // Create and return obfuscated account id based on the sha256 hash result of account id.
    // Device unique id is used here as account id for simplicity on client side app.
    // You would not want to use this for user authentication in a production game.
    public static string GetObfuscatedAccountId()
    {
        byte[] bytes = Encoding.Unicode.GetBytes(SystemInfo.deviceUniqueIdentifier);
        SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
        byte[] hashBytes = sha256.ComputeHash(bytes);
        StringBuilder hashStringBuilder = new StringBuilder();

        foreach (byte b in hashBytes)
        {
            hashStringBuilder.AppendFormat("{0:X2}", b);
        }

        return hashStringBuilder.ToString();
    }
}