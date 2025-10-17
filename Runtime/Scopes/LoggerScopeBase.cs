using System;

namespace DTech.Logging
{
	public abstract class LoggerScopeBase : IDisposable
	{
		private readonly object _state;
		
		public LoggerScopeBase(object state)
		{
			if (state == null)
			{
				throw new NullReferenceException("State cannot be null");
			}
			
			_state = state;
		}

		public abstract void Dispose();
	}
}