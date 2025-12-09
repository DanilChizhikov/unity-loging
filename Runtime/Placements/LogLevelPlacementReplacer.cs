namespace DTech.Logging.Placements
{
	internal sealed class LogLevelPlacementReplacer : LogPlacementReplacer
	{
		protected override string Placement => "LOG_LEVEL";
		
		public override string Replace(string template, LogInfo logInfo)
		{
			if (template.Contains(Placement))
			{
				string level = logInfo.Level.ToString().ToUpperInvariant();
				switch (logInfo.Level)
				{
					case LogLevel.Information:
					case LogLevel.Warning:
					case LogLevel.Critical:
					{
						level = level.Substring(0, 4);
					} break;
				}
				
				template = template.Replace(Placement, level);
			}


			return template;
		}
	}
}