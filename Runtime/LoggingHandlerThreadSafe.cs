using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

namespace DTech.Logging
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
		private readonly ConcurrentQueue<LogRecord> _queue;
		private readonly SendOrPostCallback _drainCallback;
		private int _drainPosted;

		public LoggingHandlerThreadSafe()
		{
			_unityContext = SynchronizationContext.Current;
			if (_unityContext == null)
			{
				throw new NullReferenceException(ContextNullException);
			}

			_queue = new ConcurrentQueue<LogRecord>();
			_drainCallback = DrainQueue;
			Application.logMessageReceivedThreaded += LogMessageReceivedThreadedHandler;
		}

		public void Dispose()
		{
			Application.logMessageReceivedThreaded -= LogMessageReceivedThreadedHandler;
		}

		private void LogMessageReceivedThreadedHandler(string condition, string stacktrace, LogType type)
		{
			_queue.Enqueue(new LogRecord
			{
				Condition = condition,
				StackTrace = stacktrace,
				LogType = type,
			});

			if (Interlocked.Exchange(ref _drainPosted, 1) == 0)
			{
				_unityContext.Post(_drainCallback, null);
			}
		}

		private void DrainQueue(object state)
		{
			Interlocked.Exchange(ref _drainPosted, 0);

			while (_queue.TryDequeue(out LogRecord record))
			{
				OnLogMessageReceivedThreaded?.Invoke(record.Condition, record.StackTrace, record.LogType);
			}
		}
	}
}
