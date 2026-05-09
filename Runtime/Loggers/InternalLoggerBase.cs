using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using DTech.Logging.Placements;

namespace DTech.Logging
{
	internal abstract class InternalLoggerBase : ILogger
	{
		internal AsyncLocal<LogScope> CurrentScope { get; }

		protected string Tag { get; }

		private LogLineBuilder _lineBuilder;
		private string _lineBuilderFormat;
		private IReadOnlyList<ILogPlacementReplacer> _lineBuilderReplacers;

		public InternalLoggerBase(string tag)
		{
			Tag = tag;
			CurrentScope = new AsyncLocal<LogScope>();
		}

		public IDisposable BeginScope<TState>()
		{
			return BeginScope(TypeNameCache<TState>.Name);
		}

		public IDisposable BeginScope(string state)
		{
			var newScope = new LogScope(Tag, state, this, CurrentScope.Value);
			CurrentScope.Value = newScope;
			return newScope;
		}

		public abstract bool IsEnabled(LogLevel logLevel);

		public void Log<TState>(LogLevel logLevel, Exception exception, string message, object[] args)
		{
			string scopes = BuildScopesString(CurrentScope.Value);
			SendLog<TState>(logLevel, exception, message, args, scopes);
		}

		public void Log<TState>(LogLevel logLevel, Exception exception, string message)
		{
			string scopes = BuildScopesString(CurrentScope.Value);
			SendLog<TState>(logLevel, exception, message, scopes);
		}
		
		protected abstract void SendLog<TState>(LogLevel logLevel, Exception exception, string message, object[] args, string scopes);
		
		protected abstract void SendLog<TState>(LogLevel logLevel, Exception exception, string message, string scopes);
		
		protected static string FormatMessage(Exception exception, string message, object[] args)
		{
			string formatted = (args == null || args.Length == 0)
				? message
				: FormatMessageWithoutException(message, args);

			return exception == null ? formatted : formatted + "\n" + exception;
		}

		protected static string FormatMessage(Exception exception, string message)
		{
			return exception == null ? message : message + "\n" + exception;
		}
		
		protected LogLineBuilder GetOrCreateLineBuilder(string format, IReadOnlyList<ILogPlacementReplacer> replacers)
		{
			if (_lineBuilder != null &&
			    ReferenceEquals(_lineBuilderFormat, format) &&
			    ReferenceEquals(_lineBuilderReplacers, replacers))
			{
				return _lineBuilder;
			}

			_lineBuilder = new LogLineBuilder(format, replacers);
			_lineBuilderFormat = format;
			_lineBuilderReplacers = replacers;
			return _lineBuilder;
		}

		private static string BuildScopesString(LogScope current)
		{
			return current?.Scopes ?? string.Empty;
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
	}
}
