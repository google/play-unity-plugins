# Changelog for com.google.android.appbundle

## [1.4.1 - 2021-05-17
### New Features
 - Updated bundletool-all.jar from 1.5.0 to 1.6.1
### Bug Fixes
 - Fixed issue #106: bundletool build-bundle crashed when output file was specified without a parent directory

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

