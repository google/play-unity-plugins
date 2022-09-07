# Changelog for com.google.play.assetdelivery

## [1.8.1] - 2022-09-08
- Updated plugin's data collection procedure. For more information and the opt-out
  process, please refer to the [data collection](https://github.com/google/play-unity-plugins#data-collection)
  section in README.

## [1.8.0] - 2022-07-06
### New Features
- Migrated to [Play Asset Delivery 2.0.0](https://developer.android.com/reference/com/google/android/play/core/release-notes-asset_delivery#2-0-0).

### Bug Fixes
- Fixed issue #170: sample files erroneously included in iOS and other non-android platforms

## [1.7.0] - 2022-02-15
### New Features
- Incremented version number to match other packages

## [1.6.1] - 2022-01-24
### New Features
 - Moved samples to a seperate folder

## [1.6.0] - 2021-11-15
### Other
 - Added partial support for newer versions of Unity with an alternative implementation of Play Asset Delivery

## [1.5.0] - 2021-06-14
### New Features
 - Added API and UI options to replace OBB files with an install-time asset pack
### Other
 - Removed ability to compile plugin with Unity 5.6, 2017.1, 2017.2, 2017.3, 2018.1, and 2018.2

## [1.4.0] - 2021-03-08
### Bug Fixes
 - Minor documentation updates

## [1.3.0] - 2020-09-30
### New Features
 - Updated Play Core library license to https://developer.android.com/guide/playcore/license

## [1.2.0] - 2020-07-27
### New Features
 - Removed experimental flags around new asset pack APIs added in 1.1.1
 - Updated new asset pack APIs to gracefully handle existing asset pack requests

## [1.1.1] - 2020-06-08
### New Features
 - Added experimental methods for retrieving asset packs that contain arbitrary files
 - Updated the Asset Delivery Demo app to utilize those experimental methods
 - Added menu options under "Google -> Android App Bundle -> Asset Delivery Demo" to facilitate building the sample
### Bug Fixes
 - Fixed issue where asset packs marked DoNotPackage were causing build to fail

## [1.1.0] - 2020-05-04
### New Features
 - Added Proguard config files
 - Updated documentation and menu item links

## [1.0.0] - 2020-03-17
### New Features
 - Initial release

