using System;

namespace DTech.Logging.Tests.Performance
{
    internal sealed class PerformanceInternalLogger : InternalLoggerBase
    {
        protected override LogLineBuilder LineBuilder { get; } =
            new(LoggerSettings.Instance.ConsoleFormatString, LoggerSettings.Instance.PlacementReplacers);

        public PerformanceInternalLogger(string tag) : base(tag)
        {
        }

        public override bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

	        protected override void SendLog<TState>(LogLevel logLevel, Exception exception, string message, object[] args, string scopes)
	        {
	            string logBody = FormatMessage(exception, message, args);
	            string stateName = typeof(TState).Name;
            LineBuilder.Reset();
            string log = LineBuilder.SetLogLevel(logLevel)
                .SetScopes(scopes)
                .SetTag(Tag)
                .SetStateName(stateName)
                .SetBody(logBody)
                .ToString();
            
            LineBuilder.Reset();
        }
    }
}
