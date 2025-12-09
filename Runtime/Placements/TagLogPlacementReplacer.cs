namespace DTech.Logging.Placements
{
	internal sealed class TagLogPlacementReplacer : LogPlacementReplacer
	{
		protected override string Placement => "LOG_TAG";
		
		public override string Replace(string template, LogInfo logInfo)
		{
			if (template.Contains(Placement))
			{
				if (string.IsNullOrEmpty(logInfo.Tag))
				{
					template = template.Replace($"[{Placement}]", string.Empty);
					template = template.Replace(Placement, "");
				}
				else
				{
					template = template.Replace(Placement, logInfo.Tag);
				}
			}
			
			return template;
		}
	}
}