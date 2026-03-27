namespace DTech.Logging.Tests.Performance
{
	internal static class LoggerFactory
	{
		public static ILogger CreateLogger(string tag) => new Logger(tag, new PerformanceInternalLogger(tag));
		
		public static ILogger<TState> CreateLogger<TState>() => new Logger<TState>(new PerformanceInternalLogger(typeof(TState).Name));
	}
}