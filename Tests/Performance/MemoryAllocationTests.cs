using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;

namespace DTech.Logging.Tests.Performance
{
    [TestFixture]
    internal sealed class MemoryAllocationTests
    {
        private ILogger _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = LoggerFactory.CreateLogger("MemoryTest");
            LogAssert.ignoreFailingMessages = true;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
        }

        [Test, Performance]
        public void LogInfo_NoArgs_ZeroAllocations()
        {
            Measure.Method(() => { _logger.LogInfo("Simple message without arguments"); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void LogInfo_SingleArg_Allocations()
        {
            Measure.Method(() => { _logger.LogInfo("Message with one arg: {0}", 42); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void LogInfo_MultipleArgs_Allocations()
        {
            Measure.Method(() => { _logger.LogInfo("Args: {0}, {1}, {2}", 1, "two", 3.0f); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void BeginScope_Allocations()
        {
            Measure.Method(() =>
                {
                    using (_logger.BeginScope("TestScope"))
                    {
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void BeginScope_WithLogging_Allocations()
        {
            Measure.Method(() =>
                {
                    using (_logger.BeginScope("TestScope"))
                    {
                        _logger.LogInfo("Message in scope {0}", 1);
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void IsEnabled_Check_NoAllocations()
        {
            Measure.Method(() =>
                {
                    _ = _logger.IsEnabled(LogLevel.Debug);
                    _ = _logger.IsEnabled(LogLevel.Information);
                    _ = _logger.IsEnabled(LogLevel.Warning);
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10000)
                .GC()
                .Run();
        }
    }
}
