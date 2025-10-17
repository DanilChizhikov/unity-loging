using System;

namespace DTech.Logging
{
	public sealed class UnityLogScope : LoggerScopeBase
	{
		public UnityLogScope(object state) : base(state)
		{
		}

		public override void Dispose()
		{
			
		}
	}
}