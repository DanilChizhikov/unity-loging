using System;
using DTech.Logging.Placements;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DTech.Logging.Editor
{
	internal sealed class ScriptableLogPlacementReplacerListDrawer : IDisposable
	{
		private readonly SerializedProperty _placementReplacers;
		private readonly ReorderableList _reorderableList;

		public ScriptableLogPlacementReplacerListDrawer(SerializedObject serializedObject, SerializedProperty placementReplacers)
		{
			_placementReplacers = placementReplacers;
			_reorderableList = new ReorderableList(serializedObject, _placementReplacers, true, true, true, true);
			_reorderableList.drawHeaderCallback += DrawHeaderHandler;
			_reorderableList.drawElementCallback += DrawElementHandler;
			_reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		public void Draw()
		{
			_reorderableList.displayAdd = !Application.isPlaying;
			_reorderableList.displayRemove = !Application.isPlaying;
			_reorderableList.draggable = !Application.isPlaying;
			
			_reorderableList.DoLayoutList();
		}

		public void Dispose()
		{
			_reorderableList.drawHeaderCallback -= DrawHeaderHandler;
			_reorderableList.drawElementCallback -= DrawElementHandler;
		}
		
		private void DrawHeaderHandler(Rect rect)
		{
			EditorGUI.LabelField(rect, "Log Placement Replacers");
		}
		
		private void DrawElementHandler(Rect rect, int index, bool isActive, bool isFocused)
		{
			SerializedProperty elementProperty = _placementReplacers.GetArrayElementAtIndex(index);
			rect.height -= EditorGUIUtility.standardVerticalSpacing;
			UnityEngine.Object value = EditorGUI.ObjectField(
				rect,
				elementProperty.objectReferenceValue,
				typeof(ScriptableLogPlacementReplacer),
				false);
			elementProperty.objectReferenceValue = value;
		}
	}
}
