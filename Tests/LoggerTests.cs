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
			
			logger.LogInfo("Hello {0}", "World");
			
			const string Expected = "[INFO][TestTag] Hello World";

			LogAssert.Expect(LogType.Log, Expected);
		}
		
		[Test]
		public void LogError_WithException_WritesErrorLogWithExceptionFirst()
		{
			var logger = new Logger("TestTag");
			var ex = new InvalidOperationException("Boom");
			
			logger.LogError<LoggerTests>(ex,
				"Error occurred. Exception: {0}, Value: {1}",
				42);
			
			var pattern = @"^\[ERROR\]\[TestTag\]\[LoggerTests\] Error occurred\. Exception: .*?, Value: 42$";
			var regex = new System.Text.RegularExpressions.Regex(pattern);
			
			LogAssert.Expect(LogType.Error, regex);
		}
		
		[Test]
		public void BeginScope_AddsScopeInfoToLog()
		{
			var logger = new Logger("TestTag");
			
			using (logger.BeginScope("ScopeName"))
			{
				logger.LogInfo("Message in scope");
			}
			
			const string Expected =
				"[INFO][Scope > TestTag > ScopeName][TestTag] Message in scope";
			
			LogAssert.Expect(LogType.Log, Expected);
		}
		
		[Test]
		public void NestedScopes_AreOrderedFromOuterToInner()
		{
			var logger = new Logger("TestTag");
			
			using (logger.BeginScope("Outer"))
			{
				using (logger.BeginScope("Inner"))
				{
					logger.LogInfo("Nested");
				}
			}
			
			const string Expected =
				"[INFO][Scope > TestTag > Outer > TestTag > Inner][TestTag] Nested";
			
			LogAssert.Expect(LogType.Log, Expected);
		}
		
		[Test]
		public void GenericLogger_CategoryName_IsUsedAsTag()
		{
			var logger = new Logger<LoggerTests>();
			
			logger.LogInfo("Category test");
			
			const string Expected = "[INFO][LoggerTests] Category test";
			
			LogAssert.Expect(LogType.Log, Expected);
		}
	}
}