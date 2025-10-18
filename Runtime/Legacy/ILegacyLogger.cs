using System;

namespace DTech.Logging
{
	[Obsolete("Use ILogger instead.")]
	public interface ILegacyLogger
	{
		[Obsolete("Use ILogger instead.")]
		void Info(object message, LogPriority priority = LogPriority.Default, params string[] tags);
		[Obsolete("Use ILogger instead.")]
		void InfoFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		[Obsolete("Use ILogger instead.")]
		void Warning(object message, LogPriority priority = LogPriority.Default, params string[] tags);
		[Obsolete("Use ILogger instead.")]
		void WarningFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		[Obsolete("Use ILogger instead.")]
		void Error(object message, LogPriority priority = LogPriority.Default, params string[] tags);
		[Obsolete("Use ILogger instead.")]
		void ErrorFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		[Obsolete("Use ILogger instead.")]
		void Exception(Exception exception, LogPriority priority = LogPriority.Default, params string[] tags);
		[Obsolete("Use ILogger instead.")]
		void ExceptionFatal(Exception exception, LogPriority priority = LogPriority.Default, params string[] tags);
	}
}