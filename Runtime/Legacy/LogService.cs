using System;

namespace DTech.Logging
{
	[Obsolete("Will be removed in the next major release.")]
	public sealed class LogService : ILogService
	{
		public ILogger GetLogger(string name) => new Logger();
	}
}