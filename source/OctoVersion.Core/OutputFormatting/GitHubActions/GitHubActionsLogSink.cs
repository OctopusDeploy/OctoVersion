using System;
using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Core.OutputFormatting.GitHubActions
{
    public enum LogLevel
    {
        Normal,
        Debug,
        Warning,
        Error
    }

    public class GitHubActionsLogSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            var logLevel = logEvent.Level switch
            {
                LogEventLevel.Verbose or LogEventLevel.Debug => LogLevel.Debug,
                LogEventLevel.Information => LogLevel.Normal,
                LogEventLevel.Warning => LogLevel.Warning,
                LogEventLevel.Error or LogEventLevel.Fatal => LogLevel.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(logEvent.Level), logEvent.Level, null),
            };

            var message = logEvent.MessageTemplate.Render(logEvent.Properties);
            var errorDetails = logEvent.Exception?.ToString() ?? string.Empty;

            Log(logLevel, message);
            if (logEvent.Level == LogEventLevel.Error || 
                logEvent.Level == LogEventLevel.Fatal && 
                !string.IsNullOrEmpty(errorDetails))
            {
                Log(logLevel, errorDetails);
            }
        }

        static string LogLevelPrefix(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Normal=> "",
                LogLevel.Debug => "::debug::",
                LogLevel.Warning => "::warn::",
                LogLevel.Error => "::error::",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
            };
        }

        public static void Log(LogLevel level, string message)
        {
            System.Console.WriteLine($"{LogLevelPrefix(level)}{message}");
        }
    }
}