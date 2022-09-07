# Google Play Plugins for Unity

## Overview

The Google Play Plugins for Unity provide C# APIs for accessing various Play
services at runtime from within the Unity Engine. These plugins also provide
various Unity Editor features for building an app that can be published on
[Google Play Console](//play.google.com/console).

## Version support

These plugins support Unity 2018.4 or later, as well as Unity 2017.4.40.

Note: some features depending on async tasks are only available on Unity 2018.4
or later.

Support for older versions of Unity (including 5.6) was removed after
[version 1.4.0](//github.com/google/play-unity-plugins/releases/tag/v1.4.0)
because [Google Play requires that apps published with 32 bit native libraries
also provide 64 bit native
libaries.](//android-developers.googleblog.com/2019/01/get-your-apps-ready-for-64-bit.html)

## Downloading the plugins

There are 3 different options for obtaining the plugins:

*   Download individual plugins as `.unitypackage` files or Unity Package
    Manager (`.tgz`) files from
    [Google APIs for Unity](//developers.google.com/unity)

*   Download the latest release from this project's
    [releases page](//github.com/google/play-unity-plugins/releases)

*   `git clone` this repository into the **Assets** folder of your Unity project

## Installing the plugins

For all cases except `git clone` follow the instructions to
[Install Google packages for Unity](//developers.google.com/unity/instructions).

Developers using `git clone` must also install the [External Dependency Manager for Unity (EDM4U)](https://github.com/googlesamples/unity-jar-resolver) using either the .tgz or .unitypackage available on the [Google APIs for Unity archive page](https://developers.google.com/unity/archive#external_dependency_manager_for_unity).

If EDM4U is not installed, the project won't be able to fetch necessary Java dependencies such as the [Play Core library](//developer.android.com/guide/playcore), resulting in runtime errors.

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
[documentation](//developer.android.com/guide/playcore/in-app-updates/unity) and
[Runtime API reference](//developer.android.com/reference/unity/namespace/Google/Play/AppUpdate)
for more information.

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

### Play Integrity API

*com.google.play.integrity*

The Play Integrity API helps protect your apps and games from potentially risky
and fraudulent interactions, allowing you to respond with appropriate actions to
reduce attacks and abuse such as fraud, cheating, and unauthorized access.

Refer to the
[documentation](//developer.android.com/google/play/integrity/overview) and
[Runtime API reference](//developer.android.com/reference/unity/namespace/Google/Play/Integrity)
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
[Play Core Library](//developer.android.com/guide/playcore) required by some Google Play packages, such as Play Asset
Delivery.

### [Deprecated] Play Billing

*com.google.play.billing*

**The Google Play Billing Plugin for Unity will not be receiving updates going
forward. This means that it will stay on Play Billing Library 3 and app updates
past Nov 1, 2022 will no longer be able to use the plugin.**


Package contains Google Play Billing Library, which is required to sell digital
content and subscriptions in games distributed via Google Play. This new version
of the Google Play Billing Library provides all of the features available in the
current Java and Kotlin versions.

To use this package, you must agree to the licenses for this Google Play Plugin
and the Google Play Billing Library. If you do not agree with the licenses, you
may not use this package.

Refer to the [documentation](//developer.android.com/google/play/billing/unity)
and
[Runtime API reference](//developer.android.com/reference/unity/namespace/Google/Play/Billing)
for more information.

## Known Issues

<!----><a name="built-in-pad"></a>
### Play Asset Delivery support built into Unity

Recent versions of Unity, such as 2019.4.29, 2020.3.15, and 2021.1.15 (or later), include
[built-in support](https://docs.unity3d.com/Manual/play-asset-delivery.html)
for [Play Asset Delivery (PAD)](https://developer.android.com/guide/playcore/asset-delivery). These Unity versions allow
developers to specify asset packs by placing assets in .androidpack folders within their project. These versions also
change the "Split Application Binary" option to use asset packs instead of OBBs.

The build method used by the Google Play Plugins for Unity is incompatible with these features and will ignore assets placed
in the .androidpack folders.

<!----><a name="play-core-conflicts"></a>
### Play Core library conflicts

When building an Android App Bundle with Unity's build system (e.g. "File > Build and Run"), Unity may include the
[monolithic Play Core library](https://maven.google.com/web/index.html?q=core#com.google.android.play:core) in a way that
causes conflicts with the [new Play libraries](https://developer.android.com/reference/com/google/android/play/core/release-notes#partitioned-apis)
included by Google Play Plugins for Unity.

To resolve these conflicts, follow the steps below:

1. Enable "Custom Main Gradle Template" in "Android Player > Publishing Settings"
2. Enable "Patch mainTemplate.gradle" in "Assets > External Dependency Manager > Android Resolver > Settings"
3. Include [this](https://dl.google.com/games/registry/unity/com.google.play.core/playcore_empty_m2repo.zip) empty monolithic
Play Core library as a local maven repository

These steps will allow [EDM4U](https://github.com/googlesamples/unity-jar-resolver) to update the mainTemplate.gradle to
include the empty monolithic Play Core library as a gradle dependency. This will override the version of the Play Core library
included by Unity and resolve the duplicate class errors and manifest merger failures.

## Data Collection

The Google Play Plugins for Unity may collect version related data to allow Google to improve the product, including:

* App’s package name
* App’s package version
* Google Play Plugins for Unity's version

This data will be collected when you upload [your app package](https://developer.android.com/studio/publish/upload-bundle)
to the Play Console. To opt-out of this data collection process, remove the packagename.metadata.jar
file found under each plugin's `Runtime/Plugins` folder.

Note, this data collection related to your use of the Google Play Plugins for
Unity and Google’s use of the collected data is separate and independent of Google’s
collection of library dependencies declared in Gradle when you upload your app package
to the Play Console.

## Related plugins

### Google Play Games plugin for Unity

The
[Google Play Games plugin for Unity](//github.com/playgameservices/play-games-plugin-for-unity)
enables access to the
[Google Play Games APIs](//developers.google.com/games/services) from Unity.

### External Dependency Manager for Unity (EDM4U)

The [External Dependency Manager for Unity](https://github.com/googlesamples/unity-jar-resolver) provides tools for
handling dependencies such as Android libraries (AARs) and iOS CocoaPods. The Google Play Plugins for Unity depend on
EDM4U to resolve AAR dependencies such as the [Play Core library](//developer.android.com/guide/playcore).
