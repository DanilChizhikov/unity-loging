# Changelog

## [1.0.0] - 2025-03-30

### Added
- Performance optimizations
  - Rewrote `LogLineBuilder` with template parsing and segment caching for faster log formatting
  - Added `LoggerUtility` class for centralized logger management
  - Added `DefaultLoggerProviderAttribute` for marking custom logger provider methods
  - Added `ArrayPool` usage for scope management to reduce allocations
  - Logger now accepts custom logger array via constructor: `Logger(string tag, params ILogger[] loggers)`
- Performance tests
  - `GenericLoggerPerformanceTests` - performance benchmarks for generic loggers
  - `LogLineBuilderPerformanceTests` - template parsing and formatting performance
  - `LoggerPerformanceTests` - general logger performance benchmarks
  - `LoggerStressTests` - high-load stress testing
  - `LoggerUtilityPerformanceTests` - utility method performance
  - `MemoryAllocationTests` - GC allocation testing
  - `ScopePerformanceTests` - scope creation/disposal benchmarks

### Changed
- **API Breaking Changes**
  - `ILogger.Log<TState>` signature changed: now accepts `string message, object[] args` instead of `Func<Exception, string> formatter`
  - Added new `Log<TState>` overload for plain messages without formatting arguments
  - `BeginScope<TState>()` documentation cleaned up
- **Internal Changes**
  - Removed individual placement replacer classes: `DateTimeLogPlacementReplacer`, `LogLevelPlacementReplacer`, `ScopesLogPlacementReplacer`, `StateLogPlacementReplacer`, `TagLogPlacementReplacer`
  - Built-in placements now handled internally by `LogLineBuilder` for better performance
  - Tests reorganized into `PlayMode` and `Performance` folders
  - `Logger` now uses `LoggerUtility.GetDefaultLoggers(tag)` instead of hardcoded array

### Fixed
- File logging disabled by default in Editor (performance improvement)

## [0.4.0] - 2025-12-10

## Removed
- Legacy code

## Added
- Placement replacers
  - `ILogPlacementReplacer` - interface for replace log placement from log format template
    - `DateTimeLogPlacementReplacer` - replace placement `DATE_TIME:date_time_format`
    - `LogLevelPlacementReplacer` - replace placement `LOG_LEVEL`
    - `ScopesLogPlacementReplacer` - replace placement `LOG_SCOPE`
    - `StateLogPlacementReplacer` - replace placement `LOG_STATE`
    - `TagLogPlacementReplacer` - replace placement `LOG_TAG`
    - `ScriptableLogPlacementReplacer` - base repalce class for custom replacers
- Inspector Drawer for `LoggerSettings`
- Editor Window for `LoggerSettings` you can find it in `Tools/DTech/Logger/Settings`
- Logger Settings
  - Added `Console Format String` property
  - Added `File Format String` property
  - Added `Placement Replacers` property

## [0.3.6] - 2025-12-06

### Fixed
- File logging
  - Fixed log writing to file on device
- Fixed creating `LoggerSettings`
- Fixed formatting log if use `LoggerExtensions`

### Added
- Added `LogConditions` for control log writing
- Added tests

## [0.3.5] - 2025-11-08

### Fixed
- File logging
  - Fixed editor log folder
  - Fixed log with json formatting

## [0.3.4] - 2025-10-30

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
