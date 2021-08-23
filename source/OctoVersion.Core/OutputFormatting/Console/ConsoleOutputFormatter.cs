using System;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Console
{
    class ConsoleOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; } = new NullSink();
        public string Name => "Console";

        public ConsoleOutputFormatter(AppSettings appSettings)
        {
        }

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            System.Console.WriteLine(octoVersionInfo);
        }

        public bool MatchesRuntimeEnvironment()
        {
            return false;
        }

        public bool SuppressDefaultConsoleOutput => false; //console output is our Raison D'Être
    }
}
