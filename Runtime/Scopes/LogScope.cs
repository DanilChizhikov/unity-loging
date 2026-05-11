using System;
using JetBrains.Annotations;

namespace DTech.Logging
{
	internal sealed class LogScope : IDisposable
	{
		private const string ScopesSeparator = " > ";
		private const string ScopePrefix = "Scope > ";

		private readonly Logger _owner;

		public string Name { get; }

		public string Scopes { get; }

		[CanBeNull]
		public LogScope Parent { get; }

		public bool IsDisposed { get; private set; }

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
			IsDisposed = false;
		}

		public void Dispose()
		{
			if (IsDisposed)
			{
				return;
			}

			IsDisposed = true;
			_owner?.OnScopeDisposed(this);
		}
	}
}
