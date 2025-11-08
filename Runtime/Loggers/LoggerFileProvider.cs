using System;
using System.IO;
using UnityEngine;

namespace DTech.Logging
{
	internal static class LoggerFileProvider
	{
		public const string LogsFolderName = "Logs";
		public const string LogFilePrefix = "game_log_";
		
		private const string LogFileExtension = ".log";
		private const string DateFormat = "yyyy_MM_dd_HH_mm_ss";

		private static readonly bool _isInitialized;

		public static string CurrentLogFilePath { get; private set; }

		static LoggerFileProvider()
		{
			#if UNITY_EDITOR
			string basePath = LogsFolderName;
			#else
            string basePath = Path.Combine(Application.persistentDataPath, LogsFolderName);
			#endif

			if (!_isInitialized)
			{
				try
				{
					if (!Directory.Exists(basePath))
					{
						Directory.CreateDirectory(basePath);
					}
				}
				catch (Exception e)
				{
					Debug.LogError($"[{nameof(LoggerFileProvider)}] Failed to create logs folder: {e}");
				}

				RefreshLogPath(basePath);
				_isInitialized = true;
			}
		}

		public static void RefreshLogPath(string basePath = null)
		{
			if (string.IsNullOrEmpty(basePath))
			{
				#if UNITY_EDITOR
				basePath = LogsFolderName;
				#else
				basePath = Path.Combine(Application.persistentDataPath, LogsFolderName);
				#endif
			}
			
			string today = DateTime.Now.ToString(DateFormat);
			CurrentLogFilePath = Path.Combine(basePath, $"{LogFilePrefix}{today}{LogFileExtension}");
		}
	}
}