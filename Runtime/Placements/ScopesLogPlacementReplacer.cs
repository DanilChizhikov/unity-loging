namespace DTech.Logging.Placements
{
	internal sealed class ScopesLogPlacementReplacer : LogPlacementReplacer
	{
		protected override string Placement => "LOG_SCOPE";
		
		public override string Replace(string template, LogInfo logInfo)
		{
			if (template.Contains(Placement))
			{
				template = template.Replace(Placement, logInfo.Scopes ?? string.Empty);
			}
			
			return template;
		}
	}
}