using System;

namespace DTech.Logging.Placements
{
	internal sealed class DateTimeLogPlacementReplacer : LogPlacementReplacer
	{
		protected override string Placement => "DATE_TIME";

		public override string Replace(string template, LogInfo logInfo)
		{
			if (string.IsNullOrEmpty(template))
			{
				return template;
			}

			int searchIndex = 0;
			const string Prefix = "DATE_TIME:";
			while (searchIndex < template.Length)
			{
				int tokenIndex = template.IndexOf(Prefix, searchIndex, StringComparison.Ordinal);
				if (tokenIndex < 0)
				{
					break;
				}

				int formatStart = tokenIndex + Prefix.Length;
				int closingBracketIndex = template.IndexOf(']', formatStart);
				if (closingBracketIndex <= formatStart)
				{
					searchIndex = formatStart;
					continue;
				}

				string format = template.Substring(formatStart, closingBracketIndex - formatStart);
				string value = DateTime.Now.ToString(format);
				template = template.Remove(tokenIndex, Prefix.Length + format.Length).Insert(tokenIndex, value);
				searchIndex = tokenIndex + value.Length;
			}

			return template;
		}
	}
}
