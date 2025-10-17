using System;

namespace DTech.Logging
{
	public sealed class CompositeScope : IDisposable
	{
		private readonly IDisposable[] _disposables;
		
		public CompositeScope(params IDisposable[] disposables)
		{
			_disposables = disposables;
		}
		
		public void Dispose()
		{
			foreach (var disposable in _disposables)
			{
				disposable.Dispose();
			}
		}
	}
}