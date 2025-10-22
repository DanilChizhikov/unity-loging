using UnityEditor;
using UnityEngine;

namespace DTech.Logging.Editor
{
	internal static class LoggerSettingsCreator
	{
		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			string[] assets = AssetDatabase.FindAssets($"t:{nameof(LoggerSettings)}");
			if (assets.Length == 0)
			{
				var settings = ScriptableObject.CreateInstance<LoggerSettings>();
				AssetDatabase.CreateAsset(settings, $"Assets/Resources/{nameof(LoggerSettings)}.asset");
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}
	}
}