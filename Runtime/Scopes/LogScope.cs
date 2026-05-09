using System;
using JetBrains.Annotations;

namespace DTech.Logging
{
	internal sealed class LogScope : IDisposable
	{
		private const string ScopesSeparator = " > ";
		private const string ScopePrefix = "Scope > ";

		private readonly Logger _owner;
		private bool _isDisposed;

		public string Name { get; }

		public string Scopes { get; }

		[CanBeNull]
		public LogScope Parent { get; }

		public LogScope(string blockName, [CanBeNull] LogScope parent, Logger owner)
		{
			if (string.IsNullOrEmpty(blockName))
			{
				throw new ArgumentNullException(nameof(blockName));
			}

			Name = blockName;
			Scopes = parent == null
				? ScopePrefix + Name
				: parent.Scopes + ScopesSeparator + Name;
			Parent = parent;
			_owner = owner;
			_isDisposed = false;
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;
			_owner?.OnScopeDisposed(this);
		}
	}
}
