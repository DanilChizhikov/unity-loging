using NUnit.Framework;

namespace DTech.Logging.Tests
{
	[TestFixture]
	internal sealed class LogConditionTests
	{
		[Test]
		public void LogConditions_RespectEditorAndDefines()
		{
			Assert.IsTrue(LogConditions.IsDevelopmentBuild, "IsDevelopmentBuild should be true in editor.");
			
			Assert.IsTrue(LogConditions.IsFileLoggingEnabled,
				"IsFileLoggingEnabled should be true when DISABLE_FILE_LOGGING is not defined.");
		}
	}
}