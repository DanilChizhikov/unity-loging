using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;

namespace DTech.Logging.Tests.Performance
{
    [TestFixture]
    internal sealed class ScopePerformanceTests
    {
        private ILogger _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = LoggerFactory.CreateLogger("ScopeTest");
            LogAssert.ignoreFailingMessages = true;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
        }

        [Test, Performance]
        public void BeginScope_Simple_Performance()
        {
            Measure.Method(() =>
                {
                    using (_logger.BeginScope("SimpleScope"))
                    {
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void BeginScope_Formatted_Performance()
        {
            Measure.Method(() =>
                {
                    using (_logger.BeginScope("Scope {0} {1}", 1, "test"))
                    {
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogWithScope_SingleScope_Performance()
        {
            Measure.Method(() =>
                {
                    using (_logger.BeginScope("OuterScope"))
                    {
                        _logger.LogInfo("Message in scope");
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void LogWithScope_NestedScopes_Performance()
        {
            Measure.Method(() =>
                {
                    using (_logger.BeginScope("Outer"))
                    {
                        using (_logger.BeginScope("Middle"))
                        {
                            using (_logger.BeginScope("Inner"))
                            {
                                _logger.LogInfo("Deeply nested message");
                            }
                        }
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void BeginScope_Generic_Performance()
        {
            Measure.Method(() =>
                {
                    using (_logger.BeginScope<ScopePerformanceTests>())
                    {
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }
    }
}
