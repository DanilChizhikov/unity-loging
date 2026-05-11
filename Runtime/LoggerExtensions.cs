using System;

namespace DTech.Logging
{
	public static class LoggerExtensions
	{
		/// <summary>Formats the message and creates a scope.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to create the scope in.</param>
		/// <param name="messageFormat">
		///     Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c>
		/// </param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A disposable scope object. Can be null.</returns>
		/// <example>
		///     using(logger.BeginScope("Processing request from {Address}", address))
		///     {
		///     }
		/// </example>
		public static IDisposable BeginScope(this ILogger logger, string messageFormat, params object[] args)
		{
			var message = args.Length > 0 ? string.Format(messageFormat, args) : messageFormat;
			return logger.BeginScope(message);
		}

		/// <summary>Formats and writes a log message at the specified log level.</summary>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, Exception exception, string message, params object[] args)
		{
			if (!logger.IsEnabled(logLevel)) return;
			logger.Log<TState>(logLevel, exception, message, args);
		}

		/// <summary>Writes a log message at the specified log level without format arguments.</summary>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, Exception exception, string message)
		{
			if (!logger.IsEnabled(logLevel)) return;
			logger.Log<TState>(logLevel, exception, message);
		}

		/// <summary>Formats and writes a log message at the specified log level.</summary>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, string message, params object[] args)
		{
			if (!logger.IsEnabled(logLevel)) return;
			logger.Log<TState>(logLevel, null, message, args);
		}

		/// <summary>Writes a log message at the specified log level without format arguments.</summary>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, string message)
		{
			if (!logger.IsEnabled(logLevel)) return;
			logger.Log<TState>(logLevel, null, message);
		}

		/// <summary>Formats and writes a log message at the specified log level.</summary>
		public static void Log(this ILogger logger, LogLevel logLevel, string message, params object[] args)
		{
			if (!logger.IsEnabled(logLevel)) return;
			logger.Log<NullState>(logLevel, null, message, args);
		}

		/// <summary>Writes a log message at the specified log level without format arguments.</summary>
		public static void Log(this ILogger logger, LogLevel logLevel, string message)
		{
			if (!logger.IsEnabled(logLevel)) return;
			logger.Log<NullState>(logLevel, null, message);
		}

