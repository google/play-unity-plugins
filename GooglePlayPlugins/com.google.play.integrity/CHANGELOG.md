# Changelog for com.google.play.integrity

## [1.4.0] - 2024-10-14
- Updated Java Play Integrity API dependency from 1.3.0 to 1.4.0.
- Add verdict opt-out to standard requests.
- Add [remediation dialogs](https://developer.android.com/google/play/integrity/remediation).

## [1.3.3] - 2024-09-18
- Updated Play Common dependency to 1.9.2.
- Updated Play Core dependency to 1.8.5.

## [1.3.2] - 2024-07-05
- Added new versioning metadata.
- Updated Play Common dependency to 1.9.1.
- Updated Play Core dependency to 1.8.4.

## [1.3.1] - 2024-01-12
- Updated Unity Play Core dependency from 1.8.1 to 1.8.3

## [1.3.0] - 2023-11-13
- Updated Java Play Integrity API dependency from 1.1.0-beta01 to 1.3.0
- Added new error codes `ClientTransientError` and `IntegrityTokenProviderInvalid` in StandardIntegrityErrorCode.
See [here](https://developer.android.com/google/play/integrity/error-codes) for more details.

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
