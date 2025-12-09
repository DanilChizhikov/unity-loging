using System;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class UnityLogger : InternalLoggerBase
	{
		protected override LogLineBuilder LineBuilder { get; } = new (LoggerSettings.Instance.ConsoleFormatString);
		
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
			string logBody = formatter(exception);
			string stateName = typeof(TState).Name;
			LineBuilder.Reset();
			LineBuilder.SetLogLevel(logLevel)
				.SetScopes(scopes)
				.SetTag(Tag)
				.SetStateName(stateName)
				.SetBody(logBody);

			string log = LineBuilder.ToString();
			LineBuilder.Reset();
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