using System.Collections.Generic;
using DTech.Logging.Placements;

namespace DTech.Logging
{
	internal sealed class LogLineBuilder
	{
		private static readonly ILogPlacementReplacer[] _placementReplacers = new ILogPlacementReplacer[]
		{
			new DateTimeLogPlacementReplacer(),
			new LogLevelPlacementReplacer(),
			new ScopesLogPlacementReplacer(),
			new TagLogPlacementReplacer(),
			new StateLogPlacementReplacer(),
		};
		
		private readonly string _template;
		private readonly List<ILogPlacementReplacer> _replacers;

		private LogLevel _logLevel;
		private string _scopes;
		private string _tag;
		private string _stateName;
		private string _body;
		
		public LogLineBuilder(string template, IEnumerable<ILogPlacementReplacer> replacers)
		{
			_template = template;
			_replacers = new List<ILogPlacementReplacer>(replacers);
		}
		
		public LogLineBuilder SetLogLevel(LogLevel logLevel)
		{
			_logLevel = logLevel;
			return this;
		}
		
		public LogLineBuilder SetScopes(string scopes)
		{
			_scopes = scopes;
			return this;
		}

		public LogLineBuilder SetTag(string tag)
		{
			_tag = tag;
			return this;
		}
		
		public LogLineBuilder SetStateName(string stateName)
		{
			_stateName = stateName;
			return this;
		}
		
		public LogLineBuilder SetBody(string body)
		{
			_body = body;
			return this;
		}

		public override string ToString()
		{
			string result = _template;
			if (string.IsNullOrEmpty(result))
			{
				result = _body;
			}
			else
			{
				var logInfo = new LogInfo(_logLevel, _scopes, _tag, _stateName);
				foreach (var placementReplacer in _placementReplacers)
				{
					result = placementReplacer.Replace(result, logInfo);
				}
			
				for (int i = 0; i < _replacers.Count; i++)
				{
					ILogPlacementReplacer replacer = _replacers[i];
					result = replacer.Replace(result, logInfo);
				}

				if (result.EndsWith(" "))
				{
					result = result.Remove(result.Length - 1);
				}
				
				result += $" {_body}";
			}
			
			return result;
		}

		public void Reset()
		{
			_logLevel = LogLevel.Information;
			_scopes = string.Empty;
			_tag = string.Empty;
			_stateName = string.Empty;
			_body = string.Empty;
		}
	}
}