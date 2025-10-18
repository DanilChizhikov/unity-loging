using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DTech.Logging
{
	[Obsolete("Will be removed in the next release.")]
	public static class LegacyLogExtensions
	{
		private const string White = "white";
		private const string Yellow = "yellow";
		private const string Red = "red";
		private const string TimeTagTemplate = "<color={0}>[{1}]";
		
		private static readonly StringBuilder _logBuilder = new();
		
		private static bool CanSendDefaultPriority
		{
			get
			{
				#if DEVELOPMENT_BUILD || UNITY_EDITOR
				return true;
				#endif
				
				return false;
			}
		}

		[Obsolete("Use ILogger instead.")]
		public static void Info(this ILogger logger, object message, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			logger.ThrowIfNull();
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(White, message.ToString(), tags);
				Debug.Log(logBody);
			}
		}
		
		[Obsolete("Use ILogger instead.")]
		public static void InfoFormat(this ILogger logger, string template, LogPriority priority = LogPriority.Default, params object[] args)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = string.Format(template, args);
				Info(logger, logBody, priority);
			}
		}
		
		[Obsolete("Use ILogger instead.")]
		public static void Warning(this ILogger logger, object message, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			logger.ThrowIfNull();
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Yellow, message.ToString(), tags);
				Debug.LogWarning(logBody);
			}
		}
		
		[Obsolete("Use ILogger instead.")]
		public static void WarningFormat(this ILogger logger, string template, LogPriority priority = LogPriority.Default, params object[] args)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = string.Format(template, args);
				Warning(logger, logBody, priority);
			}
		}
		
		[Obsolete("Use ILogger instead.")]
		public static void Error(this ILogger logger, object message, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			logger.ThrowIfNull();
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, message.ToString(), tags);
				Debug.LogError(logBody);
			}
		}
		
		[Obsolete("Use ILogger instead.")]
		public static void ErrorFormat(this ILogger logger, string template, LogPriority priority = LogPriority.Default, params object[] args)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = string.Format(template, args);
				Error(logger, logBody, priority);
			}
		}
		
		[Obsolete("Use ILogger instead.")]
		public static void Exception(this ILogger logger, Exception exception, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			logger.ThrowIfNull();
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, $"EXCEPTION: {exception}", tags);
				Debug.LogError(logBody);
			}
		}
		
		[Obsolete("Use ILogger instead.")]
		public static void ExceptionFatal(this ILogger logger, Exception exception, LogPriority priority = LogPriority.Default, params string[] tags)
		{
			if (priority.IsAvailableToSend())
			{
				string logBody = GetLog(Red, $"FATAL EXCEPTION: {exception}", tags);
				Debug.LogError(logBody);
			}
		}

		internal static bool IsAvailableToSend(this LogPriority priority)
		{
			if (priority == LogPriority.Critical)
			{
				return true;
			}

			return CanSendDefaultPriority;
		}
		
		internal static StringBuilder AppendTags(this StringBuilder builder, params string[] tags)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				string tag = tags[i];
				builder.Append($"[{tag}]");
			}
			
			return builder;
		}
		
		internal static string CleanColorTags(this string log)
		{
			if (string.IsNullOrEmpty(log))
			{
				return log;
			}
			
			log = Regex.Replace(log, @"^<color=.*?>", string.Empty);
			log = Regex.Replace(log, @"</color>$", string.Empty);
			
			return log;
		}
		
		private static string GetLog(string color, string message, params string[] tags)
		{
			_logBuilder.Clear();
			var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
			return _logBuilder.AppendFormat(TimeTagTemplate, color, timestamp)
				.AppendTags(tags)
				.Append(" ")
				.Append(message)
				.Append("</color>")
				.ToString();
		}
	}
}