using System;
using OctoVersion.Core.Logging;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Console
{
    class QuietConsoleOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; } = new NullSink();
        public string Name => "QuietConsole";

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            System.Console.WriteLine(octoVersionInfo);
        }

        public bool MatchesRuntimeEnvironment()
        {
            return false;
        }
    }
}