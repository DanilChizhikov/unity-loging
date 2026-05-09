using System;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class FileLogger : InternalLoggerBase
	{
		public FileLogger(string tag) : base(tag)
		{
		}

		public override bool IsEnabled(LogLevel logLevel)
		{
			return IsLogEnabled() && LoggerSettings.Instance.IsEnabled(logLevel);
		}

		protected override void SendLog<TState>(LogLevel logLevel, Exception exception, string message, object[] args, string scopes)
		{
			if (!IsLogEnabled())
			{
				return;
			}

			string logBody = FormatMessage(exception, message, args);
			SendLog<TState>(logLevel, logBody, scopes);
		}

		protected override void SendLog<TState>(LogLevel logLevel, Exception exception, string message, string scopes)
		{
			if (!IsLogEnabled())
			{
				return;
			}

			string logBody = FormatMessage(exception, message);
			SendLog<TState>(logLevel, logBody, scopes);
		}

		private bool IsLogEnabled()
		{
#if UNITY_EDITOR
			return false;
#else
			return LoggerSettings.Instance.IsFileLoggingEnabled;
#endif
		}

		private void SendLog<TState>(LogLevel logLevel, string message, string scopes)
		{
			LoggerSettings settings = LoggerSettings.Instance;
			LogLineBuilder lineBuilder = GetOrCreateLineBuilder(settings.FileFormatString, settings.PlacementReplacers);
			string stateName = StateName<TState>.Value;
			string log = lineBuilder.Render(logLevel, scopes, Tag, stateName, message);

			BackgroundFileLogSink.Instance.Enqueue(log);
		}
	}
}
