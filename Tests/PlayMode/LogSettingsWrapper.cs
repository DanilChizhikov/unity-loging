using System.Reflection;

namespace DTech.Logging.Tests
{
	internal sealed class LogSettingsWrapper
	{
		private const string IsTraceEnabledFieldName = "_isTraceEnabled";
		private const string IsDebugEnabledFieldName = "_isDebugEnabled";
		private const string IsInformationEnabledFieldName = "_isInformationEnabled";
		private const string IsWarningEnabledFieldName = "_isWarningEnabled";
		private const string IsErrorEnabledFieldName = "_isErrorEnabled";
		private const string IsCriticalEnabledFieldName = "_isCriticalEnabled";
		private const string IsFileLoggingEnabledFieldName = "_isFileLoggingEnabled";
		private const string ConsoleFormatStringFieldName = "_consoleFormatString";
		private const string FileFormatStringFieldName = "_fileFormatString";

		private readonly LoggerSettings _settings;
		private readonly bool _isTraceEnabled;
		private readonly bool _isDebugEnabled;
		private readonly bool _isInformationEnabled;
		private readonly bool _isWarningEnabled;
		private readonly bool _isErrorEnabled;
		private readonly bool _isCriticalEnabled;
		private readonly bool _isFileLoggingEnabled;
		private readonly string _consoleFormatString;
		private readonly string _fileFormatString;

		public LogSettingsWrapper(LoggerSettings settings)
		{
			_settings = settings;
			_isTraceEnabled = GetPrivateBool(IsTraceEnabledFieldName);
			_isDebugEnabled = GetPrivateBool(IsDebugEnabledFieldName);
			_isInformationEnabled = GetPrivateBool(IsInformationEnabledFieldName);
			_isWarningEnabled = GetPrivateBool(IsWarningEnabledFieldName);
			_isErrorEnabled = GetPrivateBool(IsErrorEnabledFieldName);
			_isCriticalEnabled = GetPrivateBool(IsCriticalEnabledFieldName);
			_isFileLoggingEnabled = GetPrivateBool(IsFileLoggingEnabledFieldName);
			_consoleFormatString = GetPrivateString(ConsoleFormatStringFieldName);
			_fileFormatString = GetPrivateString(FileFormatStringFieldName);
		}

		public LogSettingsWrapper OverrideTraceEnabled(bool enabled)
		{
			SetPrivateBoolField(IsTraceEnabledFieldName, enabled);
			return this;
		}
		
		public LogSettingsWrapper OverrideDebugEnabled(bool enabled)
		{
			SetPrivateBoolField(IsDebugEnabledFieldName, enabled);
			return this;
		}
		
		public LogSettingsWrapper OverrideInformationEnabled(bool enabled)
		{
			SetPrivateBoolField(IsInformationEnabledFieldName, enabled);
			return this;
		}
		
		public LogSettingsWrapper OverrideWarningEnabled(bool enabled)
		{
			SetPrivateBoolField(IsWarningEnabledFieldName, enabled);
			return this;
		}
		
		public LogSettingsWrapper OverrideErrorEnabled(bool enabled)
		{
			SetPrivateBoolField(IsErrorEnabledFieldName, enabled);
			return this;
		}
		
		public LogSettingsWrapper OverrideCriticalEnabled(bool enabled)
		{
			SetPrivateBoolField(IsCriticalEnabledFieldName, enabled);
			return this;
		}
		
		public LogSettingsWrapper OverrideFileLoggingEnabled(bool enabled)
		{
			SetPrivateBoolField(IsFileLoggingEnabledFieldName, enabled);
			return this;
		}
		
		public LogSettingsWrapper OverrideConsoleFormatString(string formatString)
		{
			SetPrivateStringField(ConsoleFormatStringFieldName, formatString);
			return this;
		}

		public LogSettingsWrapper OverrideFileFormatString(string formatString)
		{
			SetPrivateStringField(FileFormatStringFieldName, formatString);
			return this;
		}

		public LogSettingsWrapper ResetSettings()
		{
			SetPrivateBoolField(IsTraceEnabledFieldName, _isTraceEnabled);
			SetPrivateBoolField(IsDebugEnabledFieldName, _isDebugEnabled);
			SetPrivateBoolField(IsInformationEnabledFieldName, _isInformationEnabled);
			SetPrivateBoolField(IsWarningEnabledFieldName, _isWarningEnabled);
			SetPrivateBoolField(IsErrorEnabledFieldName, _isErrorEnabled);
			SetPrivateBoolField(IsCriticalEnabledFieldName, _isCriticalEnabled);
			SetPrivateBoolField(IsFileLoggingEnabledFieldName, _isFileLoggingEnabled);
			SetPrivateStringField(ConsoleFormatStringFieldName, _consoleFormatString);
			SetPrivateStringField(FileFormatStringFieldName, _fileFormatString);
			return this;
		}

		private bool GetPrivateBool(string fieldName)
		{
			FieldInfo field = typeof(LoggerSettings).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			return (bool)field.GetValue(_settings);
		}
		
		private void SetPrivateBoolField(string fieldName, bool value)
		{
			var field = typeof(LoggerSettings).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			field?.SetValue(_settings, value);
		}
		
		private string GetPrivateString(string fieldName)
		{
			FieldInfo field = typeof(LoggerSettings).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			return (string)field.GetValue(_settings);
		}
		
		private void SetPrivateStringField(string fieldName, string value)
		{
			var field = typeof(LoggerSettings).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
			field?.SetValue(_settings, value);
		}
	}
}