namespace DTech.Logging
{
	public sealed class LogService : ILogService
	{
		public ILogger GetLogger(string name) => new Logger(name);
	}
}