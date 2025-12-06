using System;
using NUnit.Framework;

namespace DTech.Logging.Tests
{
	[TestFixture]
	internal sealed class LoggerExtensionsTests
	{
		[Test]
		public void BeginScope_FormatsMessage_AndPassesToILogger()
		{
			var logger = new CapturingLogger();

			using (logger.BeginScope("User {0} from {1}", "Alice", "Wonderland"))
			{
			}

			Assert.AreEqual("User Alice from Wonderland", logger.LastScopeState);
		}
		
		[Test]
		public void Log_WithException_PrependsExceptionStringToFormatArguments()
		{
			var logger = new CapturingLogger();
			var ex = new InvalidOperationException("Boom");

			logger.Log<LoggerExtensionsTests>(
				LogLevel.Error,
				ex,
				"Error occurred. Exception: {0}, Value: {1}",
				42);

			Assert.AreEqual(LogLevel.Error, logger.LastLogLevel);
			Assert.AreSame(ex, logger.LastException);
			Assert.AreEqual(nameof(LoggerExtensionsTests), logger.LastStateTypeName);

			var expected = $"Error occurred. Exception: {ex}, Value: 42";
			Assert.AreEqual(expected, logger.LastFormattedMessage);
		}
		
		[Test]
		public void Log_WithoutException_UsesMessageAndArgsOnly()
		{
			var logger = new CapturingLogger();

			logger.Log<LoggerExtensionsTests>(
				LogLevel.Information,
				"Hello {0} from {1}",
				"Alice", "Wonderland");

			Assert.AreEqual(LogLevel.Information, logger.LastLogLevel);
			Assert.IsNull(logger.LastException);
			Assert.AreEqual(nameof(LoggerExtensionsTests), logger.LastStateTypeName);
			Assert.AreEqual("Hello Alice from Wonderland", logger.LastFormattedMessage);
		}
		
		[Test]
		public void Log_Shorthand_UsesNullStateAsGenericType()
		{
			var logger = new CapturingLogger();

			logger.Log(LogLevel.Debug, "Value = {0}", 123);

			Assert.AreEqual(LogLevel.Debug, logger.LastLogLevel);
			Assert.IsNull(logger.LastException);
			Assert.AreEqual(nameof(NullState), logger.LastStateTypeName);
			Assert.AreEqual("Value = 123", logger.LastFormattedMessage);
		}
		
		[Test]
		public void LogError_Generic_UsesErrorLevelAndStateType()
		{
			var logger = new CapturingLogger();

			logger.LogError<LoggerExtensionsTests>("Error: {0}", 5);

			Assert.AreEqual(LogLevel.Error, logger.LastLogLevel);
			Assert.IsNull(logger.LastException);
			Assert.AreEqual(nameof(LoggerExtensionsTests), logger.LastStateTypeName);
			Assert.AreEqual("Error: 5", logger.LastFormattedMessage);
		}
		
		[Test]
		public void LogError_Shorthand_UsesNullState()
		{
			var logger = new CapturingLogger();

			logger.LogError("Failed with code {0}", 13);

			Assert.AreEqual(LogLevel.Error, logger.LastLogLevel);
			Assert.IsNull(logger.LastException);
			Assert.AreEqual(nameof(NullState), logger.LastStateTypeName);
			Assert.AreEqual("Failed with code 13", logger.LastFormattedMessage);
		}
		
		[Test]
		public void LogInfo_Shorthand_UsesInformationLevel()
		{
			var logger = new CapturingLogger();

			logger.LogInfo("User {0} logged in", "Alice");

			Assert.AreEqual(LogLevel.Information, logger.LastLogLevel);
			Assert.IsNull(logger.LastException);
			Assert.AreEqual(nameof(NullState), logger.LastStateTypeName);
			Assert.AreEqual("User Alice logged in", logger.LastFormattedMessage);
		}
		
		[Test]
		public void LogCritical_WithException_UsesCriticalLevel()
		{
			var logger = new CapturingLogger();
			var ex = new Exception("Critical failure");

			logger.LogCritical<LoggerExtensionsTests>(ex, "Message: {0}");

			Assert.AreEqual(LogLevel.Critical, logger.LastLogLevel);
			Assert.AreSame(ex, logger.LastException);
			Assert.AreEqual(nameof(LoggerExtensionsTests), logger.LastStateTypeName);
			var expected = $"Message: {ex}";
			
			Assert.AreEqual(expected, logger.LastFormattedMessage);
		}
		
		[Test]
		public void ThrowIfNull_ThrowsArgumentNullException_ForNullLogger()
		{
			ILogger logger = null;

			Assert.Throws<ArgumentNullException>(() =>
			{
				logger.LogInfo("Will not be logged");
			});
		}
		
		private sealed class CapturingLogger : ILogger
		{
			public LogLevel? LastLogLevel { get; private set; }
			public Exception LastException { get; private set; }
			public string LastFormattedMessage { get; private set; }
			public string LastStateTypeName { get; private set; }
			public string LastScopeState { get; private set; }

			public IDisposable BeginScope<TState>()
			{
				return NullScope.Instance;
			}

			public IDisposable BeginScope(string state)
			{
				LastScopeState = state;
				return NullScope.Instance;
			}

			public bool IsEnabled(LogLevel logLevel)
			{
				return true;
			}

			public void Log<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter)
			{
				LastLogLevel = logLevel;
				LastException = exception;
				LastStateTypeName = typeof(TState).Name;
				LastFormattedMessage = formatter?.Invoke(exception);
			}
		}
	}
}