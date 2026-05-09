using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTech.Logging.Tests
{
	[TestFixture]
	internal sealed class LoggerTests
	{
		[SetUp]
		public void SetUp()
		{
			LogAssert.ignoreFailingMessages = false;
		}
		
		[Test]
		public void LogInfo_WithoutException_WritesExpectedUnityLog()
		{
			var logger = new Logger("TestTag");
			const string Expected = "[INFO][TestTag] Hello World";
			
			LogAssert.Expect(LogType.Log, Expected);
			logger.LogInfo("Hello {0}", "World");
		}
		
		[Test]
		public void LogError_WithException_WritesErrorLogWithExceptionFirst()
		{
			var logger = new Logger("TestTag");
			var ex = new InvalidOperationException("Boom");
			var pattern = @"^\[ERROR\]\[TestTag\]\[LoggerTests\] Error occurred\. Exception: .*?, Value: 42$";
			var regex = new System.Text.RegularExpressions.Regex(pattern);
			
			LogAssert.Expect(LogType.Error, regex);
			
			logger.LogError<LoggerTests>(ex,
				"Error occurred. Exception: {0}, Value: {1}",
				42);
			
		}
		
		[Test]
		public void BeginScope_AddsScopeInfoToLog()
		{
			var logger = new Logger("TestTag");
			const string Expected =
				"[INFO][Scope > ScopeName][TestTag] Message in scope";
			
			using (logger.BeginScope("ScopeName"))
			{
				LogAssert.Expect(LogType.Log, Expected);
				logger.LogInfo("Message in scope");
			}
		}
		
		[Test]
		public void NestedScopes_AreOrderedFromOuterToInner()
		{
			var logger = new Logger("TestTag");
			const string Expected =
				"[INFO][Scope > Outer > Inner][TestTag] Nested";
			
			using (logger.BeginScope("Outer"))
			{
				using (logger.BeginScope("Inner"))
				{
					LogAssert.Expect(LogType.Log, Expected);
					logger.LogInfo("Nested");
				}
			}
		}
		
		[Test]
		public void GenericLogger_CategoryName_IsUsedAsTag()
		{
			var logger = new Logger<LoggerTests>();
			const string Expected = "[INFO][LoggerTests] Category test";
			
			LogAssert.Expect(LogType.Log, Expected);
			logger.LogInfo("Category test");
		}
	}
}
