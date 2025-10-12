using System;
using System.Threading;
using UnityEngine;

namespace DTech.Logging.Editor
{
	internal sealed class LoggingHandlerThreadSafe : IDisposable
	{
		private struct LogRecord
		{
			public string Condition;
			public string StackTrace;
			public LogType LogType;
		}
		
		private const string ContextNullException = "Unity SynchronizationContext is null";
		
		public event Application.LogCallback OnLogMessageReceivedThreaded;

		private readonly SynchronizationContext _unityContext;

		public LoggingHandlerThreadSafe()
		{
			_unityContext = SynchronizationContext.Current;
			if (_unityContext == null)
			{
				throw new NullReferenceException(ContextNullException);
			}
            
			Application.logMessageReceivedThreaded += LogMessageReceivedThreadedHandler;
		}

		public void Dispose()
		{
			Application.logMessageReceivedThreaded -= LogMessageReceivedThreadedHandler;
		}
		
		private void LogMessageReceivedThreadedHandler(string condition, string stacktrace, LogType type)
		{
			_unityContext.Post(ContextCallback, new LogRecord
			{
				Condition = condition,
				StackTrace = stacktrace,
				LogType = type,
			});
		}
		
		private void ContextCallback(object state)
		{
			var record = (LogRecord)state;
			OnLogMessageReceivedThreaded?.Invoke(record.Condition, record.StackTrace, record.LogType);
		}
	}
}