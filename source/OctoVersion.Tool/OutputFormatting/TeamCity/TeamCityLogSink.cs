using System;
using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Tool.OutputFormatting.TeamCity
{
    public class TeamCityLogSink : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            // We need to generate messages that look like this:
            // ##teamcity[message text='<message text>' errorDetails='<error details>' status='<status value>']

            var messageText = logEvent.MessageTemplate.Render(logEvent.Properties);
            var status = Status(logEvent.Level);
            var errorDetails = logEvent.Exception?.ToString() ?? string.Empty;

            var message =
                $"##teamcity[message text='{Sanitize(messageText)}' errorDetails='{Sanitize(errorDetails)}' status='{Sanitize(status)}']";
            System.Console.WriteLine(message);
        }

        private static string Sanitize(string input)
        {
            var output = input.Replace("'", string.Empty);
            return output;
        }

        private static string Status(LogEventLevel logEventLevel)
        {
            switch (logEventLevel)
            {
                case LogEventLevel.Verbose:
                case LogEventLevel.Debug:
                case LogEventLevel.Information:
                    return "NORMAL";
                case LogEventLevel.Warning:
                    return "WARNING";
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return "ERROR";
                default:
                    throw new ArgumentOutOfRangeException(nameof(logEventLevel), logEventLevel, null);
            }
        }
    }
}