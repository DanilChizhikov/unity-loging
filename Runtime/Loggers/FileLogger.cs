using System;
using System.IO;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class FileLogger : InternalLoggerBase
	{
		public override IDisposable BeginScope(string state)
		{
			return new Scope(state);
		}

		public override bool IsEnabled(LogLevel logLevel)
		{
			if (Application.isEditor)
			{
				return false;
			}
			
			#if DISABLE_FILE_LOGGING
			return false;
			#endif
			
			bool result = false;
			switch (logLevel)
			{
				case LogLevel.None:
				{
					result = false;
				} break;

				case LogLevel.Trace:
				case LogLevel.Debug:
				{
					#if DEVELOPMENT_BUILD || UNITY_EDITOR
					result = true;
					#endif
				} break;
				
				case LogLevel.Information:
				case LogLevel.Warning:
				case LogLevel.Error:
				case LogLevel.Critical:
				{
					result = true;
				} break;

				default:
					throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
			}
			
			return result;
		}

		public override void Log<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter)
		{
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
			
			stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}] [{typeof(TState).Name}] {formatter(exception)}");
		}
		
		private sealed class Scope : IDisposable
		{
			private readonly string _state;

			public Scope(string state)
			{
				if (string.IsNullOrEmpty(state))
				{
					throw new ArgumentNullException(nameof(state));
				}
				
				_state = state;
				using var stream = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true);
				stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][Scope Begin] {_state}");
			}
			
			public void Dispose()
			{
				using var stream = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true);
				stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][Scope End] {_state}");
			}
		}
	}
}