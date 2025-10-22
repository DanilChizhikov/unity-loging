# Logging
[![Unity Version](https://img.shields.io/badge/unity-2022.3+-000.svg)](https://unity3d.com/get-unity/download/archive)

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

If you want to set a target version, Logging uses the `v*.*.*` release tag so you can specify a version like #v0.3.2.

For example `https://github.com/DanilChizhikov/unity-loging.git#v0.3.2`.

## Features
- Multiple log levels (Trace, Debug, Information, Warning, Error, Critical)
- Structured logging with named parameters
- Scoped logging for grouping related operations
- Exception logging with stack traces
- Thread-safe implementation
- Extensible logging pipeline
- Compatible with Microsoft.Extensions.Logging patterns

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

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.