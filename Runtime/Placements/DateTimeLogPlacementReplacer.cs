using System;
using System.Text.RegularExpressions;

namespace DTech.Logging.Placements
{
	internal sealed class DateTimeLogPlacementReplacer : LogPlacementReplacer
	{
		protected override string Placement => "DATE_TIME";
		
		public override string Replace(string template, LogInfo logInfo)
		{
			if (template.Contains(Placement))
			{
				template = Regex.Replace(template, $@"{Placement}:([^\]]+)", match =>
				{
					string dtFormat = match.Groups[1].Value;
					return DateTime.Now.ToString(dtFormat);
				});
			}
			
			return template;
		}
	}
}