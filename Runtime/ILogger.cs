using System;

namespace DTech.Logging
{
	public interface ILogger
	{
		void Info(object message, LogPriority priority = LogPriority.Default, params string[] tags);
		void InfoFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		void Warning(object message, LogPriority priority = LogPriority.Default, params string[] tags);
		void WarningFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		void Error(object message, LogPriority priority = LogPriority.Default, params string[] tags);
		void ErrorFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		void Exception(Exception exception, LogPriority priority = LogPriority.Default, params string[] tags);
		void ExceptionFatal(Exception exception, LogPriority priority = LogPriority.Default, params string[] tags);
	}
}