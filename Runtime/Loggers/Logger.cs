using System;
using System.Buffers;

namespace DTech.Logging
{
	public sealed class Logger : ILogger
	{
		private readonly ILogger[] _loggers;

		public Logger(string tag)
		{
			_loggers = LoggerUtility.GetDefaultLoggers(tag);
		}

		public Logger(string tag, params ILogger[] loggers)
		{
			_loggers = loggers ?? LoggerUtility.GetDefaultLoggers(tag);
		}

		public IDisposable BeginScope<TState>()
		{
			return BeginScope(nameof(TState));
		}

		public IDisposable BeginScope(string state)
		{
			int loggerCount = _loggers.Length;
			if (loggerCount == 0)
			{
				return NullScope.Instance;
			}

			if (loggerCount == 1)
			{
				return _loggers[0].BeginScope(state);
			}

			IDisposable[] scopes = ArrayPool<IDisposable>.Shared.Rent(loggerCount);
			for (int i = 0; i < loggerCount; i++)
			{
				ILogger logger = _loggers[i];
				scopes[i] = logger.BeginScope(state);
			}

			return new CompositeScope(scopes, loggerCount, true);
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			for (int i = 0; i < _loggers.Length; i++)
			{
				ILogger logger = _loggers[i];
				if (logger.IsEnabled(logLevel))
				{
					return true;
				}
			}
			
			return false;
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, string message, object[] args)
		{
			for (int i = 0; i < _loggers.Length; i++)
			{
				ILogger logger = _loggers[i];
				if (logger.IsEnabled(logLevel))
				{
					logger.Log<TState>(logLevel, exception, message, args);
				}
			}
		}
	}

	public sealed class Logger<TCategoryName> : ILogger<TCategoryName>
	{
		private readonly ILogger _logger;
		
		public Logger()
		{
			_logger = new Logger(typeof(TCategoryName).Name);
		}

		public Logger(params ILogger[] loggers)
		{
			_logger = new Logger(typeof(TCategoryName).Name, loggers);
		}
		
		public IDisposable BeginScope<TState>()
		{
			return _logger.BeginScope<TState>();
		}

		public IDisposable BeginScope(string state)
		{
			return _logger.BeginScope(state);
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return _logger.IsEnabled(logLevel);
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, string message, object[] args)
		{
			_logger.Log<TState>(logLevel, exception, message, args);
		}
	}
}
