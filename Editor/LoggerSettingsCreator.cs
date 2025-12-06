using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTech.Logging.Editor
{
	internal static class LoggerSettingsCreator
	{
		private const string ResourcesFolder = "Assets/Resources";
		private const string AssetPath = ResourcesFolder + "/{0}.asset";
		
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			string settingsPath = string.Format(AssetPath, nameof(LoggerSettings));
			if (!Directory.Exists(ResourcesFolder))
			{
				Directory.CreateDirectory(ResourcesFolder);
			}

			if (HasLoggerSettings(settingsPath))
			{
				return;
			}
			
			var settings = ScriptableObject.CreateInstance<LoggerSettings>();
			AssetDatabase.CreateAsset(settings, settingsPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static bool HasLoggerSettings(string path)
		{
			var existing = AssetDatabase.LoadAssetAtPath<LoggerSettings>(path);
			return existing != null || File.Exists(path);
		}
	}
}