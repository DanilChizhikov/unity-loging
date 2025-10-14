using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTech.Logging.Editor
{
	[InitializeOnLoad]
	internal static class UnityLogWriter
	{
		private const string LogsFolder = "Logs";
		private const string LogFilePrefix = "game_log_";
		private const string LogFileExtension = ".log";

		private static readonly bool _isInitialized;
		private static readonly LoggingHandlerThreadSafe _loggingHandlerThreadSafe;
		
		private static string _currentLogFilePath;

		static UnityLogWriter()
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
			
			_loggingHandlerThreadSafe = new LoggingHandlerThreadSafe();
			_loggingHandlerThreadSafe.OnLogMessageReceivedThreaded += LogMessageReceivedThreadedHandler;

			EditorApplication.playModeStateChanged += PlayModeStateChangedHandler;
			
			_isInitialized = true;
		}

		private static void RefreshLogPath()
		{
			string today = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
			string path = Path.Combine(LogsFolder, $"{LogFilePrefix}{today}{LogFileExtension}");
			_currentLogFilePath = path;
		}

		private static void WriteLog(string level, string condition, string stackTrace)
		{
			try
			{
				using var writer = new StreamWriter(_currentLogFilePath, true)
				{
					AutoFlush = true,
				};

				writer.WriteLine($"[{level}] {condition}");
				writer.WriteLine($"{stackTrace}");
			}
			catch (Exception exception)
			{
				Debug.LogError($"Failed to write to log file: {exception.Message}");
			}
		}

		private static void PlayModeStateChangedHandler(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredEditMode)
			{
				RefreshLogPath();
			}
		}

		private static void LogMessageReceivedThreadedHandler(string condition, string stackTrace, LogType type)
		{
			if (Application.isPlaying)
			{
				WriteLog(type.ToString(), condition.CleanColorTags(), stackTrace);
			}
		}
	}
}