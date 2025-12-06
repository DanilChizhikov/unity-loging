namespace DTech.Logging
{
	public static class LogConditions
	{
		private const bool True = true;
		private const bool False = false;

		#if DISABLE_FILE_LOGGING
		public const bool IsFileLoggingEnabled = False;
		#else
		public const bool IsFileLoggingEnabled = True;
		#endif

		#if DEVELOPMENT_BUILD || UNITY_EDITOR
		public const bool IsDevelopmentBuild = True;
		#else
		public const bool IsDevelopmentBuild = False;
		#endif
	}
}