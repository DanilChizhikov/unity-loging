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

		public static string CurrentLogFilePath { get; private set; }

		static LoggerFileProvider()
		{
			string basePath = ResolveBasePath(GetPreferredBasePath());
			RefreshLogPath(basePath);
		}

		public static void RefreshLogPath(string basePath = null)
		{
			if (string.IsNullOrEmpty(basePath))
			{
				basePath = ResolveBasePath(GetPreferredBasePath());
			}

			string today = DateTime.Now.ToString(DateFormat);
			CurrentLogFilePath = Path.Combine(basePath, $"{LogFilePrefix}{today}{LogFileExtension}");
		}

		private static string GetPreferredBasePath()
		{
			#if UNITY_EDITOR
			return LogsFolderName;
			#else
			return Path.Combine(Application.persistentDataPath, LogsFolderName);
			#endif
		}

		private static string ResolveBasePath(string preferred)
		{
			if (TryEnsureDirectory(preferred))
			{
				return preferred;
			}

			string fallback = Path.Combine(Application.temporaryCachePath, LogsFolderName);
			if (TryEnsureDirectory(fallback))
			{
				Debug.LogWarning($"[{nameof(LoggerFileProvider)}] Falling back to temporaryCachePath: {fallback}");
				return fallback;
			}

			Debug.LogError($"[{nameof(LoggerFileProvider)}] Failed to create logs folder at '{preferred}' or fallback '{fallback}'.");
			return preferred;
		}

		private static bool TryEnsureDirectory(string path)
		{
			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}

				return Directory.Exists(path);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}