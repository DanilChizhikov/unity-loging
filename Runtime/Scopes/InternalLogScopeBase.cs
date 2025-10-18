using System;

namespace DTech.Logging
{
	internal abstract class InternalLogScopeBase : IDisposable
	{
		private readonly string _tag;
		private readonly string _blockName;

		public InternalLogScopeBase(string tag, string blockName)
		{
			_tag = tag;
			if (string.IsNullOrEmpty(blockName))
			{
				throw new ArgumentNullException(nameof(blockName));
			}
			
			_blockName = blockName;
			string message = GetMessage("Begin");
			BeginScope(message);
		}
		
		public void Dispose()
		{
			string message = GetMessage("End");
			EndScope(message);
		}

		protected abstract void BeginScope(string message);
		protected abstract void EndScope(string message);

		private string GetMessage(string scopeAction)
		{
			string message;
			if (string.IsNullOrEmpty(_tag))
			{
				message = $"[Scope {scopeAction}] {_blockName}";
			}
			else
			{
				message = $"[{_tag} Scope {scopeAction}] {_blockName}";
			}
			
			return message;
		}
	}
}