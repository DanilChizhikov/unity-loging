using System.IO;
using UnityEditor;
using UnityEngine;

namespace DTech.Logging.Editor
{
	internal static class LogViewerSettingsProvider
	{
		private const string Folder = "Assets/Logging/Editor";
		private const string AssetPath = Folder + "/LogViewerSettings.asset";
		private const string SettingsMenuPath = "Project/DTech/Log Viewer";

		private static LogViewerSettings _cached;

		[InitializeOnLoadMethod]
		private static void EnsureExists()
		{
			GetOrCreate();
		}

		public static LogViewerSettings GetOrCreate()
		{
			if (_cached != null)
			{
				return _cached;
			}

			_cached = AssetDatabase.LoadAssetAtPath<LogViewerSettings>(AssetPath);
			if (_cached != null)
			{
				return _cached;
			}

			if (!Directory.Exists(Folder))
			{
				Directory.CreateDirectory(Folder);
			}

			_cached = ScriptableObject.CreateInstance<LogViewerSettings>();
			AssetDatabase.CreateAsset(_cached, AssetPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			return _cached;
		}

		public static void Ping()
		{
			LogViewerSettings settings = GetOrCreate();
			Selection.activeObject = settings;
			EditorGUIUtility.PingObject(settings);
		}

		[SettingsProvider]
		private static SettingsProvider CreateProvider()
		{
			LogViewerSettings settings = GetOrCreate();
			var provider = AssetSettingsProvider.CreateProviderFromObject(SettingsMenuPath, settings);
			provider.keywords = new[] { "log", "logger", "viewer", "logcat", "tag", "color", "highlight", "dtech" };
			return provider;
		}
	}
}
