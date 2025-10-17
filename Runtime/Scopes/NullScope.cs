using System;

namespace DTech.Logging
{
	public sealed class NullScope : IDisposable
	{
		public static readonly NullScope Instance = new();
		
		public void Dispose() { }
	}
}