# Logging
[![Unity Version](https://img.shields.io/badge/unity-2022.3+-000.svg)](https://unity3d.com/get-unity/download/archive)
![Unity Tests](https://github.com/DanilChizhikov/unity-loging/actions/workflows/tests.yml/badge.svg?branch=master)

## Overview
The package provides a simple and flexible system for logging in Unity. It allows you to control the level of logging, 
log messages with different levels, and structure log messages with named parameters.

## Table of Contents
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Manual Installation](#manual-installation)
    - [UPM Installation](#upm-installation)
- [Features](#features)
- [Settings](#settings)
- [Usage](#usage)
    - [Basic Logging](#basic-logging)
    - [Log Levels](#log-levels)
    - [Scopes](#scopes)
    - [Placements](#placements)
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

If you want to set a target version, Logging uses the `v*.*.*` release tag so you can specify a version like #v0.4.0.

For example `https://github.com/DanilChizhikov/unity-loging.git#v0.4.0`.

## Features
- Multiple log levels (Trace, Debug, Information, Warning, Error, Critical)
- Structured logging with named parameters
- Scoped logging for grouping related operations
- Exception logging with stack traces
- Thread-safe implementation
- Extensible logging pipeline
- Compatible with Microsoft.Extensions.Logging patterns
- Log format template

## Settings
For control log on release builds, you can use the `LoggerSettings`.
It is `ScriptableObject` and can be found in `Assets/Resources/LoggerSettings.asset`.
> [!NOTE]
> `LoggerSettings` is created automatically on first launch.
> Or you can create it manually `DTech/Logging/Logger Settings`.

## Usage

### Basic Logging
```csharp
using DTech.Logging;

public class Example : MonoBehaviour
{
    private ILogger _logger;
    
    private void Start()
    {
        _logger = new Logger(nameof(Example));
        
        // Basic logging
        _logger.LogInfo("This is an info message");
        _logger.LogWarning("This is a warning message");
        _logger.LogError("This is an error message");
    }
}
```

```csharp
using DTech.Logging;

public class Example : MonoBehaviour
{
    private ILogger<Example> _logger;
    
    private void Start()
    {
        _logger = new Logger<Example>();
        
        // Basic logging
        _logger.LogInfo("This is an info message");
        _logger.LogWarning("This is a warning message");
        _logger.LogError("This is an error message");
    }
}
```

### Log Levels
```csharp
// Different log levels
_logger.LogTrace("Detailed debug information");
_logger.LogDebug("Debug information");
_logger.LogInfo("Informational message");
_logger.LogWarning("Warning message");
_logger.LogError("Error message");
_logger.LogCritical("Critical error - application may terminate");
```

### Scopes
```csharp
// Using scopes for grouping related operations
using (_logger.BeginScope("OperationScope"))
{
    _logger.LogInfo("Starting operation");
    
    // Perform some operations
    
    _logger.LogInfo("Operation completed");
}
```

### Placements

Logger allows you to format log messages with placeholders.
These placeholders are replaced by values when the log message is written.

Supported placeholders:

- `LOG_TAG` - The log tag.
- `LOG_LEVEL` - The log level.
- `LOG_SCOPE` - The current scope.
- `LOG_STATE` - The current state.
- `DATE_TIME:date_time_format` - The current date and time. Replaces `date_time_format` with the desired format.

For example, if you want log messages to be in the format:

```csharp
[DATE_TIME:HH:mm:ss.fff][LOG_LEVEL][LOG_SCOPE][LOG_TAG][LOG_STATE] Your Message
```

You can set the `Console Format String` property to `[LOG_LEVEL][LOG_SCOPE][LOG_TAG][LOG_STATE]`.

#### Custom Replacers

To add your own replacers, you can create a new class that inherits from `ScriptableLogPlacementReplacer` and override the `Replace` method.
Then, you need to add an instance of your class to the `PlacementReplacers` property of the `LoggerSettings` object.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.