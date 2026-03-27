using System;
using System.Collections.Generic;
using System.Text;
using DTech.Logging.Placements;

namespace DTech.Logging
{
	internal sealed class LogLineBuilder
	{
		private const string DateTimePlacementPrefix = "DATE_TIME:";
		private const string LogLevelPlacement = "LOG_LEVEL";
		private const string LogScopePlacement = "LOG_SCOPE";
		private const string LogTagPlacement = "LOG_TAG";
		private const string LogStatePlacement = "LOG_STATE";
		
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
			if (string.IsNullOrEmpty(_template))
			{
				return _body;
			}

			var logInfo = new LogInfo(_logLevel, _scopes, _tag, _stateName);
			string result = ReplaceBuiltInPlacements(_template, logInfo);
			for (int i = 0; i < _replacers.Count; i++)
			{
				ILogPlacementReplacer replacer = _replacers[i];
				result = replacer.Replace(result, logInfo);
			}

			if (result.EndsWith(" ", StringComparison.Ordinal))
			{
				result = result.Remove(result.Length - 1);
			}

			return result + " " + _body;
		}

		public void Reset()
		{
			_logLevel = LogLevel.Information;
			_scopes = string.Empty;
			_tag = string.Empty;
			_stateName = string.Empty;
			_body = string.Empty;
		}

		private static string ReplaceBuiltInPlacements(string template, LogInfo logInfo)
		{
			var builder = new StringBuilder(template.Length + 16);
			for (int i = 0; i < template.Length; i++)
			{
				if (TryReplaceDateTime(template, ref i, builder))
				{
					continue;
				}

				if (TryReplaceToken(template, ref i, builder, LogLevelPlacement, GetLogLevelString(logInfo.Level), false))
				{
					continue;
				}

				if (TryReplaceToken(template, ref i, builder, LogScopePlacement, logInfo.Scopes, true))
				{
					continue;
				}

				if (TryReplaceToken(template, ref i, builder, LogTagPlacement, logInfo.Tag, true))
				{
					continue;
				}

				if (TryReplaceToken(template, ref i, builder, LogStatePlacement, logInfo.StateName == nameof(NullState) ? string.Empty : logInfo.StateName, true))
				{
					continue;
				}

				builder.Append(template[i]);
			}

			return builder.ToString();
		}

		private static bool TryReplaceDateTime(string template, ref int index, StringBuilder builder)
		{
			if (!MatchesAt(template, index, DateTimePlacementPrefix))
			{
				return false;
			}

			int formatStart = index + DateTimePlacementPrefix.Length;
			int closingBracketIndex = template.IndexOf(']', formatStart);
			if (closingBracketIndex <= formatStart)
			{
				return false;
			}

			string format = template.Substring(formatStart, closingBracketIndex - formatStart);
			builder.Append(DateTime.Now.ToString(format));
			index = closingBracketIndex - 1;
			return true;
		}

		private static bool TryReplaceToken(string template, ref int index, StringBuilder builder, string token, string value, bool removeEmptyBracketedToken)
		{
			if (!MatchesAt(template, index, token))
			{
				return false;
			}

			if (removeEmptyBracketedToken && string.IsNullOrEmpty(value))
			{
				bool hasOpeningBracket = index > 0 && template[index - 1] == '[';
				bool hasClosingBracket = index + token.Length < template.Length && template[index + token.Length] == ']';
				if (hasOpeningBracket && hasClosingBracket && builder.Length > 0 && builder[^1] == '[')
				{
					builder.Length--;
					index += token.Length;
					return true;
				}

				index += token.Length - 1;
				return true;
			}

			builder.Append(value);
			index += token.Length - 1;
			return true;
		}

		private static bool MatchesAt(string source, int startIndex, string token)
		{
			if (startIndex < 0 || startIndex + token.Length > source.Length)
			{
				return false;
			}

			for (int i = 0; i < token.Length; i++)
			{
				if (source[startIndex + i] != token[i])
				{
					return false;
				}
			}

			return true;
		}

		private static string GetLogLevelString(LogLevel logLevel) =>
			logLevel switch
			{
				LogLevel.None => "NONE",
				LogLevel.Trace => "TRACE",
				LogLevel.Debug => "DEBUG",
				LogLevel.Information => "INFO",
				LogLevel.Warning => "WARN",
				LogLevel.Error => "ERROR",
				LogLevel.Critical => "CRIT",
				_ => logLevel.ToString().ToUpperInvariant(),
			};
	}
}
