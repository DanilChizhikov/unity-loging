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

		private static LoggingHandlerThreadSafe _loggingHandlerThreadSafe;
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

			EditorApplication.playModeStateChanged += PlayModeStateChangedHandler;
			_isInitialized = true;
			Debug.Log($"[{nameof(UnityLogWriter)}] Initialized");
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void TryStartLogging()
		{
			if (_loggingHandlerThreadSafe != null)
			{
				return;
			}

			try
			{
				string today = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
				string path = Path.Combine(LogsFolder, $"{LogFilePrefix}{today}{LogFileExtension}");
				_currentLogFilePath = path;

				_loggingHandlerThreadSafe = new LoggingHandlerThreadSafe();
				_loggingHandlerThreadSafe.OnLogMessageReceivedThreaded += LogMessageReceivedThreadedHandler;
				Debug.Log($"[{nameof(UnityLogWriter)}] Logging started");
			}
			catch (Exception e)
			{
				Debug.LogError($"[{nameof(UnityLogWriter)}] Failed to start logging: {e.Message}");
			}
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
			if (state != PlayModeStateChange.EnteredEditMode)
			{
				return;
			}

			if (_loggingHandlerThreadSafe != null)
			{
				_loggingHandlerThreadSafe.OnLogMessageReceivedThreaded -= LogMessageReceivedThreadedHandler;
				_loggingHandlerThreadSafe.Dispose();
				_loggingHandlerThreadSafe = null;
			}
		}

		private static void LogMessageReceivedThreadedHandler(string condition, string stackTrace, LogType type)
		{
			WriteLog(type.ToString(), condition, stackTrace);
		}
	}
}