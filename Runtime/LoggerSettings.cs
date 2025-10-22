using UnityEngine;

namespace DTech.Logging
{
	[CreateAssetMenu(fileName = nameof(LoggerSettings), menuName = "DTech/Logging/Logger Settings")]
	public sealed class LoggerSettings : ScriptableObject
	{
		private static readonly object _lockObject = new();
		
		public static LoggerSettings Instance
		{
			get
			{
				lock (_lockObject)
				{
					if (_instance == null)
					{
						_instance = Resources.Load<LoggerSettings>(nameof(LoggerSettings));
					}
				}
				
				return _instance;
			}
		}
		
		[Header("Log Release Settings")]
		[SerializeField] private bool _isTraceEnabled = false;
		[SerializeField] private bool _isDebugEnabled = false;
		[SerializeField] private bool _isInformationEnabled = true;
		[SerializeField] private bool _isWarningEnabled = true;
		[SerializeField] private bool _isErrorEnabled = true;
		[SerializeField] private bool _isCriticalEnabled = true;
		
		[Header("File Log Settings")]
		[SerializeField] private bool _isFileLoggingEnabled = true;
		
		private static LoggerSettings _instance;

		public bool IsEnabled(LogLevel logLevel) =>
			logLevel switch
			{
				LogLevel.Trace => _isTraceEnabled,
				LogLevel.Debug => _isDebugEnabled,
				LogLevel.Information => _isInformationEnabled,
				LogLevel.Warning => _isWarningEnabled,
				LogLevel.Error => _isErrorEnabled,
				LogLevel.Critical => _isCriticalEnabled,
				_ => false,
			};

		private void Reset()
		{
			_isFileLoggingEnabled = true;
			#if DISABLE_FILE_LOGGING
			_isFileLoggingEnabled = false;
			#endif
		}
	}
}