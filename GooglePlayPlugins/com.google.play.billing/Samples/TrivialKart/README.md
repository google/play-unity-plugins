# TrivialKart

A sample game demonstrating use of the Google Play Billing Library plugin for Unity.

## Introduction

The purpose of this project is to provide an example of using the Google Play Billing Library plugin for Unity to support in-app purchases and subscriptions via the Unity IAP API.

In the example game, the player has a vehicle which they can drive by tapping on it. Driving the car uses gas. When the car runs out of gas, to continue driving, more must be purchased using in-game currency. If the player runs out of in-game currency, they may buy more via in-app purchases. In-game currency is an example of a consumable purchase that may be repeated.

The game has different cars available for unlock. One car is purchasable using in-game currency; other cars require unlocking via in-app purchase. These unlocks are permanent one-time purchases.

The game also has subscriptions available for in-app purchase. One subscription unlocks a different travel background. A second, more expensive subscription unlocks the travel background and adds a 40% discount off the cost of purchasing gas or unlocks using in-game currency. The features of a subscription are only available if the subscription is active.

## Pre-requisites

- Unity 2017 LTS, 2018 LTS, 2019 LTS, 2020 or later with Android build support
- Google Play developer account

## Documentation

- [Unity IAP](https://docs.unity3d.com/Manual/UnityIAP.html)
- [Google Play Billing Library plugin for Unity](https://developer.android.com/google/play/billing/unity)

## Getting Started

### Creating a project in the Google Play Developer Console

Note: Google Play Console instructions are referencing the new version of the console that was released in Q4 2020.

1. Go to the [Google Play Developer Console](https://play.google.com/apps/publish) and create a new application.
2. Select the `Monetization setup` page for your new application. Copy the Base64-encoded public key text in the `Licensing` area. You will need to use this key in your Unity project.

### Creating a project in Unity

1. Create a new 2D project in Unity
2. From the Unity menu bar, choose `File -> Build Settings` and switch to the `Android` platform.

### Importing the Google Play Billing Library Plugin

Follow the instructions at: [Use the Google Play Billing Library with Unity](https://developer.android.com/google/play/billing/unity)

It is important to note that you will need to follow the instructions in the above linkto enable the Unity IAP services and import the Unity IAP project before importing the Google Play Billing plugin. The exact details will vary slightly depending on the version of Unity being used.

### Setting up receipt obfuscation

1. Select `Window -> Unity IAP -> Receipt Validation Obfuscation` from the Unity menu bar.
2. Paste the Base64-encoded public key you retrieved from the Google Play Developer console into the text box.
3. Click the `Obfuscate Google Play License Key` button.
4. Close the popup window.

### Importing the TrivialKart assets

Copy the `TrivialKart` folder into the `Assets` folder of your new Unity project.

### Building and uploading a signed build

1. In the Unity `Build Settings` window, make sure the `TrivialKart/Scenes/playScene` is added to the list of `Scenes In Build`.
2. Configure the project for signing by creating a new key store at `File > Build Settings > Player Settings > Publishing Settings`. Additional instructions on configurating signing are available [on the Unity site](https://answers.unity.com/questions/326812/signing-android-application.html).
3. Build the signed APK or App Bundle
4. Use the Google Play Developer Console to upload your build  to Google Play. You can upload to the Internal test track.

### Setting up the purchase items in Google Play

*Note:* you must upload a build to Google Play to be able to create your in app purchasing items in the
Google Play Developer Console Console.

1. Return to the Google Play Developer Console.
2. Under `Monetize -> Products -> In-app products`, create the following in-app products (note: if you are using Classic Play Console, create MANAGED in-app items under In-app Products) with specified IDs and prices (Fill out the other fields. Set them to "Active"):
      | Product ID   |  Price|
      | :---:        | :---: |
      | car_jeep     | $2.99 |
      | car_kart     | $4.99 |
      | five_coins   | $0.99 |
      | ten_coins    | $1.99 |
      |twenty_coints | $2.49 |
      |fifty_coins   | $4.99 |

3. Under `Monetize -> Products -> Subscriptions`, create subscriptions with these IDs and prices (Fill out the other fields. Set them to "Active"):
     | Product ID   |  Price|
     | :---:        | :---: |
     | silver_subscription   | $1.99 |
     | golden_subscription    | $4.99 |

4. Publish your build to the testing channel. It may take up to a few hours to process the build. Running a build before processing complete can result in errors such as Google Play reporting that "this version of the application is not enabled for in-app billing".
5. Add tester accounts to your game. This will allow you to test purchases and subscriptions without being charged. Test accounts also have greatly reduced subscription periods, allowing for easier testing of subscription features.

## CLIENT-SIDE SECURITY

By default TrivialKart does basic client-side validation using the built-in Unity IAP functionality.

TrivialKart implements optional signature verifcation and reciept validation with an obfuscated account id,
but does not demonstrate how to enforce a tight security model. When releasing a production application to the general public, we highly recommend that you implement the security best practices described in our documentation at:

http://developer.android.com/google/play/billing/billing_best_practices.html

In particular, you should perform a security check using purchase validation an on independent server.

## OPTIONAL SERVER VALIDATION

TrivialKart supports optional server-side purchase validation. When enabled, this also saves user purchase data on the server, and retrieves it from the server to 'load' the user data. To build TrivialKart with server validation, make sure the Unity menu item
`TrivialKart -> BuildOptions -> Build with server validation` is checked. When this is checked a `USE_SERVER` global define is set in the Unity `Scripting Define Symbols` field located in `Project Settings -> Player -> Other Settings`.

The server project is located in the [Play Billing Samples repo](https://github.com/android/play-billing-samples) in the `UnityServer` directory.

The server validation example is intended to demonstrate the basic operation of purchase verification and does not include any sort of user authentication, which would be needed for a production system. For a production implementation, consider solutions such as Firebase [Authentication](https://firebase.google.com/docs/auth), which includes client-side plugin support for Unity.

## Support
If you've found any errors or bugs in this sample game, please file an issue: https://github.com/google/play-unity-plugins/issues

Patches are encouraged, and may be submitted by forking this project and submitting a pull request through GitHub.

This is not an officially supported Google product.

## License
Copyright 2020 Google LLC

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Graphic asset files created by [Kenney](https://www.kenney.nl/) are licensed under the CC0 1.0 Universal license

https://creativecommons.org/publicdomain/zero/1.0/

## CHANGELOG
2020-10-01: Initial release

## Acknowledgements
This project contains graphic assets created by [Kenney](https://www.kenney.nl/). Thanks Kenney for the amazing assets.
