using System.Text;
using System.Text.RegularExpressions;

namespace DTech.Logging
{
	internal static class LogExtensions
	{
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

		public static bool IsAvailableToSend(this LogPriority priority)
		{
			if (priority == LogPriority.Critical)
			{
				return true;
			}

			return CanSendDefaultPriority;
		}
		
		public static StringBuilder AppendTags(this StringBuilder builder, params string[] tags)
		{
			for (int i = 0; i < tags.Length; i++)
			{
				string tag = tags[i];
				builder.Append($"[{tag}]");
			}
			
			return builder;
		}
		
		public static string CleanColorTags(this string log)
		{
			if (string.IsNullOrEmpty(log))
			{
				return log;
			}
			
			log = Regex.Replace(log, @"^<color=.*?>", string.Empty);
			log = Regex.Replace(log, @"</color>$", string.Empty);
			
			return log;
		}
	}
}