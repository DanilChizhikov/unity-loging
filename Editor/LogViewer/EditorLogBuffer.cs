using System;
using System.Collections.Generic;
using System.Threading;

namespace DTech.Logging.Editor
{
	internal readonly struct LogEntry
	{
		public readonly LogLevel Level;
		public readonly string Tag;
		public readonly string Scopes;
		public readonly string StateName;
		public readonly string Message;
		public readonly string Exception;
		public readonly DateTime Time;
		public readonly int Frame;
		public readonly int ThreadId;

		public LogEntry(
			LogLevel level,
			string tag,
			string scopes,
			string stateName,
			string message,
			string exception,
			DateTime time,
			int frame,
			int threadId)
		{
			Level = level;
			Tag = tag ?? string.Empty;
			Scopes = scopes ?? string.Empty;
			StateName = stateName ?? string.Empty;
			Message = message ?? string.Empty;
			Exception = exception;
			Time = time;
			Frame = frame;
			ThreadId = threadId;
		}

		public bool HasException => !string.IsNullOrEmpty(Exception);
	}

	internal static class EditorLogBuffer
	{
		public const int DefaultCapacity = 10000;
		public const int MinCapacity = 16;

		private static readonly object _gate = new object();

		private static LogEntry[] _ring = new LogEntry[DefaultCapacity];
		private static int _head;
		private static int _count;
		private static int _version;

		public static int Version => Volatile.Read(ref _version);

		public static int Count
		{
			get
			{
				lock (_gate)
				{
					return _count;
				}
			}
		}

		public static void Add(in LogEntry entry)
		{
			lock (_gate)
			{
				int capacity = _ring.Length;
				if (_count < capacity)
				{
					int tail = (_head + _count) % capacity;
					_ring[tail] = entry;
					_count++;
				}
				else
				{
					_ring[_head] = entry;
					_head = (_head + 1) % capacity;
				}

				Bump();
			}
		}

		public static void Clear()
		{
			lock (_gate)
			{
				Array.Clear(_ring, 0, _ring.Length);
				_head = 0;
				_count = 0;
				Bump();
			}
		}

		public static void Snapshot(List<LogEntry> dest)
		{
			if (dest == null)
			{
				throw new ArgumentNullException(nameof(dest));
			}

			lock (_gate)
			{
				dest.Clear();
				if (dest.Capacity < _count)
				{
					dest.Capacity = _count;
				}

				int capacity = _ring.Length;
				for (int i = 0; i < _count; i++)
				{
					dest.Add(_ring[(_head + i) % capacity]);
				}
			}
		}

		public static void SetCapacity(int capacity)
		{
			if (capacity < MinCapacity)
			{
				capacity = MinCapacity;
			}

			lock (_gate)
			{
				if (capacity == _ring.Length)
				{
					return;
				}

				int keep = Math.Min(_count, capacity);
				var resized = new LogEntry[capacity];
				int oldCapacity = _ring.Length;
				int skip = _count - keep;
				for (int i = 0; i < keep; i++)
				{
					resized[i] = _ring[(_head + skip + i) % oldCapacity];
				}

				_ring = resized;
				_head = 0;
				_count = keep;
				Bump();
			}
		}

		private static void Bump()
		{
			Volatile.Write(ref _version, _version + 1);
		}
	}
}
