using System;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class Logger : ILogger
	{
		private const string White = "white";
		private const string Yellow = "yellow";
		private const string Red = "red";
        
		private readonly string _name;
		public Logger(string name)
		{
			_name = name;
		}

		public void Info(object message, LogPriority priority = LogPriority.Default)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(White, message.ToString());
				Debug.Log(logBody);
			}
		}

		public void InfoFormat(string template, LogPriority priority = LogPriority.Default, params object[] args)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = string.Format(template, args);
				Info(logBody);
			}
		}

		public void Warning(object message, LogPriority priority = LogPriority.Default)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Yellow, message.ToString());
				Debug.LogWarning(logBody);
			}
		}

		public void WarningFormat(string template, LogPriority priority = LogPriority.Default, params object[] args)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = string.Format(template, args);
				Warning(logBody);
			}
		}

		public void Error(object message, LogPriority priority = LogPriority.Default)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, message.ToString());
				Debug.LogError(logBody);
			}
		}

		public void ErrorFormat(string template, LogPriority priority = LogPriority.Default, params object[] args)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = string.Format(template, args);
				Error(logBody);
			}
		}

		public void Exception(Exception exception, LogPriority priority = LogPriority.Default)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, $"EXCEPTION: {exception}");
				Debug.LogError(logBody);
			}
		}

		public void ExceptionFatal(Exception exception, LogPriority priority = LogPriority.Default)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, $"FATAL EXCEPTION: {exception}");
				Debug.LogError(logBody);
			}
		}
        
		private string GetLog(string color, string message)
		{
			var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
			return $"<color={color}>[{timestamp}] [{_name}] {message}</color>";
		}
	}
}