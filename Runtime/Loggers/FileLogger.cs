using System;
using System.IO;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class FileLogger : InternalLoggerBase
	{
		private static readonly LogLineBuilder _lineBuilder =
			new(LoggerSettings.Instance.FileFormatString, LoggerSettings.Instance.PlacementReplacers);
		
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
			string stateName = typeof(TState).Name;
			_lineBuilder.Reset();
			_lineBuilder.SetLogLevel(logLevel)
				.SetScopes(scopes)
				.SetTag(Tag)
				.SetStateName(stateName)
				.SetBody(logBody);
			
			using var stream = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true);
			stream.WriteLine(_lineBuilder.ToString());
			_lineBuilder.Reset();
		}

		private bool IsLogEnabled() => !Application.isEditor && LoggerSettings.Instance.IsFileLoggingEnabled;
	}
}
