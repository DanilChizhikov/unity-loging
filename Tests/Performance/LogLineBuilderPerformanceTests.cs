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
                "[DATE_TIME:HH:mm:ss] [LOG_LEVEL] [LOG_TAG]",
                Array.Empty<Placements.ILogPlacementReplacer>());
        }

        [Test, Performance]
        public void LogLineBuilder_Render_Performance()
        {
            Measure.Method(() =>
                {
                    _ = _builder.Render(LogLevel.Information, "Scope > Test", "Tag", "State", "Message body");
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }
    }
}
