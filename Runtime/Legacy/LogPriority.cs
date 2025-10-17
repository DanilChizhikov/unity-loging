using System;

namespace DTech.Logging
{
	[Obsolete("Use ILogger instead.")]
	public enum LogPriority : byte
	{
		Default = 0,
		Critical = 1,
	}
}