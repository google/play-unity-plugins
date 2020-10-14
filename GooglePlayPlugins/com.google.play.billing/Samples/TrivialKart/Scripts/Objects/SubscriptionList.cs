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
/// Constant data for subscriptions in the game.
/// </summary>
public static class SubscriptionList
{
    public class Subscription
    {
        public class VipSubscribeButton
        {
            public VipSubscribeButton(string subscribeButtonTextWhenNoSubscription,
                string subscribeButtonTextWhenSubscribeToOthers,
                string subscribeButtonTextWhenSubscribeToThisSubscription)
            {
                SubscribeButtonTextWhenNoSubscription = subscribeButtonTextWhenNoSubscription;
                SubscribeButtonTextWhenSubscribeToOthers = subscribeButtonTextWhenSubscribeToOthers;
                SubscribeButtonTextWhenSubscribeToThisSubscription = subscribeButtonTextWhenSubscribeToThisSubscription;
            }

            public string SubscribeButtonTextWhenSubscribeToThisSubscription { get; }

            public string SubscribeButtonTextWhenSubscribeToOthers { get; }

            public string SubscribeButtonTextWhenNoSubscription { get; }

            public GameObject ButtonGameObj { get; set; }
        }

        public Subscription(string name, SubscriptionType type, float price, string productId,
            string subscribeButtonTextWhenNoSubscription,
            string subscribeButtonTextWhenSubscribeToOthers,
            string subscribeButtonTextWhenSubscribeToThisSubscription)
        {
            Name = name;
            Type = type;
            Price = price;
            ProductId = productId;
            SubscribeButton = new VipSubscribeButton(subscribeButtonTextWhenNoSubscription,
                subscribeButtonTextWhenSubscribeToOthers,
                subscribeButtonTextWhenSubscribeToThisSubscription);
        }

        public string Name { get; }

        public SubscriptionType Type { get; }

        public float Price { get; set; }

        public GameObject SubscribeButtonGameObj
        {
            get => SubscribeButton.ButtonGameObj;
            set => SubscribeButton.ButtonGameObj = value;
        }

        public string ProductId { get; }

        public VipSubscribeButton SubscribeButton { get; }
    }

    public static Subscription SilverSubscription =
        new Subscription("silver VIP", SubscriptionType.SilverSubscription, 1.99f,
            "silver_subscription", " subscribe now! ", "downgrade", "subscribed");

    public static Subscription GoldenSubscription =
        new Subscription("golden VIP", SubscriptionType.GoldenSubscription, 4.99f,
            "golden_subscription", " subscribe now! ", "upgrade!", "subscribed");

    public static Subscription NoSubscription =
        new Subscription("No VIP", SubscriptionType.NoSubscription, 0f, "no_subscription", "", "", "");

    public static List<Subscription> List = new List<Subscription>()
        {NoSubscription, SilverSubscription, GoldenSubscription};

    public static Subscription GetSubscriptionByType(SubscriptionType subscriptionType)
    {
        return List[(int) subscriptionType];
    }
    public static Subscription GetSubscriptionByIndex(int subscriptionIndex)
    {
        return List[subscriptionIndex];
    }
}