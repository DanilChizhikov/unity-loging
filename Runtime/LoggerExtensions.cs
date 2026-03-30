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
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Format string of the log message.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, Exception exception, string message, params object[] args)
		{
			logger.Log<TState>(logLevel, exception, message, args);
		}

		/// <summary>Writes a log message at the specified log level without format arguments.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Plain log message.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, Exception exception, string message)
		{
			logger.Log<TState>(logLevel, exception, message);
		}

		/// <summary>Formats and writes a log message at the specified log level.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="message">Format string of the log message.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, string message, params object[] args)
		{
			logger.Log<TState>(logLevel, null, message, args);
		}

		/// <summary>Writes a log message at the specified log level without format arguments.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="message">Plain log message.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, string message)
		{
			logger.Log<TState>(logLevel, null, message);
		}

		/// <summary>Formats and writes a log message at the specified log level.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="message">Format string of the log message.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		public static void Log(this ILogger logger, LogLevel logLevel, string message, params object[] args)
		{
			logger.Log<NullState>(logLevel, null, message, args);
		}

		/// <summary>Writes a log message at the specified log level without format arguments.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="message">Plain log message.</param>
		public static void Log(this ILogger logger, LogLevel logLevel, string message)
		{
			logger.Log<NullState>(logLevel, null, message);
		}

		/// <summary>Formats and writes a critical log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogCritical(exception, "Error while processing request from {Address}", address)</example>
		public static void LogCritical<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Critical, exception, message, args);
		}

		/// <summary>Writes a critical log message without format arguments.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Plain log message.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		public static void LogCritical<TState>(this ILogger logger, Exception exception, string message)
		{
			logger.Log<TState>(LogLevel.Critical, exception, message);
		}

		/// <summary>Formats and writes a critical log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogCritical("Processing request from {Address}", address)</example>
		public static void LogCritical<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Critical, message, args);
		}

		/// <summary>Writes a critical log message without format arguments.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Plain log message.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		public static void LogCritical<TState>(this ILogger logger, string message)
		{
			logger.Log<TState>(LogLevel.Critical, null, message);
		}

		/// <summary>Formats and writes a critical log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <example>logger.LogCritical("Processing request from {Address}", address)</example>
		public static void LogCritical(this ILogger logger, string message, params object[] args)
		{
			logger.LogCritical<NullState>(message, args);
		}

		/// <summary>Writes a critical log message without format arguments.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Plain log message.</param>
		public static void LogCritical(this ILogger logger, string message)
		{
			logger.LogCritical<NullState>(message);
		}

		/// <summary>Formats and writes a debug log message.</summary>
		public static void LogDebug<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Debug, exception, message, args);
		}

		/// <summary>Writes a debug log message without format arguments.</summary>
		public static void LogDebug<TState>(this ILogger logger, Exception exception, string message)
		{
			logger.Log<TState>(LogLevel.Debug, exception, message);
		}

		/// <summary>Formats and writes a debug log message.</summary>
		public static void LogDebug<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Debug, message, args);
		}

		/// <summary>Writes a debug log message without format arguments.</summary>
		public static void LogDebug<TState>(this ILogger logger, string message)
		{
			logger.Log<TState>(LogLevel.Debug, null, message);
		}

		/// <summary>Formats and writes a debug log message.</summary>
		public static void LogDebug(this ILogger logger, string message, params object[] args)
		{
			logger.LogDebug<NullState>(message, args);
		}

		/// <summary>Writes a debug log message without format arguments.</summary>
		public static void LogDebug(this ILogger logger, string message)
		{
			logger.LogDebug<NullState>(message);
		}

		/// <summary>Formats and writes an error log message.</summary>
		public static void LogError<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Error, exception, message, args);
		}

		/// <summary>Writes an error log message without format arguments.</summary>
		public static void LogError<TState>(this ILogger logger, Exception exception, string message)
		{
			logger.Log<TState>(LogLevel.Error, exception, message);
		}

		/// <summary>Formats and writes an error log message.</summary>
		public static void LogError<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Error, message, args);
		}

		/// <summary>Writes an error log message without format arguments.</summary>
		public static void LogError<TState>(this ILogger logger, string message)
		{
			logger.Log<TState>(LogLevel.Error, null, message);
		}

		/// <summary>Formats and writes an error log message.</summary>
		public static void LogError(this ILogger logger, string message, params object[] args)
		{
			logger.LogError<NullState>(message, args);
		}

		/// <summary>Writes an error log message without format arguments.</summary>
		public static void LogError(this ILogger logger, string message)
		{
			logger.LogError<NullState>(message);
		}

		/// <summary>Formats and writes an informational log message.</summary>
		public static void LogInfo<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Information, exception, message, args);
		}

		/// <summary>Writes an informational log message without format arguments.</summary>
		public static void LogInfo<TState>(this ILogger logger, Exception exception, string message)
		{
			logger.Log<TState>(LogLevel.Information, exception, message);
		}

		/// <summary>Formats and writes an informational log message.</summary>
		public static void LogInfo<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Information, message, args);
		}

		/// <summary>Writes an informational log message without format arguments.</summary>
		public static void LogInfo<TState>(this ILogger logger, string message)
		{
			logger.Log<TState>(LogLevel.Information, null, message);
		}

		/// <summary>Formats and writes an informational log message.</summary>
		public static void LogInfo(this ILogger logger, string message, params object[] args)
		{
			logger.LogInfo<NullState>(message, args);
		}

		/// <summary>Writes an informational log message without format arguments.</summary>
		public static void LogInfo(this ILogger logger, string message)
		{
			logger.LogInfo<NullState>(message);
		}

		/// <summary>Formats and writes a trace log message.</summary>
		public static void LogTrace<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Trace, exception, message, args);
		}

		/// <summary>Writes a trace log message without format arguments.</summary>
		public static void LogTrace<TState>(this ILogger logger, Exception exception, string message)
		{
			logger.Log<TState>(LogLevel.Trace, exception, message);
		}

		/// <summary>Formats and writes a trace log message.</summary>
		public static void LogTrace<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Trace, message, args);
		}

		/// <summary>Writes a trace log message without format arguments.</summary>
		public static void LogTrace<TState>(this ILogger logger, string message)
		{
			logger.Log<TState>(LogLevel.Trace, null, message);
		}

		/// <summary>Formats and writes a trace log message.</summary>
		public static void LogTrace(this ILogger logger, string message, params object[] args)
		{
			logger.LogTrace<NullState>(message, args);
		}

		/// <summary>Writes a trace log message without format arguments.</summary>
		public static void LogTrace(this ILogger logger, string message)
		{
			logger.LogTrace<NullState>(message);
		}

		/// <summary>Formats and writes a warning log message.</summary>
		public static void LogWarning<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Warning, exception, message, args);
		}

		/// <summary>Writes a warning log message without format arguments.</summary>
		public static void LogWarning<TState>(this ILogger logger, Exception exception, string message)
		{
			logger.Log<TState>(LogLevel.Warning, exception, message);
		}

		/// <summary>Formats and writes a warning log message.</summary>
		public static void LogWarning<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.Log<TState>(LogLevel.Warning, message, args);
		}

		/// <summary>Writes a warning log message without format arguments.</summary>
		public static void LogWarning<TState>(this ILogger logger, string message)
		{
			logger.Log<TState>(LogLevel.Warning, null, message);
		}

		/// <summary>Formats and writes a warning log message.</summary>
		public static void LogWarning(this ILogger logger, string message, params object[] args)
		{
			logger.LogWarning<NullState>(message, args);
		}

		/// <summary>Writes a warning log message without format arguments.</summary>
		public static void LogWarning(this ILogger logger, string message)
		{
			logger.LogWarning<NullState>(message);
		}
	}
}
