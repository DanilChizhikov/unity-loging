namespace DTech.Logging.Placements
{
	public interface ILogPlacementReplacer
	{
		string Replace(string template, LogInfo logInfo);
	}
}