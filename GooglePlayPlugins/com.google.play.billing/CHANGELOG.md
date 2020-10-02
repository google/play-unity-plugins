# Changelog for com.google.play.billing

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

