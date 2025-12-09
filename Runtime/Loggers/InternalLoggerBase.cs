using System;
using System.Collections.Generic;
using System.Threading;

namespace DTech.Logging
{
	internal abstract class InternalLoggerBase : ILogger
	{
		private const string ScopesSeparator = " > ";
		private const string ScopeFormat = "Scope > {0}";
		
		private readonly List<string> _scopes;
		
		internal AsyncLocal<LogScope> CurrentScope { get; }
		
		protected string Tag { get; }
		protected string NullStateName => nameof(NullState);
		protected abstract LogLineBuilder LineBuilder { get; }

		public InternalLoggerBase(string tag)
		{
			Tag = tag;
			_scopes = new List<string>();
			CurrentScope = new AsyncLocal<LogScope>();
		}
		
		public IDisposable BeginScope<TState>()
		{
			return BeginScope(nameof(TState));
		}

		public IDisposable BeginScope(string state)
		{
			var newScope = new LogScope(Tag, state, this, CurrentScope.Value);
			CurrentScope.Value = newScope;
			return newScope;
		}

		public abstract bool IsEnabled(LogLevel logLevel);

		public void Log<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter)
		{
			_scopes.Clear();
			LogScope current = CurrentScope.Value;
			while (current != null)
			{
				_scopes.Add(current.Name);
				current = current.Parent;
			}
			
			_scopes.Reverse();
			string scopes = string.Empty;
			if (_scopes.Count > 0)
			{
				scopes = string.Format(ScopeFormat, string.Join(ScopesSeparator, _scopes));
			}
			
			SendLog<TState>(logLevel, exception, formatter, scopes);
		}

		protected abstract void SendLog<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter, string scopes);
	}
}