# Google Play Plugins for Unity

## Overview

The Google Play Plugins for Unity provide C# APIs for accessing various Play
services at runtime from within the Unity Engine. These plugins also provide
various Unity Editor features for building an app that can be published on
[Google Play Console](//play.google.com/console).

## Version support

These plugins officially support Unity version 2017.4.40 or newer, though they
were written to be code compatible with Unity versions 5.6.7 or newer.

[In August 2019 Google Play started requiring that any apps published with 32
bit native libraries also provide 64 bit native
libaries.](//android-developers.googleblog.com/2019/01/get-your-apps-ready-for-64-bit.html)
An exception was made at the time enabling apps built with Unity 5.6.7 and
earlier to still be published on the Google Play Console until August 2021,
which is fast approaching.

Note: In June 2021 we will remove any plugin code that enables compilation of
Unity 2017.4.16 or earlier (including Unity 5.6.7).

## Download and import the plugins

Although it is possible to `git clone` this repository into the **Assets**
folder of your Unity project, in general it is preferable to import a released
version.

To import the plugins, follow these steps:

1.  Download the latest release from this project's
    [releases page](//github.com/google/play-unity-plugins/releases).
1.  Import the `.unitypackage` file by selecting the Unity IDE menu option
    **Assets > Import package > Custom Package** and importing all items.

## Feature plugins

These plugins add features, such as Google Play Instant support, to a Unity
project.

### Android App Bundle

*com.google.android.appbundle*

The Google Android App Bundle package provides access to the latest Android App
Bundles features, such as Play Asset Delivery.

Refer to the
[documentation](//developer.android.com/guide/app-bundle/asset-delivery/build-unity)
and
[Editor API reference](//developer.android.com/reference/unity/namespace/Google/Android/AppBundle/Editor)
for more information.

### Play Asset Delivery

*com.google.play.assetdelivery*

Play Asset Delivery enables AssetBundles and other assets to be packaged into an
Android App Bundle and delivered through Google Play.

Refer to the
[documentation](//developer.android.com/guide/playcore/asset-delivery/integrate-unity)
and
[Runtime API reference](//developer.android.com/reference/unity/namespace/Google/Play/AssetDelivery)
for more information.

### Play Billing

*com.google.play.billing*

Package contains Google Play Billing Library, which is required to sell digital
content and subscriptions in games distributed via Google Play. This new version
of the Google Play Billing Library provides all of the features available in the
current Java and Kotlin versions.

To use this package, you must agree to the licenses for this Google Play Plugin
and the Google Play Billing Library. If you do not agree with the licenses, you
may not use this package.

Refer to the [documentation](//developer.android.com/google/play/billing/unity)
for more information.

### Play In-App Review

*com.google.play.review*

Play In-App Review lets you prompt users to submit Play Store ratings and
reviews without the inconvenience of leaving your game.

Refer to the
[documentation](//developer.android.com/guide/playcore/in-app-review/unity) and
[Runtime API reference](//developer.android.com/reference/unity/namespace/Google/Play/Review)
for more information.

### Play In-App Update

*com.google.play.appupdate*

Play In-App Update lets you keep your app up-to-date on your users’ devices and
enables them to try new features, as well as benefit from performance
improvements and bug fixes.

Refer to the
[documentation](//developer.android.com/guide/playcore/in-app-updates) for more
information.

### Play Instant

*com.google.play.instant*

The Google Play Instant package simplifies the conversion of an Android app into
an instant app that can be deployed through Google Play.

Refer to the
[documentation](//developer.android.com/topic/google-play-instant/getting-started/game-unity-plugin),
[Runtime API reference](//developer.android.com/reference/unity/namespace/Google/Play/Instant),
and
[Editor API reference](//developer.android.com/reference/unity/namespace/Google/Play/Instant/Editor)
for more information.

## Support plugins

These plugins provide shared functionality to some of the above plugins, but
they don't provide any features when installed separately.

### Play Common

*com.google.play.common*

The Google Play Common package provides common files required by some Google
Play packages, such as Play Instant.

### Play Core

*com.google.play.core*

The Google Play Core package provides the
[Play Core Library](//developer.android.com/guide/playcore) required by some
Google Play packages, such as Play Asset Delivery.

## Related plugins

### Google Play Games plugin for Unity

The
[Google Play Games plugin for Unity](//github.com/playgameservices/play-games-plugin-for-unity)
enables access to the
[Google Play Games APIs](//developers.google.com/games/services) from Unity.
