using System;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Console
{
    class QuietConsoleOutputFormatter : IOutputFormatter
    {
        public string Name => "QuietConsole";

        public QuietConsoleOutputFormatter(AppSettings appSettings)
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

        public void ConfigureLogSink(LoggerConfiguration lc) => lc.WriteTo.Sink(new NullSink());
    }
}
