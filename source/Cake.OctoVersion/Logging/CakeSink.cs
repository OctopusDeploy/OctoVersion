using System;
using Cake.Core;
using Cake.Core.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace Cake.OctoVersion.Logging;

public class CakeSink : ILogEventSink
{
    readonly ICakeContext context;

    public CakeSink(ICakeContext context)
    {
        this.context = context;
    }

    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.MessageTemplate.Render(logEvent.Properties);
        var errorDetails = logEvent.Exception?.ToString() ?? string.Empty;

        switch (logEvent.Level)
        {
            case LogEventLevel.Verbose:
                context.Log.Verbosity = Verbosity.Verbose;
                context.Log.Information(message);
                break;
            case LogEventLevel.Debug:
            case LogEventLevel.Information:
                context.Log.Information(message);
                break;
            case LogEventLevel.Warning:
                context.Log.Warning(message);
                break;
            case LogEventLevel.Error:
            case LogEventLevel.Fatal:
                context.Log.Error(message);
                context.Log.Error(errorDetails);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logEvent.Level), logEvent.Level, null);
        }
    }
}