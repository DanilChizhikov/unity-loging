using UnityEditor;
using UnityEngine;

namespace DTech.Logging.Editor
{
	[CustomEditor(typeof(LoggerSettings))]
	internal sealed class LogSettingsEditor : UnityEditor.Editor
	{
		private const string IsTraceEnabledPropertyName = "_isTraceEnabled";
		private const string IsDebugEnabledPropertyName = "_isDebugEnabled";
		private const string IsInformationEnabledPropertyName = "_isInformationEnabled";
		private const string IsWarningEnabledPropertyName = "_isWarningEnabled";
		private const string IsErrorEnabledPropertyName = "_isErrorEnabled";
		private const string IsCriticalEnabledPropertyName = "_isCriticalEnabled";
		private const string IsFileLoggingEnabledPropertyName = "_isFileLoggingEnabled";
		private const string ConsoleFormatStringPropertyName = "_consoleFormatString";
		private const string FileFormatStringPropertyName = "_fileFormatString";
		
		private LoggerSettings _settings;
		private SerializedProperty _isTraceEnabled;
		private SerializedProperty _isDebugEnabled;
		private SerializedProperty _isInformationEnabled;
		private SerializedProperty _isWarningEnabled;
		private SerializedProperty _isErrorEnabled;
		private SerializedProperty _isCriticalEnabled;
		private SerializedProperty _isFileLoggingEnabled;
		private SerializedProperty _consoleFormatString;
		private SerializedProperty _fileFormatString;
		private ScriptableLogPlacementReplacerListDrawer _listDrawer;

		public override void OnInspectorGUI()
		{
			if (_settings == null)
			{
				return;
			}
			
			EditorGUILayout.LabelField("Log Release Settings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_isTraceEnabled);
			EditorGUILayout.PropertyField(_isDebugEnabled);
			EditorGUILayout.PropertyField(_isInformationEnabled);
			EditorGUILayout.PropertyField(_isWarningEnabled);
			EditorGUILayout.PropertyField(_isErrorEnabled);
			EditorGUILayout.PropertyField(_isCriticalEnabled);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Log File Settings", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(_isFileLoggingEnabled);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Log Format Settings", EditorStyles.boldLabel);
			using (new EditorGUI.DisabledScope(Application.isPlaying))
			{
				EditorGUILayout.PropertyField(_consoleFormatString);
				EditorGUILayout.PropertyField(_fileFormatString);
				_listDrawer.Draw();
			}
		}
		
		private void OnEnable()
		{
			_settings = target as LoggerSettings;
			_isTraceEnabled = serializedObject.FindProperty(IsTraceEnabledPropertyName);
			_isDebugEnabled = serializedObject.FindProperty(IsDebugEnabledPropertyName);
			_isInformationEnabled = serializedObject.FindProperty(IsInformationEnabledPropertyName);
			_isWarningEnabled = serializedObject.FindProperty(IsWarningEnabledPropertyName);
			_isErrorEnabled = serializedObject.FindProperty(IsErrorEnabledPropertyName);
			_isCriticalEnabled = serializedObject.FindProperty(IsCriticalEnabledPropertyName);
			_isFileLoggingEnabled = serializedObject.FindProperty(IsFileLoggingEnabledPropertyName);
			_consoleFormatString = serializedObject.FindProperty(ConsoleFormatStringPropertyName);
			_fileFormatString = serializedObject.FindProperty(FileFormatStringPropertyName);
			_listDrawer = new ScriptableLogPlacementReplacerListDrawer(_settings);
		}
		
		private void OnDisable()
		{
			_settings = null;
			_listDrawer.Dispose();
			_listDrawer = null;
			_isTraceEnabled = null;
			_isDebugEnabled = null;
			_isInformationEnabled = null;
			_isWarningEnabled = null;
			_isErrorEnabled = null;
			_isCriticalEnabled = null;
			_isFileLoggingEnabled = null;
			_consoleFormatString = null;
			_fileFormatString = null;
		}
	}
}