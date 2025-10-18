using System;

namespace DTech.Logging
{
	[Obsolete("Use ILogger<TCategoryName> instead.")]
	public interface ILoggerT<T> : ILogger
	{
		/// <summary>Begins a logical operation scope.</summary>
		/// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
		IDisposable BeginScope();
		
		/// <summary>Writes a log entry.</summary>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="exception">The exception related to this entry.</param>
		/// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
		void Log(LogLevel logLevel, Exception exception, Func<Exception, string> formatter);
		
		/// <summary>
		/// Writes an informational log entry.
		/// </summary>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		void LogInfo(string message, params object[] args);
		
		/// <summary>
		/// Writes an debug log entry.
		/// </summary>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		void LogDebug(string message, params object[] args);
		
		/// <summary>
		/// Writes a warning log entry.
		/// </summary>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		void LogWarning(string message, params object[] args);
		
		/// <summary>
		/// Writes a trace log entry.
		/// </summary>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		void LogTrace(string message, params object[] args);
		
		/// <summary>
		/// Writes an errors log entry.
		/// </summary>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		void LogError(string message, params object[] args);
		
		/// <summary>
		/// Writes a critical log entry.
		/// </summary>
		/// <param name="message">Format string of the log message in message template format. Example: <c>"User {User} logged in from {Address}"</c></param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		void LogCritical(string message, params object[] args);
	}
}