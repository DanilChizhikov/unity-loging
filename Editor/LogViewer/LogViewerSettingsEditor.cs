using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DTech.Logging.Editor
{
	[CustomEditor(typeof(LogViewerSettings))]
	internal sealed class LogViewerSettingsEditor : UnityEditor.Editor
	{
		private const float ToggleWidth = 18f;
		private const float ColorWidth = 80f;
		private const float Pad = 4f;

		private SerializedProperty _tagRules;
		private ReorderableList _list;

		private void OnEnable()
		{
			_tagRules = serializedObject.FindProperty("_tagRules");
			_list = new ReorderableList(serializedObject, _tagRules, true, true, true, true);
			_list.drawHeaderCallback += DrawHeader;
			_list.drawElementCallback += DrawElement;
			_list.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		private void OnDisable()
		{
			if (_list != null)
			{
				_list.drawHeaderCallback -= DrawHeader;
				_list.drawElementCallback -= DrawElement;
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.LabelField("Tag Highlighting", EditorStyles.boldLabel);
			_list.DoLayoutList();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Per-Level Text Color", EditorStyles.boldLabel);
			DrawProperty("_traceColor", "Trace");
			DrawProperty("_debugColor", "Debug");
			DrawProperty("_informationColor", "Information");
			DrawProperty("_warningColor", "Warning");
			DrawProperty("_errorColor", "Error");
			DrawProperty("_criticalColor", "Critical");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Display", EditorStyles.boldLabel);
			DrawProperty("_maxBufferSize", "Max Buffer Size");
			DrawProperty("_autoScroll", "Auto Scroll");
			DrawProperty("_useMonospaceFont", "Monospace Font");
			DrawProperty("_clearOnPlay", "Clear On Play");

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawProperty(string propertyName, string label)
		{
			SerializedProperty property = serializedObject.FindProperty(propertyName);
			if (property != null)
			{
				EditorGUILayout.PropertyField(property, new GUIContent(label));
			}
		}

		private void DrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Tag → Highlight Color");
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty element = _tagRules.GetArrayElementAtIndex(index);
			SerializedProperty enabled = element.FindPropertyRelative("_enabled");
			SerializedProperty tag = element.FindPropertyRelative("_tag");
			SerializedProperty color = element.FindPropertyRelative("_color");

			rect.y += EditorGUIUtility.standardVerticalSpacing * 0.5f;
			rect.height = EditorGUIUtility.singleLineHeight;

			var toggleRect = new Rect(rect.x, rect.y, ToggleWidth, rect.height);
			var colorRect = new Rect(rect.xMax - ColorWidth, rect.y, ColorWidth, rect.height);
			var tagRect = new Rect(
				toggleRect.xMax + Pad,
				rect.y,
				colorRect.x - toggleRect.xMax - Pad * 2f,
				rect.height);

			enabled.boolValue = EditorGUI.Toggle(toggleRect, enabled.boolValue);

			using (new EditorGUI.DisabledScope(!enabled.boolValue))
			{
				tag.stringValue = EditorGUI.TextField(tagRect, tag.stringValue);
				color.colorValue = EditorGUI.ColorField(colorRect, color.colorValue);
			}
		}
	}
}
