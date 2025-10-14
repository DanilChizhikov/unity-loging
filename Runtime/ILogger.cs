using System;

namespace DTech.Logging
{
	public interface ILogger
	{
		void Info(object message, LogPriority priority = LogPriority.Default);
		void InfoFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		void Warning(object message, LogPriority priority = LogPriority.Default);
		void WarningFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		void Error(object message, LogPriority priority = LogPriority.Default);
		void ErrorFormat(string template, LogPriority priority = LogPriority.Default, params object[] args);
		void Exception(Exception exception, LogPriority priority = LogPriority.Default);
		void ExceptionFatal(Exception exception, LogPriority priority = LogPriority.Default);
	}
}