using System;
using System.Text;
using System.Threading;

namespace DTech.Logging
{
	public sealed class Logger : ILogger
	{
		private const string ScopesSeparator = " > ";
		private const string ScopePrefix = "Scope > ";

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
			LogScope current = _currentScope.Value;
			if (current == scope)
			{
				_currentScope.Value = SkipDisposed(scope.Parent);
				return;
			}

#if UNITY_EDITOR || DEVELOPMENT_BUILD
			if (current != null)
			{
				UnityEngine.Debug.LogWarning(
					$"[{nameof(Logger)}] LogScope disposed out of LIFO order (disposed='{scope.Name}', current='{current.Name}'). Use 'using' blocks to ensure correct nesting.");
			}
#endif

			_currentScope.Value = SkipDisposed(current);
		}

		private static LogScope SkipDisposed(LogScope start)
		{
			LogScope cursor = start;
			while (cursor != null && cursor.IsDisposed)
			{
				cursor = cursor.Parent;
			}

			return cursor;
		}

		private static string BuildEffectiveScopes(LogScope leaf)
		{
			LogScope live = SkipDisposed(leaf);
			if (live == null)
			{
				return string.Empty;
			}

			if (live == leaf && !HasDisposedAncestor(live))
			{
				return live.Scopes;
			}

			var sb = new StringBuilder(ScopePrefix.Length + live.Name.Length);
			AppendNames(sb, live);
			return sb.ToString();
		}

		private static bool HasDisposedAncestor(LogScope scope)
		{
			LogScope cursor = scope.Parent;
			while (cursor != null)
			{
				if (cursor.IsDisposed)
				{
					return true;
				}

				cursor = cursor.Parent;
			}

			return false;
		}

		private static void AppendNames(StringBuilder sb, LogScope scope)
		{
			LogScope parent = SkipDisposed(scope.Parent);
			if (parent == null)
			{
				sb.Append(ScopePrefix);
			}
			else
			{
				AppendNames(sb, parent);
				sb.Append(ScopesSeparator);
			}

			sb.Append(scope.Name);
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
			string scopes = BuildEffectiveScopes(_currentScope.Value);
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
			string scopes = BuildEffectiveScopes(_currentScope.Value);
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
