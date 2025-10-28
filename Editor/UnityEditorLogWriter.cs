using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTech.Logging.Editor
{
	[InitializeOnLoad]
	internal static class UnityEditorLogWriter
	{
		private static readonly bool _isInitialized;
		private static readonly LoggingHandlerThreadSafe _loggingHandlerThreadSafe;

		static UnityEditorLogWriter()
		{
			if (_isInitialized)
			{
				return;
			}
			
			LoggerFileProvider.RefreshLogPath();
			
			_loggingHandlerThreadSafe = new LoggingHandlerThreadSafe();
			_loggingHandlerThreadSafe.OnLogMessageReceivedThreaded += LogMessageReceivedThreadedHandler;

			EditorApplication.playModeStateChanged += PlayModeStateChangedHandler;
			
			_isInitialized = true;
		}

		private static void WriteLog(string level, string condition, string stackTrace)
		{
			try
			{
				using var writer = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true)
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
				LoggerFileProvider.RefreshLogPath();
			}
		}

		private static void LogMessageReceivedThreadedHandler(string condition, string stackTrace, LogType type)
		{
			if (Application.isPlaying && LogEditorUtility.IsEditorLogWriterEnabled)
			{
				WriteLog(type.ToString(), condition.CleanColorTags(), stackTrace);
			}
		}
	}
}