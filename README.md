# Logging
[![Unity Version](https://img.shields.io/badge/unity-2022.3+-000.svg)](https://unity3d.com/get-unity/download/archive)

## Table of Contents
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Manual Installation](#manual-installation)
    - [UPM Installation](#upm-installation)
- [Features](#features)
- [Usage](#usage)
    - [Basic Logging](#basic-logging)
    - [String Formatting](#string-formatting)
    - [Exception Handling](#exception-handling)
    - [Log Priority](#log-priority)
    - [Custom Log Tags](#custom-log-tags)
- [API Reference](#api-reference)
- [License](#license)

## Getting Started

### Prerequisites
- [GIT](https://git-scm.com/downloads)
- [Unity](https://unity.com/releases/editor/archive) 2022.3+

### Manual Installation
1. Download the .unitypackage from the [releases](https://github.com/DanilChizhikov/unity-loging/releases/) page.
2. Import com.dtech.logging.x.x.x.unitypackage into your project.

### UPM Installation
1. Open the manifest.json file in your project's Packages folder.
2. Add the following line to the dependencies section:
    ```json
    "com.dtech.logging": "https://github.com/DanilChizhikov/unity-loging.git",
    ```
3. Unity will automatically import the package.

If you want to set a target version, Logging uses the `v*.*.*` release tag so you can specify a version like #v0.2.0.

For example `https://github.com/DanilChizhikov/unity-loging.git#v0.1.0`.

## Features
- Multiple log levels (Info, Warning, Error, Exception)
- Color-coded log messages for better readability
- String formatting support
- Exception handling with stack traces
- Easy integration with Unity's console
- Write logs to file for editor mode
- Thread-safe logging
- Log priority system for filtering
- Custom log tags for better organization

## Usage

### Basic Logging
```csharp
using DTech.Logging;

public class Example : MonoBehaviour
{
    private ILogService _logService = new LogService();
    
    private ILogger _logger;
    
    private void Start()
    {
        _logger = _logService.GetLogger("Example");
        
        // Basic logging
        _logger.Info("This is an info message");
        _logger.Warning("This is a warning message");
        _logger.Error("This is an error message");
    }
}
```

### String Formatting
```csharp
// String formatting
_logger.InfoFormat("Player {0} has {1} points", playerName, score);
_logger.WarningFormat("Low health: {0}%", healthPercent);
_logger.ErrorFormat("Failed to load {0} at {1}", assetName, assetPath);
```

### Exception Handling
```csharp
try
{
    // Code that might throw an exception
}
catch (Exception ex)
{
    // Log exception with stack trace
    _logger.Exception(ex);
    
    // For critical exceptions that should stop execution
    // _logger.ExceptionFatal(ex);
}
```

### Log Priority

>!TIP
> Log priority is used to to determine which logs should not be merged into the release build.

- `Default` - Will not be merged into the release build
- `Critical` - Will be merged into the release build

```csharp
// Log with specific priority
_logger.Log("Important message", LogPriority.Default);
_logger.Log("Debug message", LogPriority.Critical);
```

### Custom Log Tags

```csharp
// Log with custom tag
_logger.Log("Important message", "CustomTag1", "CustomTag2);
```

## API Reference

- `void Info(object message, LogPriority priority, params string[] tags)` - Logs an informational message
- `void InfoFormat(string template, LogPriority priority, params object[] args)` - Logs a formatted info message
- `void Warning(object message, LogPriority priority, params string[] tags)` - Logs a warning message
- `void WarningFormat(string template, LogPriority priority, params object[] args)` - Logs a formatted warning message
- `void Error(object message, LogPriority priority, params string[] tags)` - Logs an error message
- `void ErrorFormat(string template, LogPriority priority, params object[] args)` - Logs a formatted error message
- `void Exception(Exception exception, LogPriority priority, params string[] tags)` - Logs an exception with stack trace
- `void ExceptionFatal(Exception exception, LogPriority priority, params string[] tags)` - Logs a fatal exception and stops execution

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.