namespace DTech.Logging.Placements
{
	internal sealed class ScopesLogPlacementReplacer : LogPlacementReplacer
	{
		protected override string Placement => "LOG_SCOPE";
		
		public override string Replace(string template, LogInfo logInfo)
		{
			if (template.Contains(Placement))
			{
				if (string.IsNullOrEmpty(logInfo.Scopes))
				{
					template = template.Replace($"[{Placement}]", string.Empty);
					template = template.Replace(Placement, string.Empty);
				}
				else
				{
					template = template.Replace(Placement, logInfo.Scopes);
				}
			}
			
			return template;
		}
	}
}