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
	}
}