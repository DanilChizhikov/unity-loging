using System;

namespace DTech.Logging.Tests.Performance
{
    internal sealed class PerformanceInternalLogger : InternalLoggerBase
    {
        public PerformanceInternalLogger(string tag) : base(tag)
        {
        }

        public override bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        protected override void SendLog<TState>(LogLevel logLevel, Exception exception, string message, object[] args, string scopes)
        {
            LoggerSettings settings = LoggerSettings.Instance;
            LogLineBuilder lineBuilder = GetOrCreateLineBuilder(settings.ConsoleFormatString, settings.PlacementReplacers);
            string logBody = FormatMessage(exception, message, args);
            string stateName = StateName<TState>.Value;
            _ = lineBuilder.Render(logLevel, scopes, Tag, stateName, logBody);
        }

        protected override void SendLog<TState>(LogLevel logLevel, Exception exception, string message, string scopes)
        {
            LoggerSettings settings = LoggerSettings.Instance;
            LogLineBuilder lineBuilder = GetOrCreateLineBuilder(settings.ConsoleFormatString, settings.PlacementReplacers);
            string logBody = FormatMessage(exception, message);
            string stateName = StateName<TState>.Value;
            _ = lineBuilder.Render(logLevel, scopes, Tag, stateName, logBody);
        }
    }
}
