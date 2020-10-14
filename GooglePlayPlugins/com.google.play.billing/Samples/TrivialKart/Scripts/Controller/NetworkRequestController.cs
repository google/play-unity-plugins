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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using static Server;

public class NetworkRequestController
{
    // If you are using the 'USE_SERVER' server purchase validation path, you
    // will need to update these URLs to point to your deployed Firebase
    // Functions in your Firebase project for the sample.
    private const string REGISTER_URL = "https://YOURFIREBASEPROJECT.cloudfunctions.net/register_user";

    private const string VERIFY_AND_SAVE_TOKEN_URL =
        "https://YOURFIREBASEPROJECT.cloudfunctions.net/verify_and_save_purchase_token";

    private const string SAVE_GAME_DATA_URL =
        "https://YOURFIREBASEPROJECT.cloudfunctions.net/save_game_data";

    private const string GET_GAME_DATA_URL = "https://YOURFIREBASEPROJECT.cloudfunctions.net/get_game_data";

    private const string CHECK_SUBSCRIPTION_PRICE_CHANGE =
        "https://YOURFIREBASEPROJECT.cloudfunctions.net/check_subscription_price_change";

    public static ServerResponseModel RegisterUserDevice()
    {
        var values = new Dictionary<string, string> { };
        return sendUnityWebRequest(values, REGISTER_URL);
    }

    public static ServerResponseModel SaveGameDataOnline()
    {
        var values = new Dictionary<string, string>
        {
            ["gameData"] = JsonUtility.ToJson(GameDataController.GetGameData())
        };

        return sendUnityWebRequest(values, SAVE_GAME_DATA_URL);
    }

    public static void LoadGameOnline()
    {
        ServerResponseModel serverResponse =
            sendUnityWebRequest(new Dictionary<string, string>(), GET_GAME_DATA_URL);
        if (serverResponse.success)
        {
            Debug.Log(serverResponse.result);
            GameDataController.SetGameData(JsonUtility.FromJson<GameData>(serverResponse.result));
        }
        else
        {
            Debug.Log("LoadGameOnline no success from server, using default GameData");
            GameDataController.SetGameData(new GameData());
        }
    }

    public static void VerifyAndSaveUserPurchase(Product product)
    {
        var values = new Dictionary<string, string>
        {
            ["receipt"] = product.receipt
        };

        ServerResponseModel serverResponse = sendUnityWebRequest(values, VERIFY_AND_SAVE_TOKEN_URL);
        PurchaseController.ConfirmPendingPurchase(product, serverResponse.success);
    }

    public static void CheckSubscriptionPriceChange()
    {
        var values = new Dictionary<string, string> { };
        ServerResponseModel serverResponse = sendUnityWebRequest(values, CHECK_SUBSCRIPTION_PRICE_CHANGE);
        if (serverResponse.success)
        {
            PurchaseController.confirmSubscriptionPriceChange(serverResponse.result);
        }
    }
}
