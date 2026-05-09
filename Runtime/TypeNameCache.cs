namespace DTech.Logging
{
	internal static class TypeNameCache<T>
	{
		public static readonly string Name = typeof(T).Name;
	}
}
