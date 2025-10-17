using System;

namespace DTech.Logging
{
	public sealed class LoggerT<T> : ILoggerT<T>
	{
		private readonly Logger _logger;

		public LoggerT()
		{
			_logger = new Logger();
		}
		
		public IDisposable BeginScope()
		{
			return BeginScope<T>();
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
		
		public void Log(LogLevel logLevel, Exception exception, Func<Exception, string> formatter)
		{
			Log<T>(logLevel, exception, formatter);
		}

		public void LogInfo(string message, params object[] args)
		{
			_logger.LogInfo<T>(message, args);
		}

		public void LogDebug(string message, params object[] args)
		{
			_logger.LogDebug<T>(message, args);
		}

		public void LogWarning(string message, params object[] args)
		{
			_logger.LogWarning<T>(message, args);
		}

		public void LogTrace(string message, params object[] args)
		{
			_logger.LogTrace<T>(message, args);
		}

		public void LogError(string message, params object[] args)
		{
			_logger.LogError<T>(message, args);
		}

		public void LogCritical(string message, params object[] args)
		{
			_logger.LogCritical<T>(message, args);
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter)
		{
			_logger.Log<TState>(logLevel, exception, formatter);
		}
	}
}