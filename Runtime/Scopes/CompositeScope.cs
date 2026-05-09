using System;
using System.Buffers;

namespace DTech.Logging
{
	public sealed class CompositeScope : IDisposable
	{
		private readonly IDisposable[] _disposables;
		private readonly int _count;
		private readonly bool _isPooled;
		private bool _isDisposed;

		public CompositeScope(params IDisposable[] disposables)
			: this(disposables, disposables.Length, false)
		{
		}

		internal CompositeScope(IDisposable[] disposables, int count, bool isPooled)
		{
			_disposables = disposables;
			_count = count;
			_isPooled = isPooled;
		}

		public void Dispose()
		{
			if (_isDisposed || _disposables == null)
			{
				return;
			}

			_isDisposed = true;
			for (int i = 0; i < _count; i++)
			{
				IDisposable disposable = _disposables[i];
				if (disposable == null)
				{
					continue;
				}

				disposable.Dispose();
			}

			if (_isPooled)
			{
				Array.Clear(_disposables, 0, _count);
				ArrayPool<IDisposable>.Shared.Return(_disposables);
			}
		}
	}
}
