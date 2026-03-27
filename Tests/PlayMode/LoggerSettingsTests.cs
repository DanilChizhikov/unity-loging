using NUnit.Framework;

namespace DTech.Logging.Tests
{
    [TestFixture]
    internal sealed class LoggerSettingsTests
    {
        private LogSettingsWrapper _settingsWrapper;

        [OneTimeSetUp]
        public void SetUp()
        {
            _settingsWrapper = GetBackupSettings();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _settingsWrapper.ResetSettings();
        }

        [Test]
        public void IsFileLoggingEnabled_ReturnsBackingField()
        {
            _settingsWrapper.OverrideFileLoggingEnabled(true);
            Assert.IsTrue(LoggerSettings.Instance.IsFileLoggingEnabled);

            _settingsWrapper.OverrideFileLoggingEnabled(false);
            Assert.IsFalse(LoggerSettings.Instance.IsFileLoggingEnabled);
        }

        [Test]
        public void IsEnabled_RespectsEachLogLevelFlag()
        {
            _settingsWrapper.OverrideTraceEnabled(false)
                .OverrideDebugEnabled(false)
                .OverrideInformationEnabled(false)
                .OverrideWarningEnabled(false)
                .OverrideErrorEnabled(false)
                .OverrideCriticalEnabled(false);

            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Trace));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Debug));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Information));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Warning));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Error));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Critical));

            _settingsWrapper.OverrideTraceEnabled(true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Trace));

            _settingsWrapper.OverrideDebugEnabled(true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Debug));

            _settingsWrapper.OverrideInformationEnabled(true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Information));

            _settingsWrapper.OverrideWarningEnabled(true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Warning));

            _settingsWrapper.OverrideErrorEnabled(true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Error));

            _settingsWrapper.OverrideCriticalEnabled(true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Critical));
        }

        private static LogSettingsWrapper GetBackupSettings()
        {
            var instance = LoggerSettings.Instance;
            return new LogSettingsWrapper(instance);
        }
    }
}