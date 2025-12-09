using System;
using System.IO;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class FileLogger : InternalLoggerBase
	{
		protected override LogLineBuilder LineBuilder { get; } =
			new(LoggerSettings.Instance.FileFormatString, LoggerSettings.Instance.PlacementReplacers);
		
		public FileLogger(string tag) : base(tag)
		{
		}

		public override bool IsEnabled(LogLevel logLevel)
		{
			return IsLogEnabled() && LoggerSettings.Instance.IsEnabled(logLevel);
		}

		protected override void SendLog<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter, string scopes)
		{
			if (!IsLogEnabled())
			{
				return;
			}
			
			string logBody = formatter(exception);
			string stateName = typeof(TState).Name;
			LineBuilder.Reset();
			LineBuilder.SetLogLevel(logLevel)
				.SetScopes(scopes)
				.SetTag(Tag)
				.SetStateName(stateName)
				.SetBody(logBody);
			
			using var stream = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true);
			stream.WriteLine(LineBuilder.ToString());
			LineBuilder.Reset();
		}

		private bool IsLogEnabled() => !Application.isEditor && LoggerSettings.Instance.IsFileLoggingEnabled;
	}
}