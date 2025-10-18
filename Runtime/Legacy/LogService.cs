using System;

namespace DTech.Logging
{
	[Obsolete("Will be removed in the next release.")]
	public sealed class LogService : ILogService
	{
		[Obsolete("Use ILogger instead.")]
		public ILogger GetLogger(string name) => new Logger(name);
	}
}