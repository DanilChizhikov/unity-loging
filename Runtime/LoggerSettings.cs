using System;
using System.Collections.Generic;
using DTech.Logging.Placements;
using UnityEngine;

namespace DTech.Logging
{
	[CreateAssetMenu(fileName = nameof(LoggerSettings), menuName = "DTech/Logging/Logger Settings")]
	public sealed class LoggerSettings : ScriptableObject
	{
		private static volatile LoggerSettings _instance;
		private static volatile bool _loadAttempted;

		public static LoggerSettings Instance
		{
			get
			{
				LoggerSettings inst = _instance;
				if (inst != null)
				{
					return inst;
				}

				if (_loadAttempted)
				{
					return null;
				}

				inst = Resources.Load<LoggerSettings>(nameof(LoggerSettings));
				_instance = inst;
				_loadAttempted = true;

				if (inst == null)
				{
					Debug.LogError($"[{nameof(LoggerSettings)}] Asset '{nameof(LoggerSettings)}' was not found in any Resources folder. Logging will be disabled.");
				}

				return inst;
			}
		}
		
		[SerializeField] private bool _isTraceEnabled = false;
		[SerializeField] private bool _isDebugEnabled = false;
		[SerializeField] private bool _isInformationEnabled = true;
		[SerializeField] private bool _isWarningEnabled = true;
		[SerializeField] private bool _isErrorEnabled = true;
		[SerializeField] private bool _isCriticalEnabled = true;
		
		[SerializeField] private bool _isFileLoggingEnabled = true;
		
		[SerializeField] private string _consoleFormatString = "[LOG_LEVEL][LOG_SCOPE][LOG_TAG][LOG_STATE]";
		[SerializeField] private string _fileFormatString = "[DATE_TIME:HH:mm:ss.fff][LOG_LEVEL][LOG_SCOPE][LOG_TAG][LOG_STATE]";
		[SerializeField] private ScriptableLogPlacementReplacer[] _placementReplacers = Array.Empty<ScriptableLogPlacementReplacer>();

		public bool IsFileLoggingEnabled => LogConditions.IsFileLoggingEnabled && _isFileLoggingEnabled;
		public string ConsoleFormatString => _consoleFormatString;
		public string FileFormatString => _fileFormatString;
		public IReadOnlyList<ILogPlacementReplacer> PlacementReplacers => _placementReplacers;

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
	}
}