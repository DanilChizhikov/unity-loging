using System;
using System.Buffers;
using System.Collections.Generic;
using DTech.Logging.Placements;

namespace DTech.Logging
{
	internal abstract class InternalLoggerBase : ILogger
	{
		protected string Tag { get; }

		private LogLineBuilder _lineBuilder;
		private string _lineBuilderFormat;
		private IReadOnlyList<ILogPlacementReplacer> _lineBuilderReplacers;

		public InternalLoggerBase(string tag)
		{
			Tag = tag;
		}

		// Scope tracking lives on the outer Logger now. Internal loggers
		// expose no-op scope handles to satisfy ILogger.
		public IDisposable BeginScope<TState>() => NullScope.Instance;
		public IDisposable BeginScope(string state) => NullScope.Instance;

		public abstract bool IsEnabled(LogLevel logLevel);

		public void Log<TState>(LogLevel logLevel, Exception exception, string message, object[] args)
		{
			SendLog<TState>(logLevel, exception, message, args, scopes: string.Empty);
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, string message)
		{
			SendLog<TState>(logLevel, exception, message, scopes: string.Empty);
		}

		internal void Log<TState>(LogLevel logLevel, Exception exception, string message, object[] args, string scopes)
		{
			SendLog<TState>(logLevel, exception, message, args, scopes);
		}

		internal void Log<TState>(LogLevel logLevel, Exception exception, string message, string scopes)
		{
			SendLog<TState>(logLevel, exception, message, scopes);
		}

		protected abstract void SendLog<TState>(LogLevel logLevel, Exception exception, string message, object[] args, string scopes);

		protected abstract void SendLog<TState>(LogLevel logLevel, Exception exception, string message, string scopes);

		protected static string FormatMessage(Exception exception, string message, object[] args)
		{
			if (args == null || args.Length == 0)
			{
				if (exception == null)
				{
					return message;
				}

				return string.Format(message, exception.ToString());
			}

			if (exception == null)
			{
				return FormatMessageWithoutException(message, args);
			}

			return FormatMessageWithException(message, exception, args);
		}

		protected static string FormatMessage(Exception exception, string message)
		{
			if (exception == null)
			{
				return message;
			}

			return string.Format(message, exception.ToString());
		}

		protected LogLineBuilder GetOrCreateLineBuilder(string format, IReadOnlyList<ILogPlacementReplacer> replacers)
		{
			if (_lineBuilder != null &&
			    string.Equals(_lineBuilderFormat, format, StringComparison.Ordinal) &&
			    ReferenceEquals(_lineBuilderReplacers, replacers))
			{
				return _lineBuilder;
			}

			_lineBuilder = new LogLineBuilder(format, replacers);
			_lineBuilderFormat = format;
			_lineBuilderReplacers = replacers;
			return _lineBuilder;
		}

		private static string FormatMessageWithoutException(string message, object[] args)
		{
			return args.Length switch
			{
				1 => string.Format(message, args[0]),
				2 => string.Format(message, args[0], args[1]),
				3 => string.Format(message, args[0], args[1], args[2]),
				_ => string.Format(message, args),
			};
		}

		private static string FormatMessageWithException(string message, Exception exception, object[] args)
		{
			string exceptionString = exception.ToString();
			if (args.Length == 1)
			{
				return string.Format(message, exceptionString, args[0]);
			}

			if (args.Length == 2)
			{
				return string.Format(message, exceptionString, args[0], args[1]);
			}

			int length = args.Length + 1;
			object[] pooledArgs = ArrayPool<object>.Shared.Rent(length);
			try
			{
				pooledArgs[0] = exceptionString;
				for (int i = 0; i < args.Length; i++)
				{
					pooledArgs[i + 1] = args[i];
				}

				return string.Format(message, pooledArgs);
			}
			finally
			{
				Array.Clear(pooledArgs, 0, length);
				ArrayPool<object>.Shared.Return(pooledArgs);
			}
		}
	}
}
