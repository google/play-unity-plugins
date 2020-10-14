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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class Server
{
    private static string userIdentification = SystemInfo.deviceUniqueIdentifier;

    public static ServerResponseModel sendUnityWebRequest(Dictionary<string, string> values, string url)
    {
        // For the purposes of this example, SystemInfo.deviceUniqueIdentifier is
        // used as a user ID on the server to associate purchase information.
        // This would not be adviseable in a production application.  
        // Solutions such as Firebase Authentication are available to integrate
        // for projects wanting to use server validation that will require
        // robust user identity and authentication support.
        values.Add("userId", userIdentification);
        ServerResponseModel result = new ServerResponseModel();
        WWWForm form = new WWWForm();

        foreach (KeyValuePair<string, string> entry in values)
        {
            form.AddField(entry.Key, entry.Value);
        }

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);

        uwr.SendWebRequest();
        while (!uwr.isDone)
        {
        }

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            result = JsonUtility.FromJson<ServerResponseModel>(uwr.downloadHandler.text);
        }

        return result;
    }
}