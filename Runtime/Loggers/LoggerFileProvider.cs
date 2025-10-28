using System;
using System.IO;

namespace DTech.Logging
{
	internal static class LoggerFileProvider
	{
		public const string LogsFolder = "Logs";
		public const string LogFilePrefix = "game_log_";
		
		private const string LogFileExtension = ".log";
		private const string DateFormat = "yyyy_MM_dd_hh_mm_ss";
		
		private static readonly bool _isInitialized;
		
		public static string CurrentLogFilePath { get; private set; }

		static LoggerFileProvider()
		{
			if (_isInitialized)
			{
				return;
			}

			if (!Directory.Exists(LogsFolder))
			{
				Directory.CreateDirectory(LogsFolder);
			}
			
			RefreshLogPath();
			_isInitialized = true;
		}
		
		public static void RefreshLogPath()
		{
			string today = DateTime.Now.ToString(DateFormat);
			string path = Path.Combine(LogsFolder, $"{LogFilePrefix}{today}{LogFileExtension}");
			CurrentLogFilePath = path;
		}
	}
}