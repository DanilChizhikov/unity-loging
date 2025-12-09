using UnityEngine;

namespace DTech.Logging.Placements
{
	public abstract class ScriptableLogPlacementReplacer : ScriptableObject, ILogPlacementReplacer
	{
		[field: SerializeField] protected string Placement { get; private set; }

		public abstract string Replace(string template, LogInfo logInfo);
	}
}