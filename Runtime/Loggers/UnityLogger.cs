using System;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class UnityLogger : InternalLoggerBase
	{
		private static readonly LogLineBuilder _lineBuilder =
			new(LoggerSettings.Instance.ConsoleFormatString, LoggerSettings.Instance.PlacementReplacers);
		
		public UnityLogger(string tag) : base(tag)
		{
		}

		public override bool IsEnabled(LogLevel logLevel)
		{
			#if DEVELOPMENT_BUILD || UNITY_EDITOR
			return logLevel != LogLevel.None;
			#endif

			return LoggerSettings.Instance.IsEnabled(logLevel);
		}

		protected override void SendLog<TState>(LogLevel logLevel, Exception exception, string message, object[] args, string scopes)
		{
			string logBody = FormatMessage(exception, message, args);
			string stateName = typeof(TState).Name;
			_lineBuilder.Reset();
			_lineBuilder.SetLogLevel(logLevel)
				.SetScopes(scopes)
				.SetTag(Tag)
				.SetStateName(stateName)
				.SetBody(logBody);

			string log = _lineBuilder.ToString();
			_lineBuilder.Reset();
			switch (logLevel)
			{
				case LogLevel.None:
					break;

				case LogLevel.Trace:
				case LogLevel.Debug:
				case LogLevel.Information:
				{
					Debug.Log(log);
				} break;

				case LogLevel.Warning:
				{
					Debug.LogWarning(log);
				} break;

				case LogLevel.Error:
				case LogLevel.Critical:
				{
					Debug.LogError(log);
				} break;

				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
		}
	}
}
