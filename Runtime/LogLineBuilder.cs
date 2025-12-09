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

		private LogLevel _logLevel;
		private string _scopes;
		private string _tag;
		private string _stateName;
		private string _body;
		
		public LogLineBuilder(string template)
		{
			_template = template;
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
			var logInfo = new LogInfo(_logLevel, _scopes, _tag, _stateName);
			string result = _template;

			foreach (var placementReplacer in _placementReplacers)
			{
				result = placementReplacer.Replace(result, logInfo);
			}
			
			for (int i = 0; i < LoggerSettings.Instance.PlacementReplacers.Count; i++)
			{
				ILogPlacementReplacer replacer = LoggerSettings.Instance.PlacementReplacers[i];
				result = replacer.Replace(result, logInfo);
			}
			
			result += $" {_body}";
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