		/// <summary>Formats and writes a critical log message.</summary>
		public static void LogCritical<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Critical)) return;
			logger.Log<TState>(LogLevel.Critical, exception, message, args);
		}

		/// <summary>Writes a critical log message without format arguments.</summary>
		public static void LogCritical<TState>(this ILogger logger, Exception exception, string message)
		{
			if (!logger.IsEnabled(LogLevel.Critical)) return;
			logger.Log<TState>(LogLevel.Critical, exception, message);
		}

		/// <summary>Formats and writes a critical log message.</summary>
		public static void LogCritical<TState>(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Critical)) return;
			logger.Log<TState>(LogLevel.Critical, null, message, args);
		}

		/// <summary>Writes a critical log message without format arguments.</summary>
		public static void LogCritical<TState>(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Critical)) return;
			logger.Log<TState>(LogLevel.Critical, null, message);
		}

		/// <summary>Formats and writes a critical log message.</summary>
		public static void LogCritical(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Critical)) return;
			logger.Log<NullState>(LogLevel.Critical, null, message, args);
		}

		/// <summary>Writes a critical log message without format arguments.</summary>
		public static void LogCritical(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Critical)) return;
			logger.Log<NullState>(LogLevel.Critical, null, message);
		}

		/// <summary>Formats and writes a debug log message.</summary>
		public static void LogDebug<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Debug)) return;
			logger.Log<TState>(LogLevel.Debug, exception, message, args);
		}

		/// <summary>Writes a debug log message without format arguments.</summary>
		public static void LogDebug<TState>(this ILogger logger, Exception exception, string message)
		{
			if (!logger.IsEnabled(LogLevel.Debug)) return;
			logger.Log<TState>(LogLevel.Debug, exception, message);
		}

		/// <summary>Formats and writes a debug log message.</summary>
		public static void LogDebug<TState>(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Debug)) return;
			logger.Log<TState>(LogLevel.Debug, null, message, args);
		}

		/// <summary>Writes a debug log message without format arguments.</summary>
		public static void LogDebug<TState>(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Debug)) return;
			logger.Log<TState>(LogLevel.Debug, null, message);
		}

		/// <summary>Formats and writes a debug log message.</summary>
		public static void LogDebug(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Debug)) return;
			logger.Log<NullState>(LogLevel.Debug, null, message, args);
		}

		/// <summary>Writes a debug log message without format arguments.</summary>
		public static void LogDebug(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Debug)) return;
			logger.Log<NullState>(LogLevel.Debug, null, message);
		}

		/// <summary>Formats and writes an error log message.</summary>
		public static void LogError<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Error)) return;
			logger.Log<TState>(LogLevel.Error, exception, message, args);
		}

		/// <summary>Writes an error log message without format arguments.</summary>
		public static void LogError<TState>(this ILogger logger, Exception exception, string message)
		{
			if (!logger.IsEnabled(LogLevel.Error)) return;
			logger.Log<TState>(LogLevel.Error, exception, message);
		}

		/// <summary>Formats and writes an error log message.</summary>
		public static void LogError<TState>(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Error)) return;
			logger.Log<TState>(LogLevel.Error, null, message, args);
		}

		/// <summary>Writes an error log message without format arguments.</summary>
		public static void LogError<TState>(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Error)) return;
			logger.Log<TState>(LogLevel.Error, null, message);
		}

		/// <summary>Formats and writes an error log message.</summary>
		public static void LogError(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Error)) return;
			logger.Log<NullState>(LogLevel.Error, null, message, args);
		}

		/// <summary>Writes an error log message without format arguments.</summary>
		public static void LogError(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Error)) return;
			logger.Log<NullState>(LogLevel.Error, null, message);
		}

		/// <summary>Formats and writes an informational log message.</summary>
		public static void LogInfo<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Information)) return;
			logger.Log<TState>(LogLevel.Information, exception, message, args);
		}

		/// <summary>Writes an informational log message without format arguments.</summary>
		public static void LogInfo<TState>(this ILogger logger, Exception exception, string message)
		{
			if (!logger.IsEnabled(LogLevel.Information)) return;
			logger.Log<TState>(LogLevel.Information, exception, message);
		}

		/// <summary>Formats and writes an informational log message.</summary>
		public static void LogInfo<TState>(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Information)) return;
			logger.Log<TState>(LogLevel.Information, null, message, args);
		}

		/// <summary>Writes an informational log message without format arguments.</summary>
		public static void LogInfo<TState>(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Information)) return;
			logger.Log<TState>(LogLevel.Information, null, message);
		}

		/// <summary>Formats and writes an informational log message.</summary>
		public static void LogInfo(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Information)) return;
			logger.Log<NullState>(LogLevel.Information, null, message, args);
		}

		/// <summary>Writes an informational log message without format arguments.</summary>
		public static void LogInfo(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Information)) return;
			logger.Log<NullState>(LogLevel.Information, null, message);
		}

		/// <summary>Formats and writes a trace log message.</summary>
		public static void LogTrace<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Trace)) return;
			logger.Log<TState>(LogLevel.Trace, exception, message, args);
		}

		/// <summary>Writes a trace log message without format arguments.</summary>
		public static void LogTrace<TState>(this ILogger logger, Exception exception, string message)
		{
			if (!logger.IsEnabled(LogLevel.Trace)) return;
			logger.Log<TState>(LogLevel.Trace, exception, message);
		}

		/// <summary>Formats and writes a trace log message.</summary>
		public static void LogTrace<TState>(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Trace)) return;
			logger.Log<TState>(LogLevel.Trace, null, message, args);
		}

		/// <summary>Writes a trace log message without format arguments.</summary>
		public static void LogTrace<TState>(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Trace)) return;
			logger.Log<TState>(LogLevel.Trace, null, message);
		}

		/// <summary>Formats and writes a trace log message.</summary>
		public static void LogTrace(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Trace)) return;
			logger.Log<NullState>(LogLevel.Trace, null, message, args);
		}

		/// <summary>Writes a trace log message without format arguments.</summary>
		public static void LogTrace(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Trace)) return;
			logger.Log<NullState>(LogLevel.Trace, null, message);
		}

		/// <summary>Formats and writes a warning log message.</summary>
		public static void LogWarning<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Warning)) return;
			logger.Log<TState>(LogLevel.Warning, exception, message, args);
		}

		/// <summary>Writes a warning log message without format arguments.</summary>
		public static void LogWarning<TState>(this ILogger logger, Exception exception, string message)
		{
			if (!logger.IsEnabled(LogLevel.Warning)) return;
			logger.Log<TState>(LogLevel.Warning, exception, message);
		}

		/// <summary>Formats and writes a warning log message.</summary>
		public static void LogWarning<TState>(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Warning)) return;
			logger.Log<TState>(LogLevel.Warning, null, message, args);
		}

		/// <summary>Writes a warning log message without format arguments.</summary>
		public static void LogWarning<TState>(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Warning)) return;
			logger.Log<TState>(LogLevel.Warning, null, message);
		}

		/// <summary>Formats and writes a warning log message.</summary>
		public static void LogWarning(this ILogger logger, string message, params object[] args)
		{
			if (!logger.IsEnabled(LogLevel.Warning)) return;
			logger.Log<NullState>(LogLevel.Warning, null, message, args);
		}

		/// <summary>Writes a warning log message without format arguments.</summary>
		public static void LogWarning(this ILogger logger, string message)
		{
			if (!logger.IsEnabled(LogLevel.Warning)) return;
			logger.Log<NullState>(LogLevel.Warning, null, message);
		}
	}
}
