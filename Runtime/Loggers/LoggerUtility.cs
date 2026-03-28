using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
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
		}
	}
}
