using System;

namespace DTech.Logging
{
	[Obsolete("Will be removed in the next release.")]
	public interface ILogService
	{
		ILogger GetLogger(string name);
	}
}