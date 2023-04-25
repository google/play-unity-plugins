# Changelog for com.google.play.integrity

## [1.1.0] - 2023-04-05
### New Features
- Updated Java Play Integrity API dependency from 1.0.1 to 1.1.0-beta01
- Added support for Standard APIs (beta).
- Added new error code `ClientTransientError` in IntegrityErrorCode. This indicates that a transient error has occurred on the device. The caller should retry with [exponential backoff](https://developer.android.com/google/play/integrity/error-codes#retry-logic).

## [1.0.2] - 2022-09-08
- Updated plugin's data collection procedure. For more information and the opt-out
  process, please refer to the [data collection](https://github.com/google/play-unity-plugins#data-collection)
  section in README.

## [1.0.1] - 2022-07-06
### New Features
- Updated Java Play Integrity API dependency from 1.0.0 to 1.0.1

## [1.0.0] - 2022-02-15
### New Features
- Initial release
