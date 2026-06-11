using System;

namespace DTech.Logging.Editor
{
	internal readonly struct LogEntry
	{
		public readonly LogLevel Level;
		public readonly string Tag;
		public readonly string Scopes;
		public readonly string StateName;
		public readonly string Message;
		public readonly string Exception;
		public readonly DateTime Time;
		public readonly int Frame;
		public readonly int ThreadId;

		public LogEntry(
			LogLevel level,
			string tag,
			string scopes,
			string stateName,
			string message,
			string exception,
			DateTime time,
			int frame,
			int threadId)
		{
			Level = level;
			Tag = tag ?? string.Empty;
			Scopes = scopes ?? string.Empty;
			StateName = stateName ?? string.Empty;
			Message = message ?? string.Empty;
			Exception = exception;
			Time = time;
			Frame = frame;
			ThreadId = threadId;
		}

		public bool HasException => !string.IsNullOrEmpty(Exception);
	}
}