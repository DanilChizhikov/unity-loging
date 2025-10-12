namespace DTech.Logging
{
	public interface ILogService
	{
		ILogger GetLogger(string name);
	}
}