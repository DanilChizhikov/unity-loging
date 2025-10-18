using System;

namespace DTech.Logging
{
	[Obsolete("Use Logger<TCategoryName> instead.")]
	public sealed class LoggerT<TCategoryName> : ILoggerT<TCategoryName>
	{
		private readonly Logger _logger;

		public LoggerT()
		{
			_logger = new Logger(typeof(TCategoryName).Name);
		}
		
		public IDisposable BeginScope()
		{
			return BeginScope<TCategoryName>();
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
			Log<TCategoryName>(logLevel, exception, formatter);
		}

		public void LogInfo(string message, params object[] args)
		{
			_logger.LogInfo<TCategoryName>(message, args);
		}

		public void LogDebug(string message, params object[] args)
		{
			_logger.LogDebug<TCategoryName>(message, args);
		}

		public void LogWarning(string message, params object[] args)
		{
			_logger.LogWarning<TCategoryName>(message, args);
		}

		public void LogTrace(string message, params object[] args)
		{
			_logger.LogTrace<TCategoryName>(message, args);
		}

		public void LogError(string message, params object[] args)
		{
			_logger.LogError<TCategoryName>(message, args);
		}

		public void LogCritical(string message, params object[] args)
		{
			_logger.LogCritical<TCategoryName>(message, args);
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter)
		{
			_logger.Log<TState>(logLevel, exception, formatter);
		}
	}
}