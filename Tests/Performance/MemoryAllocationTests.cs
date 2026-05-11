using System;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;

namespace DTech.Logging.Tests.Performance
{
    [TestFixture]
    internal sealed class MemoryAllocationTests
    {
        private const int WarmupIterations = 100;
        private const int MeasurementIterations = 1000;

        // Thresholds expressed as bytes-per-call. Tuned for the post-optimization
        // baseline; tighter than necessary on develop, but the point is to detect
        // regressions if allocation behaviour grows from here.
        private const long IsEnabledMaxBytesPerCall = 0;
        private const long LogInfoNoArgsMaxBytesPerCall = 512;
        private const long LogInfoSingleArgMaxBytesPerCall = 768;
        private const long BeginScopeMaxBytesPerCall = 512;

        private static readonly object[] _emptyArgs = Array.Empty<object>();
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

        [Test]
        public void IsEnabled_Check_AllocatesNothing()
        {
            for (int i = 0; i < WarmupIterations; i++)
            {
                _ = _logger.IsEnabled(LogLevel.Information);
            }

            long perCall = MeasureBytesPerCall(() =>
            {
                _ = _logger.IsEnabled(LogLevel.Debug);
                _ = _logger.IsEnabled(LogLevel.Information);
                _ = _logger.IsEnabled(LogLevel.Warning);
            }, MeasurementIterations);

            AssertWithinBudget(perCall, IsEnabledMaxBytesPerCall, nameof(IsEnabled_Check_AllocatesNothing));
        }

        [Test]
        public void LogInfo_NoArgs_StaysWithinBudget()
        {
            Warmup(() => _logger.LogInfo("Simple message without arguments"));

            long perCall = MeasureBytesPerCall(
                () => _logger.LogInfo("Simple message without arguments"),
                MeasurementIterations);

            AssertWithinBudget(perCall, LogInfoNoArgsMaxBytesPerCall, nameof(LogInfo_NoArgs_StaysWithinBudget));
        }

        [Test]
        public void LogInfo_SingleArg_StaysWithinBudget()
        {
            Warmup(() => _logger.LogInfo("Message with one arg: {0}", 42));

            long perCall = MeasureBytesPerCall(
                () => _logger.LogInfo("Message with one arg: {0}", 42),
                MeasurementIterations);

            AssertWithinBudget(perCall, LogInfoSingleArgMaxBytesPerCall, nameof(LogInfo_SingleArg_StaysWithinBudget));
        }

        [Test]
        public void BeginScope_StaysWithinBudget()
        {
            Warmup(() =>
            {
                using (_logger.BeginScope("TestScope")) { }
            });

            long perCall = MeasureBytesPerCall(() =>
            {
                using (_logger.BeginScope("TestScope")) { }
            }, MeasurementIterations);

            AssertWithinBudget(perCall, BeginScopeMaxBytesPerCall, nameof(BeginScope_StaysWithinBudget));
        }

        // ----- Performance benchmarks (collected by Unity Performance Testing) -----

        [Test, Performance]
        public void LogInfo_NoArgs_Benchmark()
        {
            Measure.Method(() => { _logger.LogInfo("Simple message without arguments"); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void LogInfo_NoArgs_ParamsEmptyArray_Benchmark()
        {
            Measure.Method(() => { _logger.LogInfo("Simple message without arguments", _emptyArgs); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Log_GenericNoArgs_Benchmark()
        {
            Measure.Method(() => { _logger.Log<NullState>(LogLevel.Information, "Simple message without arguments"); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Log_GenericNoArgs_ParamsEmptyArray_Benchmark()
        {
            Measure.Method(() => { _logger.Log<NullState>(LogLevel.Information, "Simple message without arguments", _emptyArgs); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void LogInfo_MultipleArgs_Benchmark()
        {
            Measure.Method(() => { _logger.LogInfo("Args: {0}, {1}, {2}", 1, "two", 3.0f); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void BeginScope_WithLogging_Benchmark()
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

        // ----- Helpers -----

        private static void Warmup(Action action)
        {
            for (int i = 0; i < WarmupIterations; i++)
            {
                action();
            }
        }

        private static long MeasureBytesPerCall(Action action, int iterations)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            long before = GC.GetAllocatedBytesForCurrentThread();
            for (int i = 0; i < iterations; i++)
            {
                action();
            }

            long after = GC.GetAllocatedBytesForCurrentThread();
            return (after - before) / iterations;
        }

        private static void AssertWithinBudget(long actualBytesPerCall, long budget, string testName)
        {
            Assert.That(
                actualBytesPerCall,
                Is.LessThanOrEqualTo(budget),
                $"{testName}: {actualBytesPerCall} bytes/call exceeds budget of {budget} bytes/call. " +
                "Investigate recent changes to the rendering hot path.");
        }
    }
}
