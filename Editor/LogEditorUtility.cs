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
		public static string LogFolderPath => Path.Combine(Application.dataPath.Replace("Assets", ""), LoggerFileProvider.LogsFolder);
		
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
		
	}
}