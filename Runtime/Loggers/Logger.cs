using System;
using System.Threading;

namespace DTech.Logging
{
	public sealed class Logger : ILogger
	{
		private readonly ILogger[] _loggers;
		private readonly AsyncLocal<LogScope> _currentScope;

		public Logger(string tag)
		{
			_loggers = LoggerUtility.GetDefaultLoggers(tag);
			_currentScope = new AsyncLocal<LogScope>();
		}

		public Logger(string tag, params ILogger[] loggers)
		{
			_loggers = loggers ?? LoggerUtility.GetDefaultLoggers(tag);
			_currentScope = new AsyncLocal<LogScope>();
		}

		public IDisposable BeginScope<TState>()
		{
			return BeginScope(TypeNameCache<TState>.Name);
		}

		public IDisposable BeginScope(string state)
		{
			var newScope = new LogScope(state, _currentScope.Value, this);
			_currentScope.Value = newScope;
			return newScope;
		}

		internal void OnScopeDisposed(LogScope scope)
		{
			if (_currentScope.Value == scope)
			{
				_currentScope.Value = scope.Parent;
			}
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			for (int i = 0; i < _loggers.Length; i++)
			{
				if (_loggers[i].IsEnabled(logLevel))
				{
					return true;
				}
			}

			return false;
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, string message, object[] args)
		{
			string scopes = _currentScope.Value?.Scopes ?? string.Empty;
			for (int i = 0; i < _loggers.Length; i++)
			{
				ILogger logger = _loggers[i];
				if (!logger.IsEnabled(logLevel))
				{
					continue;
				}

				if (logger is InternalLoggerBase internalLogger)
				{
					internalLogger.Log<TState>(logLevel, exception, message, args, scopes);
				}
				else
				{
					logger.Log<TState>(logLevel, exception, message, args);
				}
			}
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, string message)
		{
			string scopes = _currentScope.Value?.Scopes ?? string.Empty;
			for (int i = 0; i < _loggers.Length; i++)
			{
				ILogger logger = _loggers[i];
				if (!logger.IsEnabled(logLevel))
				{
					continue;
				}

				if (logger is InternalLoggerBase internalLogger)
				{
					internalLogger.Log<TState>(logLevel, exception, message, scopes);
				}
				else
				{
					logger.Log<TState>(logLevel, exception, message);
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

		public void Log<TState>(LogLevel logLevel, Exception exception, string message)
		{
			_logger.Log<TState>(logLevel, exception, message);
		}
	}
}
