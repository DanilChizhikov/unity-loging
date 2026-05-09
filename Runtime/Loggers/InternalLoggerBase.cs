using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DTech.Logging.Placements;

namespace DTech.Logging
{
	internal abstract class InternalLoggerBase : ILogger
	{
		private const string ScopesSeparator = " > ";

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
			var newScope = new LogScope(state, this, CurrentScope.Value);
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
			if (current == null)
			{
				return string.Empty;
			}

			int scopeCount = 0;
			LogScope traversal = current;
			while (traversal != null)
			{
				scopeCount++;
				traversal = traversal.Parent;
			}

			string[] names = ArrayPool<string>.Shared.Rent(scopeCount);
			int index = scopeCount;
			int totalNamesLength = 0;
			traversal = current;
			while (traversal != null)
			{
				string name = traversal.Name;
				names[--index] = name;
				totalNamesLength += name.Length;
				traversal = traversal.Parent;
			}

			int totalLength = totalNamesLength + (scopeCount - 1) * ScopesSeparator.Length;
			var builder = new StringBuilder(totalLength);
			for (int i = 0; i < scopeCount; i++)
			{
				if (i > 0)
				{
					builder.Append(ScopesSeparator);
				}

				builder.Append(names[i]);
			}

			string log = builder.ToString();
			Array.Clear(names, 0, scopeCount);
			ArrayPool<string>.Shared.Return(names);

			return log;
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
