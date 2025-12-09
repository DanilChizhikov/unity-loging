using System;
using System.IO;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class FileLogger : InternalLoggerBase
	{
		protected override LogLineBuilder LineBuilder { get; } = new (LoggerSettings.Instance.FileFormatString);
		
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
			
			using var stream = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true);
			string level = logLevel.ToString().ToUpperInvariant();
			switch (logLevel)
			{
				case LogLevel.Information:
				case LogLevel.Warning:
				case LogLevel.Critical:
				{
					level = level.Substring(0, 4);
				} break;
			}

			string stateName = typeof(TState).Name;
			bool isNullState = stateName == NullStateName;
			if (string.IsNullOrEmpty(Tag))
			{
				if (isNullState)
				{
					stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}]{scopes} {formatter(exception)}");
				}
				else
				{
					stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}]{scopes}[{stateName}] {formatter(exception)}");	
				}
			}
			else
			{
				if (isNullState)
				{
					stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}]{scopes}[{Tag}] {formatter(exception)}");
				}
				else
				{
					stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}]{scopes}[{Tag}][{stateName}] {formatter(exception)}");
				}
			}
		}

		private bool IsLogEnabled() => !Application.isEditor && LoggerSettings.Instance.IsFileLoggingEnabled;
	}
}