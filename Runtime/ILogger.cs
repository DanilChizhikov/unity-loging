using System;

namespace DTech.Logging
{
	public interface ILogger
	{
		/// <summary>Begins a logical operation scope.</summary>
		/// <param name="state">The identifier for the scope.</param>
		/// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
		/// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
		IDisposable BeginScope<TState>();
		
		/// <summary>Begins a logical operation scope.</summary>
		/// <param name="state">The identifier for the scope.</param>
		/// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
		IDisposable BeginScope(string state);

		/// <summary>
		/// Checks if the given <paramref name="logLevel" /> is enabled.
		/// </summary>
		/// <param name="logLevel">Level to be checked.</param>
		/// <returns><c>true</c> if enabled.</returns>
		bool IsEnabled(LogLevel logLevel);

		/// <summary>Writes a log entry.</summary>
		/// <param name="logLevel">Entry will be written on this level.</param>
		/// <param name="exception">The exception related to this entry.</param>
		/// <param name="formatter">Function to create a <see cref="T:System.String" /> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
		/// <typeparam name="TState">The type of the object to be written.</typeparam>
		void Log<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter);
	}
}