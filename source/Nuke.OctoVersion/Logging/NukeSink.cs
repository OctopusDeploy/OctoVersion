using System;
using Nuke.Common;
using Serilog.Core;
using Serilog.Events;
using Logger = Nuke.Common.Logger;

namespace Nuke.OctoVersion
{
    public class NukeSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            var errorDetails = logEvent.Exception?.ToString() ?? string.Empty;

            switch (logEvent.Level)
            {
                case LogEventLevel.Verbose:
                    Logger.LogLevel = LogLevel.Trace;
                    Logger.Info(message);
                    break;
                case LogEventLevel.Debug:
                case LogEventLevel.Information:
                    Logger.Info(message);
                    break;
                case LogEventLevel.Warning:
                    Logger.Warn(message);
                    break;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    Logger.Error(message);
                    Logger.Error(errorDetails);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logEvent.Level), logEvent.Level, null);
            }
        }
    }
}