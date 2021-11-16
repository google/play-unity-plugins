# Changelog for com.google.android.appbundle

## [1.6.0] - 2021-11-15
### New Features
 - Updated bundletool-all.jar from 1.6.1 to 1.8.2
### Bug Fixes
 - Fixed issue #127: crash with IL2CPP and "Separate base APK asset" enabled
 - Fixed issue #143: handle AssetBundle files that have file extensions
 - Fixed issue #145: AAB upload to Play Console fails due to BundleConfig.pb file size
 - Fixed issue when installing Android APIs using the plugin
### Other
 - Update minimum Target SDK version to 30

## [1.5.0] - 2021-06-14
### New Features
 - Added a new Bundletool method for building AABs (requires 2018.4+ and .NET 4+)
 - Added API and UI options to replace OBB files with an install-time asset pack
 - Added API options for customizing file compression within generated APKs
 - Changes to support minSdkVersion of 22 in Unity 2021.2
 - Updated bundletool-all.jar from 1.5.0 to 1.6.1
### Bug Fixes
 - Fixed issue #80: prevent building on Unity 2020.2 and early versions of 2020.3 and 2021.1
 - Fixed issue #106: bundletool build-bundle crashed when output file was specified without a parent directory
### Other
 - Removed ability to compile plugin with Unity 5.6, 2017.1, 2017.2, 2017.3, 2018.1, and 2018.2
 - Require Gradle for all Unity builds
 - Update minimum Target SDK version to 29

## [1.4.0] - 2021-03-08
### New Features
 - Updated bundletool-all.jar from 1.2.0 to 1.5.0
### Bug Fixes
 - Fixed issue #69: add a random name to temporary build path
 - Fixed issue #74: update bundletool-all.jar with fix
 - Fixed issue #82: pass "-Duser.language=en" to jarsigner

## [1.3.0] - 2020-09-30
### New Features
 - Updated bundletool-all.jar from 1.0.0 to 1.2.0
### Bug Fixes
 - Skip signing app bundles if no signing key is configured, instead of using a debug key (This reduces build time and provides a workaround if jarsigner is freezing)
 - Switch build-bundle to use new "--overwrite" flag (Fixes an issue on some systems where the build would fail unless an existing .aab was present)

## [1.2.0] - 2020-07-27
### New Features
 - Fixed issue #26: Add support for packaging raw assets targeted by texture format
 - Updated bundletool-all.jar from 0.14.0 to 1.0.0
### Bug Fixes
 - Support signing app bundles larger than 4GB by switching to jarsigner

## [1.1.1] - 2020-06-08
### Bug Fixes
 - Clear the EditorUserBuildSettings.exportAsGoogleAndroidProject flag for AAB builds and Build & Run

## [1.1.0] - 2020-05-04
### New Features
 - Updated bundletool-all.jar from 0.13.0 to 0.14.0
 - Updated documentation and menu item links
### Bug Fixes
 - Fixed issue #11: the package now compiles even if the Android platform isn't installed

## [1.0.0] - 2020-03-17
### New Features
 - Initial release
 - Includes bundletool-all.jar version 0.13.0

