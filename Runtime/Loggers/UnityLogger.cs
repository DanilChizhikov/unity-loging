using System;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class UnityLogger : InternalLoggerBase
	{
		public UnityLogger(string tag) : base(tag)
		{
		}

		public override IDisposable BeginScope(string state)
		{
			return new Scope(state);
		}

		public override bool IsEnabled(LogLevel logLevel)
		{
			bool result = false;
			switch (logLevel)
			{
				case LogLevel.None:
				{
					result = false;
				} break;

				case LogLevel.Trace:
				case LogLevel.Debug:
				case LogLevel.Information:
				case LogLevel.Warning:
				case LogLevel.Error:
				{
					#if DEVELOPMENT_BUILD || UNITY_EDITOR
					result = true;
					#endif
				} break;
				
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
			string log = string.IsNullOrEmpty(Tag)
				? $"[{level}][{typeof(TState).Name}] {logBody}"
				: $"[{level}] [{Tag}] [{typeof(TState).Name}] {logBody}";
			
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
				Debug.Log($"[Scope Begin] {state}");
			}
			
			public void Dispose()
			{
				Debug.Log($"[Scope End] {_state}");
			}
		}
	}
}