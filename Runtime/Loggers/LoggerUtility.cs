namespace DTech.Logging
{
	internal static class LoggerUtility
	{
		public static ILogger[] GetDefaultLoggers(string tag)
		{
			return new ILogger[]
			{
				new UnityLogger(tag),
				new FileLogger(tag),
			};
		}
	}
}