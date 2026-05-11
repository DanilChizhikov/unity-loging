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
		private const int PauseFlushTimeoutMs = 200;

		private readonly struct LogCommand
		{
			public readonly string Line;
			public readonly bool IsFlush;
			public readonly ManualResetEventSlim Signal;

			private LogCommand(string line, bool isFlush, ManualResetEventSlim signal)
			{
				Line = line;
				IsFlush = isFlush;
				Signal = signal;
			}

			public static LogCommand FromLine(string line) => new(line, false, null);
			public static LogCommand Flush() => new(null, true, null);
			public static LogCommand FlushWithSignal(ManualResetEventSlim signal) => new(null, true, signal);
		}

		private static Lazy<BackgroundFileLogSink> _lazy = CreateLazy();
		private static BackgroundFileLogSinkLifecycle _lifecycleHook;
		private static int _unityLifecycleAttached;

		public static BackgroundFileLogSink Instance => _lazy.Value;

		public static void AttachUnityLifecycle()
		{
			if (Interlocked.Exchange(ref _unityLifecycleAttached, 1) == 1)
			{
				return;
			}

			BackgroundFileLogSink instance = _lazy.Value;
			Application.quitting += instance.OnQuitting;
			Application.focusChanged += instance.OnFocusChanged;
			EnsureLifecycleHook();
		}

		// Domain reload may be disabled in the editor's Enter Play Mode settings;
		// in that case static fields survive between play sessions, leaving us with
		// a stale worker thread, a dangling StreamWriter and a destroyed lifecycle
		// GameObject. Reset everything before the next session bootstraps.
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetStatics()
		{
			Lazy<BackgroundFileLogSink> previous = _lazy;
			if (previous.IsValueCreated)
			{
				try { previous.Value.Shutdown(); } catch { /* ignore */ }
			}

			_lazy = CreateLazy();
			_lifecycleHook = null;
			Interlocked.Exchange(ref _unityLifecycleAttached, 0);
		}

		private static Lazy<BackgroundFileLogSink> CreateLazy() =>
			new(() => new BackgroundFileLogSink(), LazyThreadSafetyMode.ExecutionAndPublication);

		private static void EnsureLifecycleHook()
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

		public void FlushAndWait(int timeoutMs)
		{
			if (_queue.IsAddingCompleted)
			{
				return;
			}

			using var signal = new ManualResetEventSlim(false);
			if (!_queue.TryAdd(LogCommand.FlushWithSignal(signal)))
			{
				Interlocked.Increment(ref _droppedCount);
				return;
			}

			signal.Wait(timeoutMs);
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
					finally
					{
						command.Signal?.Set();
					}
				}

				try
				{
					ReportDroppedIfAny();
				}
				catch (Exception ex)
				{
					Debug.LogWarning($"[{nameof(BackgroundFileLogSink)}] dropped report failed: {ex.Message}");
				}
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
			Shutdown();
		}

		internal void Shutdown()
		{
			try { _queue.CompleteAdding(); } catch { /* ignore */ }
			try { _worker.Join(JoinTimeoutMs); } catch { /* ignore */ }
		}

		private void OnFocusChanged(bool hasFocus)
		{
			if (hasFocus)
			{
				return;
			}

			FlushAndWait(PauseFlushTimeoutMs);
		}

		private sealed class BackgroundFileLogSinkLifecycle : MonoBehaviour
		{
			private void OnApplicationPause(bool pauseStatus)
			{
				if (!pauseStatus)
				{
					return;
				}

				Instance.FlushAndWait(PauseFlushTimeoutMs);
			}
		}
	}
}
