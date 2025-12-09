namespace DTech.Logging.Placements
{
	public readonly struct LogInfo
	{
		public LogLevel Level { get; }
		public string Scopes { get; }
		public string Tag { get; }
		public string StateName { get; }

		public LogInfo(LogLevel level, string scopes, string tag, string stateName)
		{
			Level = level;
			Scopes = scopes;
			Tag = tag;
			StateName = stateName;
		}
	}
}