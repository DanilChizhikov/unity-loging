using System.Collections.Generic;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace DTech.Logging.Tests.Performance
{
    [TestFixture]
    internal sealed class LoggerUtilityPerformanceTests
    {
        [Test, Performance]
        public void GetDefaultLoggers_SingleCall_Performance()
        {
            Measure.Method(() => { LoggerUtility.GetDefaultLoggers("TestTag"); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void GetDefaultLoggers_MultipleCalls_Performance()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        LoggerUtility.GetDefaultLoggers($"Tag{i}");
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void GetDefaultLoggers_RepeatedSameTag_Performance()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        LoggerUtility.GetDefaultLoggers("SameTag");
                    }
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1)
                .Run();
        }

        [Test, Performance]
        public void GetDefaultLoggers_UniqueTags_Performance()
        {
            var tags = new List<string>(100);
            for (int i = 0; i < 100; i++)
            {
                tags.Add($"UniqueTag{i}");
            }

            Measure.Method(() =>
                {
                    for (int i = 0; i < tags.Count; i++)
                    {
                        LoggerUtility.GetDefaultLoggers(tags[i]);
                    }
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void GetDefaultLoggers_EmptyTag_Performance()
        {
            Measure.Method(() => { LoggerUtility.GetDefaultLoggers(string.Empty); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void GetDefaultLoggers_LongTag_Performance()
        {
            string longTag = "VeryLongTagNameThatMightBeUsedInRealScenarios" +
                             "WithAdditionalContextInformationAndCategoryDetails";

            Measure.Method(() => { LoggerUtility.GetDefaultLoggers(longTag); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }
    }
}
