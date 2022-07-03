using System;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Console;

class QuietConsoleOutputFormatter : IOutputFormatter
{
    public QuietConsoleOutputFormatter(AppSettings appSettings)
    {
    }

    public ILogEventSink LogSink { get; } = new NullSink();
    public string Name => "QuietConsole";

    public bool SuppressDefaultConsoleOutput => true; //shhh. we dont want any noise

    public void Write(OctoVersionInfo octoVersionInfo)
    {
        System.Console.WriteLine(octoVersionInfo);
    }

    public bool MatchesRuntimeEnvironment()
    {
        return false;
    }
}