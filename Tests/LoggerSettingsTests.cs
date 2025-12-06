using System.Reflection;
using NUnit.Framework;

namespace DTech.Logging.Tests
{
    [TestFixture]
    internal sealed class LoggerSettingsTests
    {
        private LogSettingsInfo _backupSettings;

        [OneTimeSetUp]
        public void SetUp()
        {
            _backupSettings = GetBackupSettings();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            LoggerSettings settings = LoggerSettings.Instance;
            SetPrivateBoolField(settings, LogSettingsInfo.IsTraceEnabledFieldName, _backupSettings.IsTraceEnabled);
            SetPrivateBoolField(settings, LogSettingsInfo.IsDebugEnabledFieldName, _backupSettings.IsDebugEnabled);
            SetPrivateBoolField(settings, LogSettingsInfo.IsInformationEnabledFieldName, _backupSettings.IsInformationEnabled);
            SetPrivateBoolField(settings, LogSettingsInfo.IsWarningEnabledFieldName, _backupSettings.IsWarningEnabled);
            SetPrivateBoolField(settings, LogSettingsInfo.IsErrorEnabledFieldName, _backupSettings.IsErrorEnabled);
            SetPrivateBoolField(settings, LogSettingsInfo.IsCriticalEnabledFieldName, _backupSettings.IsCriticalEnabled);
            SetPrivateBoolField(settings, LogSettingsInfo.IsFileLoggingEnabledFieldName, _backupSettings.IsFileLoggingEnabled);
        }

        [Test]
        public void IsFileLoggingEnabled_ReturnsBackingField()
        {
            var settings = LoggerSettings.Instance;

            SetPrivateBoolField(settings, LogSettingsInfo.IsFileLoggingEnabledFieldName, true);
            Assert.IsTrue(LoggerSettings.Instance.IsFileLoggingEnabled);

            SetPrivateBoolField(settings, LogSettingsInfo.IsFileLoggingEnabledFieldName, false);
            Assert.IsFalse(LoggerSettings.Instance.IsFileLoggingEnabled);
        }

        [Test]
        public void IsEnabled_RespectsEachLogLevelFlag()
        {
            var settings = LoggerSettings.Instance;

            SetPrivateBoolField(settings, LogSettingsInfo.IsTraceEnabledFieldName, false);
            SetPrivateBoolField(settings, LogSettingsInfo.IsDebugEnabledFieldName, false);
            SetPrivateBoolField(settings, LogSettingsInfo.IsInformationEnabledFieldName, false);
            SetPrivateBoolField(settings, LogSettingsInfo.IsWarningEnabledFieldName, false);
            SetPrivateBoolField(settings, LogSettingsInfo.IsErrorEnabledFieldName, false);
            SetPrivateBoolField(settings, LogSettingsInfo.IsCriticalEnabledFieldName, false);

            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Trace));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Debug));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Information));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Warning));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Error));
            Assert.IsFalse(LoggerSettings.Instance.IsEnabled(LogLevel.Critical));

            SetPrivateBoolField(settings, LogSettingsInfo.IsTraceEnabledFieldName, true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Trace));

            SetPrivateBoolField(settings, LogSettingsInfo.IsDebugEnabledFieldName, true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Debug));

            SetPrivateBoolField(settings, LogSettingsInfo.IsInformationEnabledFieldName, true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Information));

            SetPrivateBoolField(settings, LogSettingsInfo.IsWarningEnabledFieldName, true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Warning));

            SetPrivateBoolField(settings, LogSettingsInfo.IsErrorEnabledFieldName, true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Error));

            SetPrivateBoolField(settings, LogSettingsInfo.IsCriticalEnabledFieldName, true);
            Assert.IsTrue(LoggerSettings.Instance.IsEnabled(LogLevel.Critical));
        }

        private static LogSettingsInfo GetBackupSettings()
        {
            var instance = LoggerSettings.Instance;
            return new LogSettingsInfo(instance);
        }

        private static void SetPrivateBoolField(object obj, string fieldName, bool value)
        {
            var field = obj.GetType()
                .GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(obj, value);
        }
        
        private readonly struct LogSettingsInfo
        {
            public const string IsTraceEnabledFieldName = "_isTraceEnabled";
            public const string IsDebugEnabledFieldName = "_isDebugEnabled";
            public const string IsInformationEnabledFieldName = "_isInformationEnabled";
            public const string IsWarningEnabledFieldName = "_isWarningEnabled";
            public const string IsErrorEnabledFieldName = "_isErrorEnabled";
            public const string IsCriticalEnabledFieldName = "_isCriticalEnabled";
            public const string IsFileLoggingEnabledFieldName = "_isFileLoggingEnabled";
            
            public bool IsTraceEnabled { get; }
            public bool IsDebugEnabled { get; }
            public bool IsInformationEnabled { get; }
            public bool IsWarningEnabled { get; }
            public bool IsErrorEnabled { get; }
            public bool IsCriticalEnabled { get; }
            public bool IsFileLoggingEnabled { get; }

            public LogSettingsInfo(LoggerSettings settings)
            {
                IsTraceEnabled = GetPrivateBool(settings, IsTraceEnabledFieldName);
                IsDebugEnabled = GetPrivateBool(settings, IsDebugEnabledFieldName);
                IsInformationEnabled = GetPrivateBool(settings, IsInformationEnabledFieldName);
                IsWarningEnabled = GetPrivateBool(settings, IsWarningEnabledFieldName);
                IsErrorEnabled = GetPrivateBool(settings, IsErrorEnabledFieldName);
                IsCriticalEnabled = GetPrivateBool(settings, IsCriticalEnabledFieldName);
                IsFileLoggingEnabled = GetPrivateBool(settings, IsFileLoggingEnabledFieldName);
            }

            private static bool GetPrivateBool(object obj, string fieldName)
            {
                FieldInfo field = typeof(LoggerSettings).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                return (bool)field.GetValue(obj);
            }
        }
    }
}