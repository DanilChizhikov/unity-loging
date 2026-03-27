using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine.TestTools;

namespace DTech.Logging.Tests.Performance
{
    [TestFixture]
    internal sealed class LoggerStressTests
    {
        private ILogger _logger;
        private ILogger<LoggerStressTests> _genericLogger;

        [SetUp]
        public void SetUp()
        {
            _logger = LoggerFactory.CreateLogger("StressTest");
            _genericLogger = LoggerFactory.CreateLogger<LoggerStressTests>();
            LogAssert.ignoreFailingMessages = true;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
        }

        [Test, Performance]
        public void StressTest_RapidLogging_Performance()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        _logger.LogInfo("Rapid log {0}", i);
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .Run();
        }

        [Test, Performance]
        public void StressTest_MultipleLogLevels_Performance()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        _logger.LogTrace("Trace {0}", i);
                        _logger.LogDebug("Debug {0}", i);
                        _logger.LogInfo("Info {0}", i);
                        _logger.LogWarning("Warning {0}", i);
                        _logger.LogError("Error {0}", i);
                        _logger.LogCritical("Critical {0}", i);
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .Run();
        }

        [Test, Performance]
        public void StressTest_MultipleArguments_Performance()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < 500; i++)
                    {
                        _logger.LogInfo(
                            "Args: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}",
                            i, i * 2, i * 3, "test", 1.5f, true, 'c', 123L, 4.5d, Guid.NewGuid());
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .Run();
        }

        [Test, Performance]
        public void StressTest_LoggerCreation_Performance()
        {
            var loggers = new List<Logger>(100);

            Measure.Method(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        loggers.Add(new Logger($"Tag{i}"));
                    }

                    loggers.Clear();
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void StressTest_GenericLoggerCreation_Performance()
        {
            var loggers = new List<Logger<LoggerStressTests>>(100);

            Measure.Method(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        loggers.Add(new Logger<LoggerStressTests>());
                    }

                    loggers.Clear();
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void StressTest_ComplexScenario_Performance()
        {
            Measure.Method(() =>
                {
                    using (_logger.BeginScope("Operation"))
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            using (_logger.BeginScope($"Iteration{i}"))
                            {
                                _logger.LogInfo("Processing {0} of {1}", i, 100);
                                if (i % 10 == 0)
                                {
                                    _logger.LogWarning("Milestone {0}", i);
                                }
                            }
                        }
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
        }
    }
}
