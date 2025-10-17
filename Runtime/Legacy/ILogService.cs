using System;

namespace DTech.Logging
{
	[Obsolete("Will be removed in the next release.")]
	public interface ILogService
	{
		[Obsolete("Use ILogger instead.")]
		ILogger GetLogger(string name);
	}
}