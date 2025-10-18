using System;

namespace DTech.Logging
{
	public sealed class Logger : ILogger
	{
		private readonly InternalLoggerBase[] _loggers;

		public Logger(string tag)
		{
			_loggers = new InternalLoggerBase[]
			{
				new UnityLogger(tag),
				new FileLogger(tag)
			};
		}

		public IDisposable BeginScope<TState>()
		{
			return BeginScope(nameof(TState));
		}

		public IDisposable BeginScope(string state)
		{
			var scopes = new IDisposable[_loggers.Length];
			for (int i = 0; i < _loggers.Length; i++)
			{
				InternalLoggerBase logger = _loggers[i];
				scopes[i] = logger.BeginScope(state);
			}

			if (scopes.Length == 0)
			{
				return NullScope.Instance;
			}
			
			return new CompositeScope(scopes);
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			for (int i = 0; i < _loggers.Length; i++)
			{
				InternalLoggerBase logger = _loggers[i];
				if (logger.IsEnabled(logLevel))
				{
					return true;
				}
			}
			
			return false;
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter)
		{
			for (int i = 0; i < _loggers.Length; i++)
			{
				InternalLoggerBase logger = _loggers[i];
				if (logger.IsEnabled(logLevel))
				{
					logger.Log<TState>(logLevel, exception, formatter);
				}
			}
		}
	}
}