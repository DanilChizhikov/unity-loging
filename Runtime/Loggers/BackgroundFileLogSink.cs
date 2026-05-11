using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEngine;

namespace DTech.Logging
{
	internal sealed class BackgroundFileLogSink
	{
		private const int QueueCapacity = 16384;
		private const int JoinTimeoutMs = 2000;
		private const int DropReportInterval = 256;

		private readonly struct LogCommand
		{
			public readonly string Line;
			public readonly bool IsFlush;

			private LogCommand(string line, bool isFlush)
			{
				Line = line;
				IsFlush = isFlush;
			}

			public static LogCommand FromLine(string line) => new(line, false);
			public static LogCommand Flush() => new(null, true);
		}

		private static readonly Lazy<BackgroundFileLogSink> _lazy =
			new(() => new BackgroundFileLogSink(), LazyThreadSafetyMode.ExecutionAndPublication);

		private static BackgroundFileLogSinkLifecycle _lifecycleHook;

		public static BackgroundFileLogSink Instance => _lazy.Value;

		public static void EnsureLifecycleHook()
		{
			if (_lifecycleHook != null)
			{
				return;
			}

			var go = new GameObject($"~{nameof(BackgroundFileLogSink)}Lifecycle")
			{
				hideFlags = HideFlags.HideAndDontSave,
			};
			UnityEngine.Object.DontDestroyOnLoad(go);
			_lifecycleHook = go.AddComponent<BackgroundFileLogSinkLifecycle>();
		}

		private readonly BlockingCollection<LogCommand> _queue;
		private readonly Thread _worker;

		private string _currentPath;
		private StreamWriter _writer;
		private long _droppedCount;
		private int _writesSinceDropReport;

		private BackgroundFileLogSink()
		{
			_queue = new BlockingCollection<LogCommand>(new ConcurrentQueue<LogCommand>(), QueueCapacity);
			_worker = new Thread(WorkerLoop)
			{
				IsBackground = true,
				Name = "DTech.Logging.FileSink",
			};
			_worker.Start();

			Application.quitting += OnQuitting;
			Application.focusChanged += OnFocusChanged;
		}

		public void Enqueue(string line)
		{
			if (line == null || _queue.IsAddingCompleted)
			{
				return;
			}

			if (!_queue.TryAdd(LogCommand.FromLine(line)))
			{
				Interlocked.Increment(ref _droppedCount);
			}
		}

		public void RequestFlush()
		{
			if (_queue.IsAddingCompleted)
			{
				return;
			}

			if (!_queue.TryAdd(LogCommand.Flush()))
			{
				Interlocked.Increment(ref _droppedCount);
			}
		}

		private void WorkerLoop()
		{
			try
			{
				foreach (LogCommand command in _queue.GetConsumingEnumerable())
				{
					try
					{
						if (command.IsFlush)
						{
							_writer?.Flush();
							continue;
						}

						EnsureWriter(LoggerFileProvider.CurrentLogFilePath);
						_writer?.WriteLine(command.Line);

						if (++_writesSinceDropReport >= DropReportInterval)
						{
							_writesSinceDropReport = 0;
							ReportDroppedIfAny();
						}
					}
					catch (Exception ex)
					{
						Debug.LogWarning($"[{nameof(BackgroundFileLogSink)}] write failed: {ex.Message}");
					}
				}

				try { ReportDroppedIfAny(); } catch { /* ignore */ }
			}
			finally
			{
				try { _writer?.Flush(); } catch { /* ignore */ }
				try { _writer?.Dispose(); } catch { /* ignore */ }
			}
		}

		private void ReportDroppedIfAny()
		{
			long dropped = Interlocked.Exchange(ref _droppedCount, 0);
			if (dropped <= 0 || _writer == null)
			{
				return;
			}

			_writer.WriteLine($"[{nameof(BackgroundFileLogSink)}] dropped {dropped} entries");
		}

		private void EnsureWriter(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			if (_writer != null && string.Equals(_currentPath, path, StringComparison.Ordinal))
			{
				return;
			}

			try { _writer?.Flush(); } catch { /* ignore */ }
			try { _writer?.Dispose(); } catch { /* ignore */ }

			_writer = new StreamWriter(path, append: true) { AutoFlush = true };
			_currentPath = path;
		}

		private void OnQuitting()
		{
			_queue.CompleteAdding();
			try { _worker.Join(JoinTimeoutMs); } catch { /* ignore */ }
		}

		private void OnFocusChanged(bool hasFocus)
		{
			if (hasFocus)
			{
				return;
			}

			RequestFlush();
		}

		private sealed class BackgroundFileLogSinkLifecycle : MonoBehaviour
		{
			private void OnApplicationPause(bool pauseStatus)
			{
				if (!pauseStatus)
				{
					return;
				}

				Instance.RequestFlush();
			}
		}
	}
}
