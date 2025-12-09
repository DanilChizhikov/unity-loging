using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTech.Logging.Tests
{
    [TestFixture]
    internal sealed class LogFormatTests
    {
        private LogSettingsWrapper _settingsWrapper;
        
        [OneTimeSetUp]
        public void SetUp()
        {
            _settingsWrapper = new LogSettingsWrapper(LoggerSettings.Instance);
        }
        
        [TearDown]
        public void TearDown()
        {
            _settingsWrapper.ResetSettings();
        }

        [Test]
        public void Log_WithDefaultFormat_ContainsExpectedElements()
        {
            const string Message = "Test message 123";
            const string Expected = "[INFO][" + nameof(LogFormatTests) + "] " + Message;
            
            var logger = new Logger<LogFormatTests>();
            
            _settingsWrapper.ResetSettings()
                .OverrideFileLoggingEnabled(false);
            
            logger.LogInfo(Message);
            
            LogAssert.Expect(LogType.Log, Expected);
        }

        [Test]
        [TestCase("[LOG_LEVEL]", "Test message", "[INFO] Test message")]
        [TestCase("[LOG_TAG] ", "Test message", "[LogFormatTests] Test message")]
        [TestCase("[LOG_SCOPE] ", "Test message", "[[Scope > LogFormatTests > TestScope]] Test message")]
        [TestCase("[LOG_LEVEL] [LOG_TAG] ", "Test message", "[INFO] [LogFormatTests] Test message")]
        [TestCase("", "Test message", "Test message")]
        public void Log_WithCustomConsoleFormat_AppliesFormatCorrectly(string format, string message, string expectedStart = null)
        {
            _settingsWrapper.ResetSettings()
                .OverrideFileLoggingEnabled(false)
                .OverrideConsoleFormatString(format);
            
            var logger = new Logger<LogFormatTests>();

            IDisposable scope = null;
            if (format.Contains("LOG_SCOPE"))
            {
                scope = logger.BeginScope("TestScope");
            }
            
            logger.LogInfo(message);
            
            scope?.Dispose();
            
            LogAssert.Expect(LogType.Log, expectedStart);
        }

        [Test]
        public void Log_WithDifferentLogLevels_RespectsLogLevelFormatting()
        {
            const string ExpectedInfo = "[INFO] Info message";
            const string ExpectedWarning = "[WARN] Warning message";
            const string ExpectedError = "[ERROR] Error message";
            
            _settingsWrapper.ResetSettings()
                .OverrideFileLoggingEnabled(false)
                .OverrideInformationEnabled(true)
                .OverrideWarningEnabled(true)
                .OverrideErrorEnabled(true)
                .OverrideConsoleFormatString("[LOG_LEVEL]");
            var logger = new Logger<LogFormatTests>();
            
            
            logger.LogInfo("Info message");
            LogAssert.Expect(LogType.Log, ExpectedInfo);
            
            logger.LogWarning("Warning message");
            LogAssert.Expect(LogType.Warning, ExpectedWarning);
            
            logger.LogError("Error message");
            LogAssert.Expect(LogType.Error, ExpectedError);
        }

        [Test]
        public void Log_WithScopes_IncludesScopesInOutput()
        {
            const string Expected = "[Scope > LogFormatTests > TestScope] Scoped message";
            
            _settingsWrapper.ResetSettings()
                .OverrideFileLoggingEnabled(false)
                .OverrideConsoleFormatString("LOG_SCOPE");
            
            var logger = new Logger<LogFormatTests>();
            
            using (logger.BeginScope("TestScope"))
            {
                logger.LogInfo("Scoped message");
            }
            
            LogAssert.Expect(LogType.Log, Expected);
        }
    }
}