using System;

namespace DTech.Logging.Attributes
{
	/// <summary>
	/// Marks an <see cref="ILogger"/> implementation that should be auto-discovered and instantiated
	/// for every <see cref="Logger"/> alongside the built-in Unity and file sinks. The marked type
	/// must be a non-abstract class implementing <see cref="ILogger"/> and must declare a public or
	/// non-public constructor with the signature <c>.ctor(string tag)</c>.
	/// </summary>
	/// <remarks>
	/// Scope context (the chain produced by <see cref="ILogger.BeginScope(string)"/>) is currently
	/// forwarded only to internal sinks (UnityLogger, FileLogger). Custom providers registered via
	/// this attribute receive the original message and arguments but do not receive the rendered
	/// scope string. If your provider needs scopes, surface them in the body yourself or open an
	/// issue to extend the contract.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultLoggerProviderAttribute : Attribute
	{
	}
}
