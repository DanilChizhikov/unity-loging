using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DTech.Logging.Editor
{
	internal sealed class LogViewerWindow : EditorWindow
	{
		private static readonly LogLevel[] DisplayLevels =
		{
			LogLevel.Trace,
			LogLevel.Debug,
			LogLevel.Information,
			LogLevel.Warning,
			LogLevel.Error,
			LogLevel.Critical,
		};

		private static readonly Color DefaultPillColor = new Color(0f, 0f, 0f, 0.25f);

		private sealed class RowElements
		{
			public VisualElement Stripe;
			public Label Time;
			public Label Frame;
			public Label Tag;
			public Label Message;
		}

		private LogViewerSettings _settings;

		private ListView _list;
		private Label _detailLabel;
		private Label _footerLabel;
		private Label _emptyHint;
		private ToolbarToggle _autoScrollToggle;

		private readonly Dictionary<LogLevel, ToolbarToggle> _levelToggles = new Dictionary<LogLevel, ToolbarToggle>();
		private readonly HashSet<LogLevel> _enabledLevels = new HashSet<LogLevel>(DisplayLevels);

		private readonly List<LogEntry> _snapshot = new List<LogEntry>();
		private readonly List<LogEntry> _filtered = new List<LogEntry>();

		private string _search = string.Empty;
		private string _tagFilter;
		private bool _autoScroll = true;
		private int _lastVersion = -1;

		[MenuItem("Tools/DTech/Logger/Viewer")]
		private static void Open()
		{
			LogViewerWindow window = GetWindow<LogViewerWindow>();
			window.titleContent = new GUIContent("Log Viewer");
			window.minSize = new Vector2(480f, 240f);
			window.Show();
		}

		private void OnEnable()
		{
			EditorApplication.update += OnUpdate;
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
			Undo.undoRedoPerformed += OnUndoRedo;
		}

		private void OnDisable()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.playModeStateChanged -= OnPlayModeChanged;
			Undo.undoRedoPerformed -= OnUndoRedo;
		}

		public void CreateGUI()
		{
			_settings = LogViewerSettingsProvider.GetOrCreate();
			EditorLogBuffer.SetCapacity(_settings.MaxBufferSize);
			_autoScroll = _settings.AutoScroll;

			var tree = Resources.Load<VisualTreeAsset>("LogViewerWindow");
			if (tree == null)
			{
				rootVisualElement.Add(new Label("LogViewerWindow.uxml not found in any Editor/Resources folder."));
				return;
			}

			tree.CloneTree(rootVisualElement);

			var styles = Resources.Load<StyleSheet>("LogViewerWindow");
			if (styles != null)
			{
				rootVisualElement.styleSheets.Add(styles);
			}

			BuildToolbar();
			BuildList();

			_detailLabel = rootVisualElement.Q<Label>("detail-label");
			_footerLabel = rootVisualElement.Q<Label>("footer-label");
			_emptyHint = rootVisualElement.Q<Label>("empty-hint");

			_lastVersion = -1;
			OnUpdate();
		}

		private void BuildToolbar()
		{
			rootVisualElement.Q<ToolbarButton>("btn-clear").clicked += () =>
			{
				EditorLogBuffer.Clear();
			};

			_autoScrollToggle = rootVisualElement.Q<ToolbarToggle>("tgl-autoscroll");
			_autoScrollToggle.SetValueWithoutNotify(_autoScroll);
			_autoScrollToggle.RegisterValueChangedCallback(evt => _autoScroll = evt.newValue);

			VisualElement levelHost = rootVisualElement.Q<VisualElement>("level-toggles");
			foreach (LogLevel level in DisplayLevels)
			{
				LogLevel captured = level;
				var toggle = new ToolbarToggle
				{
					text = LevelLetter(level),
					tooltip = level.ToString(),
					value = true,
				};
				toggle.AddToClassList("lvw-level-toggle");
				toggle.style.color = _settings.GetLevelColor(level);
				toggle.RegisterValueChangedCallback(evt =>
				{
					if (evt.newValue)
					{
						_enabledLevels.Add(captured);
					}
					else
					{
						_enabledLevels.Remove(captured);
					}

					RebuildFiltered(false);
				});

				_levelToggles[level] = toggle;
				levelHost.Add(toggle);
			}

			rootVisualElement.Q<ToolbarButton>("tag-menu").clicked += ShowTagMenu;

			var search = rootVisualElement.Q<ToolbarSearchField>("search-field");
			search.RegisterValueChangedCallback(evt =>
			{
				_search = evt.newValue ?? string.Empty;
				RebuildFiltered(false);
			});

			rootVisualElement.Q<ToolbarButton>("btn-colors").clicked += () =>
			{
				SettingsService.OpenProjectSettings("Project/DTech/Log Viewer");
			};
		}

		private void BuildList()
		{
			_list = rootVisualElement.Q<ListView>("list");
			_list.itemsSource = _filtered;
			_list.selectionType = SelectionType.Single;
			_list.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
			_list.makeItem = MakeRow;
			_list.bindItem = BindRow;
			_list.selectionChanged += _ => UpdateDetail();
		}

		private VisualElement MakeRow()
		{
			var row = new VisualElement();
			row.AddToClassList("lvw-row");

			var stripe = new VisualElement();
			stripe.AddToClassList("lvw-row-stripe");
			row.Add(stripe);

			var time = new Label();
			time.AddToClassList("lvw-row-time");
			row.Add(time);

			var frame = new Label();
			frame.AddToClassList("lvw-row-frame");
			row.Add(frame);

			var tag = new Label();
			tag.AddToClassList("lvw-row-tag");
			row.Add(tag);

			var message = new Label();
			message.AddToClassList("lvw-row-msg");
			row.Add(message);

			row.userData = new RowElements
			{
				Stripe = stripe,
				Time = time,
				Frame = frame,
				Tag = tag,
				Message = message,
			};
			return row;
		}

		private void BindRow(VisualElement element, int index)
		{
			if (index < 0 || index >= _filtered.Count)
			{
				return;
			}

			var refs = (RowElements)element.userData;
			LogEntry entry = _filtered[index];
			Color levelColor = _settings.GetLevelColor(entry.Level);

			refs.Stripe.style.backgroundColor = levelColor;
			refs.Time.text = entry.Time.ToString("HH:mm:ss.fff");
			refs.Frame.text = entry.Frame >= 0 ? entry.Frame.ToString() : "-";
			refs.Tag.text = entry.Tag;
			refs.Message.text = FirstLine(entry.Message);
			refs.Message.style.color = levelColor;

			refs.Tag.style.backgroundColor =
				_settings.TryGetTagColor(entry.Tag, out Color highlight) ? highlight : DefaultPillColor;
		}

		private void OnUpdate()
		{
			if (_settings == null)
			{
				_settings = LogViewerSettingsProvider.GetOrCreate();
			}

			int version = EditorLogBuffer.Version;
			if (version == _lastVersion)
			{
				return;
			}

			_lastVersion = version;
			EditorLogBuffer.Snapshot(_snapshot);
			RebuildFiltered(_autoScroll);
		}

		private void RebuildFiltered(bool autoScroll)
		{
			if (_list == null)
			{
				return;
			}

			_filtered.Clear();
			for (int i = 0; i < _snapshot.Count; i++)
			{
				LogEntry entry = _snapshot[i];
				if (Passes(entry))
				{
					_filtered.Add(entry);
				}
			}

			_list.RefreshItems();

			if (_emptyHint != null)
			{
				_emptyHint.style.display = _filtered.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
			}

			if (_footerLabel != null)
			{
				string tagInfo = string.IsNullOrEmpty(_tagFilter) ? string.Empty : $" · tag: {_tagFilter}";
				_footerLabel.text = $"{_filtered.Count} / {_snapshot.Count} shown{tagInfo}";
			}

			if (autoScroll && _filtered.Count > 0)
			{
				_list.ScrollToItem(_filtered.Count - 1);
			}
		}

		private bool Passes(in LogEntry entry)
		{
			if (!_enabledLevels.Contains(entry.Level))
			{
				return false;
			}

			if (!string.IsNullOrEmpty(_tagFilter) && !string.Equals(entry.Tag, _tagFilter, System.StringComparison.Ordinal))
			{
				return false;
			}

			if (!string.IsNullOrEmpty(_search))
			{
				bool inMessage = entry.Message != null &&
					entry.Message.IndexOf(_search, System.StringComparison.OrdinalIgnoreCase) >= 0;
				bool inTag = entry.Tag != null &&
					entry.Tag.IndexOf(_search, System.StringComparison.OrdinalIgnoreCase) >= 0;
				if (!inMessage && !inTag)
				{
					return false;
				}
			}

			return true;
		}

		private void ShowTagMenu()
		{
			var tags = new SortedSet<string>(System.StringComparer.Ordinal);
			for (int i = 0; i < _snapshot.Count; i++)
			{
				string tag = _snapshot[i].Tag;
				if (!string.IsNullOrEmpty(tag))
				{
					tags.Add(tag);
				}
			}

			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("All Tags"), string.IsNullOrEmpty(_tagFilter), () =>
			{
				_tagFilter = null;
				RebuildFiltered(false);
			});
			menu.AddSeparator(string.Empty);

			foreach (string tag in tags)
			{
				string captured = tag;
				menu.AddItem(new GUIContent(tag), string.Equals(_tagFilter, tag, System.StringComparison.Ordinal), () =>
				{
					_tagFilter = captured;
					RebuildFiltered(false);
				});
			}

			menu.ShowAsContext();
		}

		private void UpdateDetail()
		{
			if (_detailLabel == null)
			{
				return;
			}

			int index = _list.selectedIndex;
			if (index < 0 || index >= _filtered.Count)
			{
				_detailLabel.text = string.Empty;
				return;
			}

			LogEntry entry = _filtered[index];
			var sb = new StringBuilder();
			sb.Append(entry.Level.ToString().ToUpperInvariant()).Append("  ").Append(entry.Tag);
			if (!string.IsNullOrEmpty(entry.StateName))
			{
				sb.Append("  <").Append(entry.StateName).Append('>');
			}

			sb.Append('\n').Append(entry.Time.ToString("yyyy-MM-dd HH:mm:ss.fff"));
			if (entry.Frame >= 0)
			{
				sb.Append("  frame ").Append(entry.Frame);
			}

			sb.Append("  thread ").Append(entry.ThreadId);

			if (!string.IsNullOrEmpty(entry.Scopes))
			{
				sb.Append('\n').Append(entry.Scopes);
			}

			sb.Append("\n\n").Append(entry.Message);

			if (entry.HasException)
			{
				sb.Append("\n\n").Append(entry.Exception);
			}

			_detailLabel.text = sb.ToString();
		}

		private void OnFocus()
		{
			if (_settings == null || _list == null)
			{
				return;
			}

			EditorLogBuffer.SetCapacity(_settings.MaxBufferSize);
			_list.RefreshItems();
		}

		private void OnUndoRedo()
		{
			if (_list != null)
			{
				_list.RefreshItems();
			}
		}

		private void OnPlayModeChanged(PlayModeStateChange change)
		{
			if (change == PlayModeStateChange.ExitingEditMode && _settings != null && _settings.ClearOnPlay)
			{
				EditorLogBuffer.Clear();
			}
		}

		private static string LevelLetter(LogLevel level)
		{
			switch (level)
			{
				case LogLevel.Trace: return "T";
				case LogLevel.Debug: return "D";
				case LogLevel.Information: return "I";
				case LogLevel.Warning: return "W";
				case LogLevel.Error: return "E";
				case LogLevel.Critical: return "C";
				default: return "?";
			}
		}

		private static string FirstLine(string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				return string.Empty;
			}

			int newline = message.IndexOf('\n');
			return newline < 0 ? message : message.Substring(0, newline);
		}
	}
}
