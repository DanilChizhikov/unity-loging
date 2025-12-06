using System;
using System.IO;
using NUnit.Framework;

namespace DTech.Logging.Tests
{
	[TestFixture]
	internal sealed class FileLoggerTests
	{
		private LogSettingsWrapper _settingsWrapper;
		
		[OneTimeSetUp]
		public void SetUp()
		{
			_settingsWrapper = new LogSettingsWrapper(LoggerSettings.Instance);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			_settingsWrapper.ResetSettings();
			if (File.Exists(LoggerFileProvider.CurrentLogFilePath))
			{
				File.Delete(LoggerFileProvider.CurrentLogFilePath);
			}
		}
		
		[Test]
		public void Log_WithEnabledFileLogging_WritesToFile()
		{
			_settingsWrapper.OverrideInformationEnabled(true)
				.OverrideFileLoggingEnabled(true);
			
			var logger = new CapturingFileLogger("TestTag");
			
			logger.LogInfo("Hello World");
			bool exists = File.Exists(LoggerFileProvider.CurrentLogFilePath);
			if (exists)
			{
				File.Delete(LoggerFileProvider.CurrentLogFilePath);
			}
			
			Assert.IsTrue(exists);
		}
		
		[Test]
		public void Log_WithDisabledFileLogging_DoesNotWriteToLog()
		{
			_settingsWrapper.OverrideFileLoggingEnabled(false);
			string currentLogFilePath = LoggerFileProvider.CurrentLogFilePath;
			if (File.Exists(currentLogFilePath))
			{
				File.Delete(currentLogFilePath);
			}
			
			var logger = new CapturingFileLogger("TestTag");
			logger.LogInfo("Hello World");
			
			bool exists = File.Exists(currentLogFilePath);
			Assert.IsFalse(exists, $"File should not exist at path: {currentLogFilePath};" +
				$"\n{File.ReadAllText(currentLogFilePath)}");
		}
		
		private sealed class CapturingFileLogger : InternalLoggerBase
		{
			public CapturingFileLogger(string tag) : base(tag)
			{
			}

			public override bool IsEnabled(LogLevel logLevel)
			{
				return LoggerSettings.Instance.IsFileLoggingEnabled &&
					LoggerSettings.Instance.IsEnabled(logLevel);
			}

			protected override void SendLog<TState>(LogLevel logLevel, Exception exception, Func<Exception, string> formatter, string scopes)
			{
				if (!LoggerSettings.Instance.IsFileLoggingEnabled)
				{
					return;
				}
			
				using var stream = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true);
				string level = logLevel.ToString().ToUpperInvariant();
				switch (logLevel)
				{
					case LogLevel.Information:
					case LogLevel.Warning:
					case LogLevel.Critical:
					{
						level = level.Substring(0, 4);
					} break;
				}

				string stateName = typeof(TState).Name;
				bool isNullState = stateName == NullStateName;
				if (string.IsNullOrEmpty(Tag))
				{
					if (isNullState)
					{
						stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}]{scopes} {formatter(exception)}");
					}
					else
					{
						stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}]{scopes}[{stateName}] {formatter(exception)}");	
					}
				}
				else
				{
					if (isNullState)
					{
						stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}]{scopes}[{Tag}] {formatter(exception)}");
					}
					else
					{
						stream.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}][{level}]{scopes}[{Tag}][{stateName}] {formatter(exception)}");
					}
				}
			}
		}
	}
}