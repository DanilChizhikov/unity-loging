namespace DTech.Logging.Placements
{
	internal sealed class StateLogPlacementReplacer : LogPlacementReplacer
	{
		private static readonly string _nullStateName = nameof(NullState);
		
		protected override string Placement => "LOG_STATE";
		
		public override string Replace(string template, LogInfo logInfo)
		{
			if (template.Contains(Placement))
			{
				string stateName = logInfo.StateName == _nullStateName ? string.Empty : logInfo.StateName;
				if (string.IsNullOrEmpty(stateName))
				{
					template = template.Replace($"[{Placement}]", string.Empty);
					template = template.Replace(Placement, "");
				}
				else
				{
					template = template.Replace(Placement, stateName);
				}
			}
			
			return template;
		}
	}
}