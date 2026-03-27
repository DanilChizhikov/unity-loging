using NUnit.Framework;
using Unity.PerformanceTesting;

namespace DTech.Logging.Tests.Performance
{
    [TestFixture]
    internal sealed class LoggerPerformanceTests
    {
        private ILogger _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = LoggerFactory.CreateLogger("PerfTest");
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test, Performance]
        public void LogInfo_SimpleMessage_Performance()
        {
            Measure.Method(() => { _logger.LogInfo("Simple message"); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogInfo_FormattedMessage_Performance()
        {
            Measure.Method(() => { _logger.LogInfo("Formatted message {0} {1} {2}", 1, "test", 3.14f); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogInfo_LargeMessage_Performance()
        {
            const string largeMessage =
                "This is a very long message with lots of text content that simulates " +
                "real-world logging scenarios where messages can be quite verbose and contain " +
                "detailed information about the application state and various parameters " +
                "that developers need to track during debugging and monitoring sessions.";

            Measure.Method(() => { _logger.LogInfo(largeMessage); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogDebug_Performance()
        {
            Measure.Method(() => { _logger.LogDebug("Debug message {0}", 123); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogWarning_Performance()
        {
            Measure.Method(() => { _logger.LogWarning("Warning message {0}", "alert"); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogError_Performance()
        {
            Measure.Method(() => { _logger.LogError("Error message {0}", 456); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogCritical_Performance()
        {
            Measure.Method(() => { _logger.LogCritical("Critical message {0}", "fatal"); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogTrace_Performance()
        {
            Measure.Method(() => { _logger.LogTrace("Trace message {0}", 789); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }
    }
}
