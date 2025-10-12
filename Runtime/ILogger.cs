using System;

namespace DTech.Logging
{
	public interface ILogger
	{
		void Info(object message);
		void InfoFormat(string template, params object[] args);
		void Warning(object message);
		void WarningFormat(string template, params object[] args);
		void Error(object message);
		void ErrorFormat(string template, params object[] args);
		void Exception(Exception exception);
		void ExceptionFatal(Exception exception);
	}
}