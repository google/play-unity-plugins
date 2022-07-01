# Changelog for com.google.play.billing

## [3.2.4] - 2022-07-06
### New Features
- Incremented version number to match other packages

## [3.2.3] - 2022-02-15
### New Features
- Incremented version number to match other packages

## [3.2.2] - 2021-01-24
### New features
 - Incremented version number to match other packages

## [3.2.1] - 2021-11-15
### New features
 - incremented version number to match other packages
### Other
 - Updated README and announced deprecation of this plugin

## [3.2.0] - 2021-06-14
### Bug Fixes
 - Clarified documentation in GooglePlayStoreProrationMode
### Other
 - Removed ability to compile plugin with Unity 5.6, 2017.1, 2017.2, 2017.3, 2018.1, and 2018.2

## [3.1.2] - 2021-03-08
### New Features
 - Included Google Play Billing Libary [version 3.0.3](https://developer.android.com/google/play/billing/billing_library_releases_notes)
### Bug Fixes
 - Fixed issue #78: address conflicting class issue with Unity IAP 2.2+

## [3.1.0] - 2020-09-30
### New Features
 - Included Google Play Billing Libary [version 3.0.1](https://developer.android.com/google/play/billing/billing_library_releases_notes)
 - Added support for Unity IAP 2.1.0+ on Unity 2018.4+
 - Note: Removed Google.Play.Billing.asmdef for Unity 2018.3 and earlier, so there will no longer be a Google.Play.Billing.dll generated on these versions
### Bug Fixes
 - Fixed issue #33: remove "Deferred" mode in updateSubscription API as this mode is not compatible with Unity IAP

## [3.0.0] - 2020-06-08
### New Features
 - Included Google Play Billing Libary [version 3.0.0](https://developer.android.com/google/play/billing/billing_library_releases_notes)
 - Added a proguard config file so that developers don't have to create it themselves

## [2.2.1] - 2020-05-04
### New Features
 - Updated documentation and menu item links
### Bug Fixes
 - Fixed issue #12: the onPurchaseFailed callback now uses storeSpecificId instead of the internal Unity productId

## [2.2.0] - 2020-03-17
### New Features
 - Initial release
 - Includes GooglePlayBilling.aar version 2.2.0

