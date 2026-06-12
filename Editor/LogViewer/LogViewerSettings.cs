using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DTech.Logging.Editor
{
	internal sealed class LogViewerSettings : ScriptableObject
	{
		[Header("Tag highlighting")]
		[SerializeField] private List<TagColorRule> _tagRules = new List<TagColorRule>();

		[Header("Per-level text color")]
		[SerializeField] private Color _traceColor = new Color(0.60f, 0.60f, 0.60f, 1f);
		[SerializeField] private Color _debugColor = new Color(0.55f, 0.80f, 1.00f, 1f);
		[SerializeField] private Color _informationColor = new Color(0.85f, 0.85f, 0.85f, 1f);
		[SerializeField] private Color _warningColor = new Color(1.00f, 0.80f, 0.25f, 1f);
		[SerializeField] private Color _errorColor = new Color(1.00f, 0.45f, 0.45f, 1f);
		[SerializeField] private Color _criticalColor = new Color(1.00f, 0.30f, 0.65f, 1f);

		[Header("Display")]
		[SerializeField] private int _maxBufferSize = EditorLogBuffer.DefaultCapacity;
		[SerializeField] private bool _autoScroll = true;
		[SerializeField] private bool _useMonospaceFont;
		[SerializeField] private bool _clearOnPlay = true;

		public int MaxBufferSize => Mathf.Max(EditorLogBuffer.MinCapacity, _maxBufferSize);
		public bool AutoScroll => _autoScroll;
		public bool UseMonospaceFont => _useMonospaceFont;
		public bool ClearOnPlay => _clearOnPlay;

		internal void SetAutoScroll(bool value)
		{
			if (_autoScroll == value)
			{
				return;
			}

			_autoScroll = value;
			EditorUtility.SetDirty(this);
		}

		public Color GetLevelColor(LogLevel level)
		{
			switch (level)
			{
				case LogLevel.Trace: return _traceColor;
				case LogLevel.Debug: return _debugColor;
				case LogLevel.Information: return _informationColor;
				case LogLevel.Warning: return _warningColor;
				case LogLevel.Error: return _errorColor;
				case LogLevel.Critical: return _criticalColor;
				default: return _informationColor;
			}
		}

		public bool TryGetTagColor(string tag, out Color color)
		{
			if (!string.IsNullOrEmpty(tag))
			{
				for (int i = 0; i < _tagRules.Count; i++)
				{
					TagColorRule rule = _tagRules[i];
					if (rule != null && rule.Enabled && string.Equals(rule.Tag, tag, StringComparison.Ordinal))
					{
						color = rule.Color;
						return true;
					}
				}
			}

			color = default;
			return false;
		}
	}
}
