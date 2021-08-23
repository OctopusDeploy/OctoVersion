using System;
using OctoVersion.Core.Configuration;
using Serilog;

namespace OctoVersion.Core.OutputFormatting.Console
{
    class ConsoleOutputFormatter : IOutputFormatter
    {
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

        public void ConfigureLogSink(LoggerConfiguration lc) => lc.WriteTo.LiterateConsole();
    }
}
