using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DTech.Logging.Editor
{
	internal static class LogEditorUtility
	{
		private const string EditorLogWritingSaveKey = "DTech.Logging.Editor.LogWriting.Save";
		private const string EditorLogWritingMenuPath = "Tools/DTech/Logger/Editor Log Writing Enable";
		
		public static bool IsEditorLogWriterEnabled
		{
			get => EditorPrefs.GetBool(EditorLogWritingSaveKey, true);
			private set => EditorPrefs.SetBool(EditorLogWritingSaveKey, value);
		}
		
		private static string LogFolderPath => Path.Combine(Application.dataPath.Replace("Assets", ""), LoggerFileProvider.LogsFolderName);
		
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			Menu.SetChecked(EditorLogWritingMenuPath, IsEditorLogWriterEnabled);
		}
		
		[MenuItem("Tools/DTech/Logger/Open Logs Folder")]
		private static void OpenLogsFolder()
		{
			var path = LogFolderPath;
			if (Directory.Exists(path))
			{
				switch (Application.platform)
				{
					case RuntimePlatform.WindowsPlayer:
					case RuntimePlatform.WindowsEditor:
					{
						Process.Start("explorer.exe", path.Replace("/", "\\"));
					} break;

					case RuntimePlatform.OSXPlayer:
					case RuntimePlatform.OSXEditor:
					{
						Process.Start("open", path);
					} break;

					case RuntimePlatform.LinuxPlayer:
					case RuntimePlatform.LinuxEditor:
					{
						Process.Start("xdg-open", path);
					} break;

					default:
					{
						Debug.LogError("Платформа не поддерживается для открытия папки");
					} break;
				}
			}
			else
			{
				Debug.LogError($"Logs folder not found: {path}");
			}
		}
		
		[MenuItem("Tools/DTech/Logger/Remove All Logs")]
		private static void RemoveAllLogs()
		{
			var path = LogFolderPath;
			if (Directory.Exists(path))
			{
				IEnumerable<string> files = Directory.EnumerateFiles(path);
				foreach (string file in files)
				{
					if (file.Contains(LoggerFileProvider.LogFilePrefix))
					{
						File.Delete(file);
					}
				}
			}
		}
		
		[MenuItem(EditorLogWritingMenuPath)]
		private static void ToggleEditorLogWriting()
		{
			IsEditorLogWriterEnabled = !IsEditorLogWriterEnabled;
			Menu.SetChecked(EditorLogWritingMenuPath, IsEditorLogWriterEnabled);
		}
	}
}