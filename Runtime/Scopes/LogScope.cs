using System;
using JetBrains.Annotations;

namespace DTech.Logging
{
	internal sealed class LogScope : IDisposable
	{
		private readonly string _tag;
		private readonly string _blockName;
		private readonly InternalLoggerBase _logger;

		public string Name { get; }
		[CanBeNull] public LogScope Parent { get; }

		private bool _isDisposed;

		public LogScope(string tag, string blockName, InternalLoggerBase logger, [CanBeNull] LogScope parent)
		{
			_tag = tag;
			if (string.IsNullOrEmpty(blockName))
			{
				throw new ArgumentNullException(nameof(blockName));
			}

			_blockName = blockName;
			Name = string.IsNullOrEmpty(tag) ? $"{blockName}" : $"{tag} > {blockName}";
			_logger = logger;
			Parent = parent;
			_isDisposed = false;
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_logger.CurrentScope.Value = Parent;
			_isDisposed = true;
		}
	}
}