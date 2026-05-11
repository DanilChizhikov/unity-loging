using System;
using System.Collections.Generic;
#if !ENABLE_IL2CPP
using System.Linq.Expressions;
#endif
using System.Reflection;
using System.Threading;
using DTech.Logging.Attributes;
using UnityEngine;

namespace DTech.Logging
{
	internal static class LoggerUtility
	{
		private static readonly Func<string, ILogger>[] _builtInLoggerFactories =
		{
			tag => new UnityLogger(tag),
			tag => new FileLogger(tag),
		};

		private static readonly Lazy<Func<string, ILogger>[]> _cachedLoggerFactories =
			new(BuildLoggerFactories, true);

		private static int _mainThreadId;
		private static volatile bool _prewarmCompleted;

		internal static bool IsMainThreadKnown => _mainThreadId != 0;
		internal static bool IsOnMainThread => _mainThreadId != 0 && Thread.CurrentThread.ManagedThreadId == _mainThreadId;
		internal static bool PrewarmCompleted => _prewarmCompleted;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Prewarm()
		{
			_mainThreadId = Thread.CurrentThread.ManagedThreadId;

			// Force the assembly scan + factory build to happen during scene load
			// (typically behind a splash) instead of on the first log call mid-gameplay.
			_ = _cachedLoggerFactories.Value;

			// Touch Unity APIs that are main-thread-only here, while we are guaranteed
			// to run on the main thread. Without this, the first log call from a
			// background thread would force LoggerFileProvider's static ctor to read
			// Application.persistentDataPath off-main-thread (UnityException on device)
			// and Resources.Load to run off-main-thread.
			_ = LoggerSettings.Instance;
			_ = LoggerFileProvider.CurrentLogFilePath;
			_ = BackgroundFileLogSink.Instance;
			BackgroundFileLogSink.AttachUnityLifecycle();

			_prewarmCompleted = true;
		}

		public static ILogger[] GetDefaultLoggers(string tag)
		{
			Func<string, ILogger>[] factories = _cachedLoggerFactories.Value;
			var loggers = new ILogger[factories.Length];
			for (int i = 0; i < factories.Length; i++)
			{
				loggers[i] = factories[i](tag);
			}

			return loggers;
		}

		private static Func<string, ILogger>[] BuildLoggerFactories()
		{
			var factories = new List<Func<string, ILogger>>(_builtInLoggerFactories.Length + 4);
			factories.AddRange(_builtInLoggerFactories);
			var discoveredTypes = new HashSet<Type>();
			Type loggerInterfaceType = typeof(ILogger);
			
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
			{
				Assembly assembly = assemblies[assemblyIndex];
				Type[] types = GetAssemblyTypes(assembly);
				for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
				{
					Type type = types[typeIndex];
					if (type == null)
					{
						continue;
					}
					
					if (!Attribute.IsDefined(type, typeof(DefaultLoggerProviderAttribute), false))
					{
						continue;
					}

					if (!type.IsClass || type.IsAbstract)
					{
						Debug.LogWarning($"[DTech.Logging] Type '{type.FullName}' marked with [{nameof(DefaultLoggerProviderAttribute)}] must be a non-abstract class and was skipped.");
						continue;
					}

					if (!loggerInterfaceType.IsAssignableFrom(type))
					{
						Debug.LogWarning($"[DTech.Logging] Type '{type.FullName}' marked with [{nameof(DefaultLoggerProviderAttribute)}] must implement {nameof(ILogger)} and was skipped.");
						continue;
					}

					if (!discoveredTypes.Add(type))
					{
						continue;
					}

					ConstructorInfo ctor = type.GetConstructor(
						BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
						null,
						new[] { typeof(string) },
						null);

					if (ctor == null)
					{
						Debug.LogWarning($"[DTech.Logging] Type '{type.FullName}' marked with [{nameof(DefaultLoggerProviderAttribute)}] must declare a constructor with signature .ctor(string tag) and was skipped.");
						continue;
					}

					Func<string, ILogger> factory = CreateFactory(ctor);
					factories.Add(factory);
				}
			}
			
			return factories.ToArray();
		}

		private static Type[] GetAssemblyTypes(Assembly assembly)
		{
			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException ex)
			{
				return ex.Types ?? Array.Empty<Type>();
			}
			catch
			{
				return Array.Empty<Type>();
			}
		}

		private static Func<string, ILogger> CreateFactory(ConstructorInfo ctor)
		{
#if ENABLE_IL2CPP
			// Expression.Compile is not supported on IL2CPP; it falls back to
			// reflection invoke at runtime anyway. Skip the build cost.
			return tag => (ILogger)ctor.Invoke(new object[] { tag });
#else
			try
			{
				ParameterExpression tagParameter = Expression.Parameter(typeof(string), "tag");
				NewExpression newExpression = Expression.New(ctor, tagParameter);
				UnaryExpression castExpression = Expression.Convert(newExpression, typeof(ILogger));
				return Expression.Lambda<Func<string, ILogger>>(castExpression, tagParameter).Compile();
			}
			catch
			{
				return tag => (ILogger)ctor.Invoke(new object[] { tag });
			}
#endif
		}
	}
}
