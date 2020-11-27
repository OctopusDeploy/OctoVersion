using System;
using OctoVersion.Core.Logging;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Console
{
    class ConsoleOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; } = new NullSink();

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            System.Console.WriteLine(octoVersionInfo);
        }
    }
}