using System;

namespace DTech.Logging
{
	internal abstract class InternalLoggerBase : ILogger
	{
		public IDisposable BeginScope<TState>()
		{
			return BeginScope(nameof(TState));
		}
		
		public abstract IDisposable BeginScope(string state);

		public abstract bool IsEnabled(LogLevel logLevel);

		public abstract void Log<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter);
	}
}