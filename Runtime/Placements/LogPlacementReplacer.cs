namespace DTech.Logging.Placements
{
	internal abstract class LogPlacementReplacer : ILogPlacementReplacer
	{
		protected abstract string Placement { get; }

		public abstract string Replace(string template, LogInfo logInfo);
	}
}