using System;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class UnityLogger : InternalLoggerBase
	{
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

		protected override void SendLog<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter, string scopes)
		{
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
			
			string logBody = formatter(exception);
			string stateName = typeof(TState).Name;
			bool isNullState = stateName == NullStateName;
			string log;
			if (string.IsNullOrEmpty(Tag))
			{
				if (isNullState)
				{
					log = $"[{level}]{scopes} {logBody}";
				}
				else
				{
					log = $"[{level}]{scopes}[{stateName}] {logBody}";
				}
			}
			else
			{
				if (isNullState)
				{
					log = $"[{level}]{scopes}[{Tag}] {logBody}";
				}
				else
				{
					log = $"[{level}]{scopes}[{Tag}][{stateName}] {logBody}";
				}
			}
			
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