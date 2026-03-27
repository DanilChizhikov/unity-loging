using System;
using System.IO;
using DTech.Logging.Placements;
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
			
			var logger = new CapturingFileLogger("TestTag", LoggerSettings.Instance.FileFormatString);
			
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
			
			var logger = new CapturingFileLogger("TestTag", LoggerSettings.Instance.FileFormatString);
			logger.LogInfo("Hello World");
			
			bool exists = File.Exists(currentLogFilePath);
			string fileContents = exists ? File.ReadAllText(currentLogFilePath) : string.Empty;

			Assert.IsFalse(exists, $"File should not exist at path: {currentLogFilePath};\n{fileContents}");
		}
		
		private sealed class CapturingFileLogger : InternalLoggerBase
		{
			protected override LogLineBuilder LineBuilder { get; }
			
			public CapturingFileLogger(string tag, string logTemplate) : base(tag)
			{
				LineBuilder = new LogLineBuilder(logTemplate, Array.Empty<ILogPlacementReplacer>());
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
			
				string logBody = formatter(exception);
				string stateName = typeof(TState).Name;
				LineBuilder.Reset();
				LineBuilder.SetLogLevel(logLevel)
					.SetScopes(scopes)
					.SetTag(Tag)
					.SetStateName(stateName)
					.SetBody(logBody);
			
				using var stream = new StreamWriter(LoggerFileProvider.CurrentLogFilePath, true);
				stream.WriteLine(LineBuilder.ToString());
				LineBuilder.Reset();
			}
		}
	}
}