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

/// <summary>
/// Constant data for coin store items.
/// </summary>
public static class CoinList
{
    public class Coin
    {
        public Coin(int amount, float price, string productId)
        {
            Amount = amount;
            Price = price;
            ProductId = productId;
        }

        public int Amount { get; }

        public float Price { get; set; }

        public string ProductId { get; }

        public GameObject StoreItemCoinGameObj { get; set; }
    }

    public static Coin FiveCoins = new Coin(5, 0.99f, "five_coins");
    public static Coin TenCoins = new Coin(10, 1.99f, "ten_coins");
    public static Coin TwentyCoins = new Coin(20, 2.49f, "twenty_coins");
    public static Coin FiftyCoins = new Coin(50, 4.99f, "fifty_coins");
    public static List<Coin> List = new List<Coin>() {FiveCoins, TenCoins, TwentyCoins, FiftyCoins};

    public static Coin GetCoinByCoinIndex(int coinIndex)
    {
        return List[coinIndex];
    }
}