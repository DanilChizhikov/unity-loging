using System;
using System.Text;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class LegacyLogger
	{
		private const string White = "white";
		private const string Yellow = "yellow";
		private const string Red = "red";
		private const string TimeTagTemplate = "<color={0}>[{1}]";
        
		private readonly string _name;
		private readonly StringBuilder _logBuilder;
		
		public LegacyLogger(string name)
		{
			_name = name;
			_logBuilder = new StringBuilder();
		}

		public void Info(object message, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(White, message.ToString(), tags);
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

		public void Warning(object message, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Yellow, message.ToString(), tags);
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

		public void Error(object message, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, message.ToString(), tags);
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

		public void Exception(Exception exception, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, $"EXCEPTION: {exception}", tags);
				Debug.LogError(logBody);
			}
		}

		public void ExceptionFatal(Exception exception, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, $"FATAL EXCEPTION: {exception}", tags);
				Debug.LogError(logBody);
			}
		}
        
		private string GetLog(string color, string message, params string[] tags)
		{
			_logBuilder.Clear();
			var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
			return _logBuilder.AppendFormat(TimeTagTemplate, color, timestamp)
				.Append($"[{_name}]")
				.AppendTags(tags)
				.Append(" ")
				.Append(message)
				.Append("</color>")
				.ToString();
		}
	}
}