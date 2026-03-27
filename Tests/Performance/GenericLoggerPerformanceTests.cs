using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;

namespace DTech.Logging.Tests.Performance
{
    [TestFixture]
    internal sealed class GenericLoggerPerformanceTests
    {
        private ILogger<GenericLoggerPerformanceTests> _genericLogger;

        [SetUp]
        public void SetUp()
        {
            _genericLogger = LoggerFactory.CreateLogger<GenericLoggerPerformanceTests>();
            LogAssert.ignoreFailingMessages = true;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
        }

        [Test, Performance]
        public void GenericLogger_LogInfo_Performance()
        {
            Measure.Method(() => { _genericLogger.LogInfo("Generic logger message {0}", 123); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void GenericLogger_BeginScope_Performance()
        {
            Measure.Method(() =>
                {
                    using (_genericLogger.BeginScope("Scope"))
                    {
                        _genericLogger.LogInfo("Message");
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void GenericLogger_IsEnabled_Performance()
        {
            Measure.Method(() =>
                {
                    _ = _genericLogger.IsEnabled(LogLevel.Information);
                    _ = _genericLogger.IsEnabled(LogLevel.Debug);
                    _ = _genericLogger.IsEnabled(LogLevel.Warning);
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }
    }
}
