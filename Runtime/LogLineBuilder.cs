using System;
using System.Collections.Generic;
using System.Text;
using DTech.Logging.Placements;

namespace DTech.Logging
{
	internal sealed class LogLineBuilder
	{
		private enum TemplateSegmentKind : byte
		{
			Literal = 0,
			DateTime = 1,
			LogLevel = 2,
			LogScope = 3,
			LogTag = 4,
			LogState =5,
		}

		private readonly struct TemplateSegment
		{
			public TemplateSegment(TemplateSegmentKind kind, string value, bool isBracketed, bool removeWhenEmpty)
			{
				Kind = kind;
				Value = value;
				IsBracketed = isBracketed;
				RemoveWhenEmpty = removeWhenEmpty;
			}

			public TemplateSegmentKind Kind { get; }
			public string Value { get; }
			public bool IsBracketed { get; }
			public bool RemoveWhenEmpty { get; }
		}

		private const string DateTimePlacementPrefix = "DATE_TIME:";
		private const string LogLevelPlacement = "LOG_LEVEL";
		private const string LogScopePlacement = "LOG_SCOPE";
		private const string LogTagPlacement = "LOG_TAG";
		private const string LogStatePlacement = "LOG_STATE";
		
		private const int DefaultBuilderCapacity = 128;

		private readonly List<ILogPlacementReplacer> _replacers;
		private readonly TemplateSegment[] _segments;

		private LogLevel _logLevel;
		private string _scopes;
		private string _tag;
		private string _stateName;
		private string _body;

		public LogLineBuilder(string template, IEnumerable<ILogPlacementReplacer> replacers)
		{
			_replacers = new List<ILogPlacementReplacer>(replacers);
			_segments = ParseTemplate(template);
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
			if (_segments.Length == 0)
			{
				return _body;
			}

			var logInfo = new LogInfo(_logLevel, _scopes, _tag, _stateName);
			string result = ReplaceBuiltInPlacements(logInfo);
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

		private string ReplaceBuiltInPlacements(LogInfo logInfo)
		{
			var builder = new StringBuilder(DefaultBuilderCapacity);
			for (int i = 0; i < _segments.Length; i++)
			{
				TemplateSegment segment = _segments[i];
				switch (segment.Kind)
				{
					case TemplateSegmentKind.Literal:
					{
						builder.Append(segment.Value);
					} break;

					case TemplateSegmentKind.DateTime:
					{
						AppendTokenValue(builder, DateTime.Now.ToString(segment.Value), segment.IsBracketed, false);
					} break;

					case TemplateSegmentKind.LogLevel:
					{
						AppendTokenValue(builder, GetLogLevelString(logInfo.Level), segment.IsBracketed, segment.RemoveWhenEmpty);
					} break;

					case TemplateSegmentKind.LogScope:
					{
						AppendTokenValue(builder, logInfo.Scopes, segment.IsBracketed, segment.RemoveWhenEmpty);
					} break;

					case TemplateSegmentKind.LogTag:
					{
						AppendTokenValue(builder, logInfo.Tag, segment.IsBracketed, segment.RemoveWhenEmpty);
					} break;

					case TemplateSegmentKind.LogState:
					{
						string stateName = logInfo.StateName == nameof(NullState) ? string.Empty : logInfo.StateName;
						AppendTokenValue(builder, stateName, segment.IsBracketed, segment.RemoveWhenEmpty);
					} break;
				}
			}

			return builder.ToString();
		}

		private static TemplateSegment[] ParseTemplate(string template)
		{
			if (string.IsNullOrEmpty(template))
			{
				return Array.Empty<TemplateSegment>();
			}

			var segments = new List<TemplateSegment>(8);
			int literalStart = 0;
			int i = 0;
			while (i < template.Length)
			{
				if (TryReadDateTimeSegment(template, i, out TemplateSegment dateSegment, out int nextIndex))
				{
					int tokenStart = i;
					int literalEnd = dateSegment.IsBracketed ? tokenStart - 1 : tokenStart;
					if (literalEnd > literalStart)
					{
						segments.Add(new TemplateSegment(TemplateSegmentKind.Literal, template.Substring(literalStart, literalEnd - literalStart), false, false));
					}

					segments.Add(dateSegment);
					i = nextIndex;
					literalStart = i;
					continue;
				}

				if (TryReadBuiltInTokenSegment(template, i, out TemplateSegment tokenSegment, out int tokenLength))
				{
					bool hasOpeningBracket = i > 0 && template[i - 1] == '[';
					bool hasClosingBracket = i + tokenLength < template.Length && template[i + tokenLength] == ']';
					bool isBracketed = hasOpeningBracket && hasClosingBracket;

					int literalEnd = isBracketed ? i - 1 : i;
					if (literalEnd > literalStart)
					{
						segments.Add(new TemplateSegment(TemplateSegmentKind.Literal, template.Substring(literalStart, literalEnd - literalStart), false, false));
					}

					segments.Add(new TemplateSegment(tokenSegment.Kind, tokenSegment.Value, isBracketed, tokenSegment.RemoveWhenEmpty));
					i += tokenLength + (isBracketed ? 1 : 0);
					literalStart = i;
					continue;
				}

				i++;
			}

			if (literalStart < template.Length)
			{
				segments.Add(new TemplateSegment(TemplateSegmentKind.Literal, template.Substring(literalStart), false, false));
			}

			return segments.ToArray();
		}

		private static bool TryReadDateTimeSegment(string template, int index, out TemplateSegment segment, out int nextIndex)
		{
			segment = default;
			nextIndex = index;
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

			bool isBracketed = index > 0 && template[index - 1] == '[';
			string format = template.Substring(formatStart, closingBracketIndex - formatStart);
			segment = new TemplateSegment(TemplateSegmentKind.DateTime, format, isBracketed, false);
			nextIndex = isBracketed ? closingBracketIndex + 1 : closingBracketIndex;
			return true;
		}

		private static bool TryReadBuiltInTokenSegment(string template, int index, out TemplateSegment segment, out int tokenLength)
		{
			segment = default;
			tokenLength = 0;

			if (MatchesAt(template, index, LogLevelPlacement))
			{
				segment = new TemplateSegment(TemplateSegmentKind.LogLevel, null, false, false);
				tokenLength = LogLevelPlacement.Length;
				return true;
			}

			if (MatchesAt(template, index, LogScopePlacement))
			{
				segment = new TemplateSegment(TemplateSegmentKind.LogScope, null, false, true);
				tokenLength = LogScopePlacement.Length;
				return true;
			}

			if (MatchesAt(template, index, LogTagPlacement))
			{
				segment = new TemplateSegment(TemplateSegmentKind.LogTag, null, false, true);
				tokenLength = LogTagPlacement.Length;
				return true;
			}

			if (MatchesAt(template, index, LogStatePlacement))
			{
				segment = new TemplateSegment(TemplateSegmentKind.LogState, null, false, true);
				tokenLength = LogStatePlacement.Length;
				return true;
			}

			return false;
		}

		private static void AppendTokenValue(StringBuilder builder, string value, bool isBracketed, bool removeWhenEmpty)
		{
			if (removeWhenEmpty && string.IsNullOrEmpty(value))
			{
				return;
			}

			if (isBracketed)
			{
				builder.Append('[');
			}

			builder.Append(value);

			if (isBracketed)
			{
				builder.Append(']');
			}
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
