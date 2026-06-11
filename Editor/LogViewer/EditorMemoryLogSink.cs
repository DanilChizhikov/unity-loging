using System;
using System.Threading;
using DTech.Logging.Attributes;
using UnityEngine;

namespace DTech.Logging.Editor
{
	[DefaultLoggerProvider]
	internal sealed class EditorMemoryLogSink : InternalLoggerBase
	{
		public EditorMemoryLogSink(string tag) : base(tag)
		{
		}

		public override bool IsEnabled(LogLevel logLevel)
		{
			LoggerSettings settings = LoggerSettings.Instance;
			return settings != null && settings.IsEnabled(logLevel);
		}

		protected override void SendLog<TState>(LogLevel logLevel, Exception exception, string message, object[] args, string scopes)
		{
			Capture<TState>(logLevel, exception, FormatMessage(null, message, args), scopes);
		}

		protected override void SendLog<TState>(LogLevel logLevel, Exception exception, string message, string scopes)
		{
			Capture<TState>(logLevel, exception, message, scopes);
		}

		private void Capture<TState>(LogLevel logLevel, Exception exception, string body, string scopes)
		{
			string exceptionText = exception?.ToString();
			int frame = LoggerUtility.IsOnMainThread ? Time.frameCount : -1;

			var entry = new LogEntry(
				logLevel,
				Tag,
				scopes,
				StateName<TState>.Value,
				body,
				exceptionText,
				DateTime.Now,
				frame,
				Thread.CurrentThread.ManagedThreadId);

			EditorLogBuffer.Add(entry);
		}
	}
}
