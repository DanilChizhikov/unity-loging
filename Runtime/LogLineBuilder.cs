using System;
using System.Text.RegularExpressions;

namespace DTech.Logging
{
	public class LogLineBuilder
	{
		private readonly string _template;
		private readonly string _nullStateName;

		private LogLevel _logLevel;
		private string _scopes;
		private string _tag;
		private string _stateName;
		private string _body;
		
		public LogLineBuilder(string template, string nullStateName)
		{
			_template = template;
			_nullStateName = nullStateName;
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
			result = ApplyLogLevel(result);
			result = ApplyDateTime(result);
			result = ApplyScopes(result);
			result = ApplyTag(result);
			result = ApplyState(result);
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
		
		private string ApplyLogLevel(string result)
		{
			if (result.Contains("LOG_LEVEL"))
			{
				string level = _logLevel.ToString().ToUpperInvariant();
				switch (_logLevel)
				{
					case LogLevel.Information:
					case LogLevel.Warning:
					case LogLevel.Critical:
					{
						level = level.Substring(0, 4);
					} break;
				}
				
				result = result.Replace("LOG_LEVEL", level);
			}


			return result;
		}

		private string ApplyDateTime(string result)
		{
			if (result.Contains("DATE_TIME"))
			{
				result = Regex.Replace(result, @"DATE_TIME:([^\]]+)", match =>
				{
					string dtFormat = match.Groups[1].Value;
					return DateTime.Now.ToString(dtFormat);
				});
			}
			
			return result;
		}

		private string ApplyScopes(string result)
		{
			if (result.Contains("LOG_SCOPE"))
			{
				result = result.Replace("LOG_SCOPE", _scopes ?? string.Empty);
			}
			
			return result;
		}

		private string ApplyTag(string result)
		{
			if (result.Contains("LOG_TAG"))
			{
				if (string.IsNullOrEmpty(_tag))
				{
					result = result.Replace("[LOG_TAG]", string.Empty);
					result = result.Replace("LOG_TAG", "");
				}
				else
				{
					result = result.Replace("LOG_TAG", _tag);
				}
			}
			
			return result;
		}
		
		private string ApplyState(string result)
		{
			if (result.Contains("LOG_STATE"))
			{
				string stateName = _stateName == _nullStateName ? string.Empty : _stateName;
				if (string.IsNullOrEmpty(stateName))
				{
					result = result.Replace("[LOG_STATE]", string.Empty);
					result = result.Replace("LOG_STATE", "");
				}
				else
				{
					result = result.Replace("LOG_TAG", stateName);
				}
			}
			
			return result;
		}
	}
}