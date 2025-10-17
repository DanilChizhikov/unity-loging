using System;

namespace DTech.Logging
{
	[Obsolete("Will be removed in the next major release.")]
	public interface ILogService
	{
		ILogger GetLogger(string name);
	}
}