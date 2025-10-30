# Changelog

## [0.4.0] - 2025-10-30

### Fixed
- File logging
  - Fixed `IOException` from `LoggerFileProvider`

### Changed
- File logging
  - Added check `LoggerSettings.IsFileLoggingEnabled` before file logging

## [0.3.3] - 2025-10-28

### Added
- Log Editor Utilities
  - ``Tools/DTech/Logger/Editor Log Writing Enable`` - switch editor log writing enable

## [0.3.2] - 2025-10-22

### Added
- Log Editor Utilities
  - ``Tools/DTech/Logger/Open Logs Folder``
  - ``Tools/DTech/Logger/Remove All Logs``
- ``LoggerSettings`` - settings for control log on release builds

### Changed
- **Logging**
  - Updated scope logging

### Fixed
- **Logging**
  - Fixed scope logging on ``async`` methods

## [0.3.0] - 2025-10-18

### Changed
- **Logging**
    - Updated log system
    - Mark ``ILoggerT`` and ``LoggerT`` as ``Obsolete``
    - Update Internal Log Scopes

### Added
- **Logging**
    - Added ``ILogger<TCategoryName>`` and ``Logger<TCategoryName>``
    - Added new Extension methods for ``ILogger``

## [0.3.0] - 2025-10-17

### Changed
- **Logging**
  - Refactored log system

### Added
- **Logging**
  - LogLevel
  - Internal loggers
    - UnityLogger
    - FileLogger
  - Generic loggers ``ILoggerT<>`` and ``LoggerT<>``

## [0.2.0] - 2025-10-14

### Added
- **Logging**
  - Added log priority
  - Added custom log tags

### Fixed
- **Log Writing**
  - Removed log color tags then log writing to file
  - Fixed missed logs how writing to file on launch play mode

## [0.1.0] - 2025-10-12

### Added
- **Logging**
  - Added logging service with support for different log levels
  - Added support for logging to file in editor mode
