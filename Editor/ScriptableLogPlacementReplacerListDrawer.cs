using System;
using System.Collections.Generic;
using System.Reflection;
using DTech.Logging.Placements;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DTech.Logging.Editor
{
	internal sealed class ScriptableLogPlacementReplacerListDrawer : IDisposable
	{
		private const string PlacementReplacersPropertyName = "_placementReplacers";

		private static readonly FieldInfo _placementReplacersFieldInfo =
			typeof(LoggerSettings).GetField(PlacementReplacersPropertyName, BindingFlags.Instance | BindingFlags.NonPublic);
		
		private readonly LoggerSettings _settings;
		private readonly List<ScriptableLogPlacementReplacer> _replacers;
		private readonly ReorderableList _reorderableList;

		public ScriptableLogPlacementReplacerListDrawer(LoggerSettings settings)
		{
			_settings = settings;
			_replacers = new List<ScriptableLogPlacementReplacer>((IReadOnlyList<ScriptableLogPlacementReplacer>)_settings.PlacementReplacers);
			_reorderableList = new ReorderableList(_replacers, typeof(ScriptableLogPlacementReplacer), true, true, true, true);
			_reorderableList.drawHeaderCallback += DrawHeaderHandler;
			_reorderableList.onAddCallback += AddClickHandler;
			_reorderableList.onRemoveCallback += RemoveClickHandler;
			_reorderableList.onChangedCallback += ChangedHandler;
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
			_reorderableList.onAddCallback -= AddClickHandler;
			_reorderableList.onRemoveCallback -= RemoveClickHandler;
			_reorderableList.onChangedCallback -= ChangedHandler;
			_reorderableList.drawElementCallback -= DrawElementHandler;
		}
		
		private void DrawHeaderHandler(Rect rect)
		{
			EditorGUI.LabelField(rect, "Log Placement Replacers");
		}
		
		private void AddClickHandler(ReorderableList list)
		{
			_replacers.Add(null);
		}
		
		private void RemoveClickHandler(ReorderableList list)
		{
			_replacers.RemoveAt(list.index);
		}
		
		private void ChangedHandler(ReorderableList list)
		{
			_placementReplacersFieldInfo.SetValue(_settings, _replacers.ToArray());
			EditorUtility.SetDirty(_settings);
		}
		
		private void DrawElementHandler(Rect rect, int index, bool isActive, bool isFocused)
		{
			ScriptableLogPlacementReplacer element = _replacers[index];
			rect.height -= EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.ObjectField(rect, element, typeof(ScriptableLogPlacementReplacer), false);
		}
	}
}