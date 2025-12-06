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
			logger.ThrowIfNull();
			var message = args.Length > 0 ? string.Format(messageFormat, args) : messageFormat;
			return logger.BeginScope(message);
		}

		/// <summary>
		///     Formats and writes a log message at the specified log level.
		/// </summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Format string of the log message.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, Exception exception, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(logLevel, exception, Formatter);

			string Formatter(Exception ex)
			{
				bool hasException = ex != null;
				int lenght = hasException ? args.Length + 1 : args.Length;
				object[] param = new object[lenght];
				if (hasException)
				{
					param[0] = ex.ToString();
				}
				
				for (int i = 0; i < args.Length; i++)
				{
					int paramIndex = hasException ? i + 1 : i;
					param[paramIndex] = args[i];
				}
				
				return string.Format(message, param);
			}
		}

		/// <summary>
		///     Formats and writes a log message at the specified log level.
		/// </summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="message">Format string of the log message.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		public static void Log<TState>(this ILogger logger, LogLevel logLevel, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(logLevel, null, Formatter);

			string Formatter(Exception ex)
			{
				if (args.Length == 0)
				{
					return message;
				}
				
				return string.Format(message, args);
			}
		}
		
		/// <summary>
		///     Formats and writes a log message at the specified log level.
		/// </summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="message">Format string of the log message.</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		public static void Log(this ILogger logger, LogLevel logLevel, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<NullState>(logLevel, null, Formatter);
			string Formatter(Exception ex)
			{
				if (args.Length == 0)
				{
					return message;
				}
				
				return string.Format(message, args);
			}
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
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Critical, exception, message, args);
		}

		/// <summary>Formats and writes a critical log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogCritical("Processing request from {Address}", address)</example>
		public static void LogCritical<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Critical, message, args);
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

		/// <summary>Formats and writes a debug log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogDebug(exception, "Error while processing request from {Address}", address)</example>
		public static void LogDebug<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Debug, exception, message, args);
		}

		/// <summary>Formats and writes a debug log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogDebug("Processing request from {Address}", address)</example>
		public static void LogDebug<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Debug, message, args);
		}
		
		/// <summary>Formats and writes a debug log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <example>logger.LogDebug("Processing request from {Address}", address)</example>
		public static void LogDebug(this ILogger logger, string message, params object[] args)
		{
			logger.LogDebug<NullState>(message, args);
		}

		/// <summary>Formats and writes an error log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogError(exception, "Error while processing request from {Address}", address)</example>
		public static void LogError<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Error, exception, message, args);
		}

		/// <summary>Formats and writes an error log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogError("Processing request from {Address}", address)</example>
		public static void LogError<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Error, message, args);
		}
		
		/// <summary>Formats and writes an error log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <example>logger.LogError("Processing request from {Address}", address)</example>
		public static void LogError(this ILogger logger, string message, params object[] args)
		{
			logger.LogError<NullState>(message, args);
		}

		/// <summary>Formats and writes an informational log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogInfo(exception, "Error while processing request from {Address}", address)</example>
		public static void LogInfo<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Information, exception, message, args);
		}

		/// <summary>Formats and writes an informational log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogInfo("Processing request from {Address}", address)</example>
		public static void LogInfo<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Information, message, args);
		}
		
		/// <summary>Formats and writes an informational log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <example>logger.LogInfo("Processing request from {Address}", address)</example>
		public static void LogInfo(this ILogger logger, string message, params object[] args)
		{
			logger.LogInfo<NullState>(message, args);
		}

		/// <summary>Formats and writes a trace log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogTrace(exception, "Error while processing request from {Address}", address)</example>
		public static void LogTrace<TState>(this ILogger logger, Exception exception, string message,
			params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Trace, exception, message, args);
		}

		/// <summary>Formats and writes a trace log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogTrace("Processing request from {Address}", address)</example>
		public static void LogTrace<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Trace, message, args);
		}
		
		/// <summary>Formats and writes a trace log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <example>logger.LogTrace("Processing request from {Address}", address)</example>
		public static void LogTrace(this ILogger logger, string message, params object[] args)
		{
			logger.LogTrace<NullState>(message, args);
		}

		/// <summary>Formats and writes a warning log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="exception">The exception to log.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogWarning(exception, "Error while processing request from {Address}", address)</example>
		public static void LogWarning<TState>(this ILogger logger, Exception exception, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Warning, exception, message, args);
		}

		/// <summary>Formats and writes a warning log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		/// <example>logger.LogWarning("Processing request from {Address}", address)</example>
		public static void LogWarning<TState>(this ILogger logger, string message, params object[] args)
		{
			logger.ThrowIfNull();
			logger.Log<TState>(LogLevel.Warning, message, args);
		}
		
		/// <summary>Formats and writes a warning log message.</summary>
		/// <param name="logger">The <see cref="T:Logging.ILogger" /> to write to.</param>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <example>logger.LogWarning("Processing request from {Address}", address)</example>
		public static void LogWarning(this ILogger logger, string message, params object[] args)
		{
			logger.LogWarning<NullState>(message, args);
		}

		internal static void ThrowIfNull(this ILogger logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}
		}
	}
}