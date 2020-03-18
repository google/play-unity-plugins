# Google Play Plugins for Unity

## Overview

The Google Play Plugins for Unity provide C# APIs for accessing various Play
services at runtime from within the Unity Engine. These plugins also provide
various Unity Editor features for building an app that can be published on
[Google Play Console](https://play.google.com/apps/publish).

## Core Plugins

### Android App Bundle

*com.google.android.appbundle*

The Google Android App Bundle package provides access to the latest Android App
Bundles features, such as Play Asset Delivery.

### Play Asset Delivery

*com.google.play.assetdelivery*

Play Asset Delivery enables AssetBundles and other assets to be packaged into an
Android App Bundle and delivered through Google Play.

### Play Billing

*com.google.play.billing*

Package contains Google Play Billing Library, which is required to sell digital
content and subscriptions in games distributed via Google Play. This new version
of the Google Play Billing Library provides all of the features available in the
current Java and Kotlin versions.

To use this package, you must agree to the licenses for this Google Play Plugin
and the Google Play Billing Library. If you do not agree with the licenses, you
may not use this package.

### Play Instant

*com.google.play.instant*

The Google Play Instant package simplifies the conversion of an Android app into
an instant app that can be deployed through Google Play.

## Support Plugins

These plugins provide shared functionality to some of the above plugins, but
they don't provide any features when installed separately.

### Play Common

*com.google.play.common*

The Google Play Common package provides common files required by some Google
Play packages, such as Play Instant.

### Play Core

*com.google.play.core*

The Google Play Core package provides the Play Core Library required by some
Google Play packages, such as Play Asset Delivery.

## Related Plugins

### Google Play Games plugin for Unity

The
[Google Play Games plugin for Unity](https://github.com/playgameservices/play-games-plugin-for-unity)
enables access to the Google Play Games API from Unity.
