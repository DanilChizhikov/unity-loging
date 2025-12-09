using UnityEditor;
using UnityEngine;

namespace DTech.Logging.Editor
{
	internal sealed class LogSettingsEditorWindow : EditorWindow
	{
		private UnityEditor.Editor _settingsEditor;
		private Vector2 _scrollPosition;
		
		[MenuItem("Tools/DTech/Logger/Settings")]
		private static void ShowWindow()
		{
			LogSettingsEditorWindow window = GetWindow<LogSettingsEditorWindow>("Log Settings");
			window.Show();
		}
		
		private void OnGUI()
		{
			_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
			_settingsEditor.OnInspectorGUI();
			EditorGUILayout.EndScrollView();
		}

		private void OnEnable()
		{
			LoggerSettings settings = LoggerSettings.Instance;
			_settingsEditor = UnityEditor.Editor.CreateEditor(settings);
		}
		
		private void OnDisable()
		{
			_settingsEditor = null;
		}
	}
}