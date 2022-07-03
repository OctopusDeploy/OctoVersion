using System;
using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Core.OutputFormatting.GitHubActions;

public class GitHubActionsLogSink : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        var message = logEvent.MessageTemplate.Render(logEvent.Properties);

        Log(logEvent.Level, message, logEvent.Exception);
    }

    public static void Log(LogEventLevel logEventLevel, string message, Exception? ex = null)
    {
        // ::debug::This is a debug message
        // This is a normal message
        // ::warn::This is a warning message
        // ::error::This is an error message

        var prefix = Prefix(logEventLevel);
        var errorDetails = ex?.ToString() ?? string.Empty;

        System.Console.WriteLine($"{prefix}{Sanitize(message)}");

        if ((logEventLevel == LogEventLevel.Error ||
                logEventLevel == LogEventLevel.Fatal) &&
            !string.IsNullOrEmpty(errorDetails))
            System.Console.WriteLine($"{prefix}{Sanitize(errorDetails)}");
    }

    static string Sanitize(string input)
    {
        // See GitHub Actions toolkit for items that need to be sanitized/escaped
        // https://github.com/actions/toolkit/blob/2f164000dcd42fb08287824a3bc3030dbed33687/packages/core/src/command.ts#L92-L97
        var output = input
                .Replace("%", "%25")
                .Replace("\r", "%0A")
                .Replace("\n", "%0D")
            ;
        return output;
    }

    static string Prefix(LogEventLevel logEventLevel)
    {
        return logEventLevel switch
        {
            LogEventLevel.Verbose or LogEventLevel.Debug => "::debug::",
            LogEventLevel.Information => "",
            LogEventLevel.Warning => "::warn::",
            LogEventLevel.Error or LogEventLevel.Fatal => "::error::",
            _ => throw new ArgumentOutOfRangeException(nameof(logEventLevel), logEventLevel, null)
        };
    }
}