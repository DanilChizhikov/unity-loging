using System;
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
            
            LogAssert.Expect(LogType.Log, Expected);
            logger.LogInfo(Message);
        }

        [Test]
        [TestCase("[LOG_LEVEL]", "Test message", "[INFO] Test message")]
        [TestCase("[LOG_TAG] ", "Test message", "[LogFormatTests] Test message")]
        [TestCase("[LOG_SCOPE] ", "Test message", "[TestScope] Test message")]
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
            
            LogAssert.Expect(LogType.Log, expectedStart);
            
            logger.LogInfo(message);
            scope?.Dispose();
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
            
            
            LogAssert.Expect(LogType.Log, ExpectedInfo);
            logger.LogInfo("Info message");
            
            LogAssert.Expect(LogType.Warning, ExpectedWarning);
            logger.LogWarning("Warning message");
            
            LogAssert.Expect(LogType.Error, ExpectedError);
            logger.LogError("Error message");
        }

        [Test]
        public void Log_WithScopes_IncludesScopesInOutput()
        {
            const string Expected = "[TestScope] Scoped message";
            
            _settingsWrapper.ResetSettings()
                .OverrideFileLoggingEnabled(false)
                .OverrideConsoleFormatString("[LOG_SCOPE]");
            
            var logger = new Logger<LogFormatTests>();
            
            using (logger.BeginScope("TestScope"))
            {
                LogAssert.Expect(LogType.Log, Expected);
                logger.LogInfo("Scoped message");
            }
        }

		[Test]
		public void Log_WithDateTimePlacement_ReplacesWithCurrentYear()
		{
			_settingsWrapper.ResetSettings()
				.OverrideFileLoggingEnabled(false)
				.OverrideConsoleFormatString("[DATE_TIME:yyyy]");

			var logger = new Logger<LogFormatTests>();
			string expected = $"[{DateTime.Now:yyyy}] Date test";

			LogAssert.Expect(LogType.Log, expected);
			logger.LogInfo("Date test");
		}
    }
}
