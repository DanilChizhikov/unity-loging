using System;
using System.IO;
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
		
		private bool IsLogEnabled() => !Application.isEditor && LoggerSettings.Instance.IsFileLoggingEnabled;

		private void SendLog<TState>(LogLevel logLevel, string message, string scopes)
		{
			LoggerSettings settings = LoggerSettings.Instance;
			LogLineBuilder lineBuilder = GetOrCreateLineBuilder(settings.FileFormatString, settings.PlacementReplacers);
			string stateName = typeof(TState).Name;
			string log = lineBuilder.Render(logLevel, scopes, Tag, stateName, message);

			using var stream = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true);
			stream.WriteLine(log);
		}
	}
}
