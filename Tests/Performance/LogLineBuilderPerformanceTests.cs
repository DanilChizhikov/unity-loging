using System;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace DTech.Logging.Tests.Performance
{
    [TestFixture]
    internal sealed class LogLineBuilderPerformanceTests
    {
        private LogLineBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new LogLineBuilder(
                "[{DateTime:HH:mm:ss}] [{LogLevel}] [{Tag}] {Body}",
                Array.Empty<Placements.ILogPlacementReplacer>());
        }

        [Test, Performance]
        public void LogLineBuilder_SetProperties_Performance()
        {
            Measure.Method(() =>
                {
                    _builder.SetLogLevel(LogLevel.Information)
                        .SetScopes("Scope > Test")
                        .SetTag("Tag")
                        .SetStateName("State")
                        .SetBody("Message body");
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .Run();
        }

        [Test, Performance]
        public void LogLineBuilder_ToString_Performance()
        {
            _builder.SetLogLevel(LogLevel.Information)
                .SetScopes("Scope > Test")
                .SetTag("Tag")
                .SetStateName("State")
                .SetBody("Message body");

            Measure.Method(() =>
                {
                    _ = _builder.ToString();
                    _builder.Reset();
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .Run();
        }

        [Test, Performance]
        public void LogLineBuilder_FullCycle_Performance()
        {
            Measure.Method(() =>
                {
                    _builder.SetLogLevel(LogLevel.Information)
                        .SetScopes("Scope > Test")
                        .SetTag("Tag")
                        .SetStateName("State")
                        .SetBody("Message body");
                    _ = _builder.ToString();
                    _builder.Reset();
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void LogLineBuilder_Reset_Performance()
        {
            _builder.SetLogLevel(LogLevel.Information)
                .SetScopes("Scope > Test")
                .SetTag("Tag")
                .SetStateName("State")
                .SetBody("Message body");

            Measure.Method(() => { _builder.Reset(); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(10000)
                .Run();
        }
    }
}
