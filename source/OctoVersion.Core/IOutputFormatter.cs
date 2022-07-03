using System;
using Serilog.Core;

namespace OctoVersion.Core;

public interface IOutputFormatter
{
    ILogEventSink LogSink { get; }
    string Name { get; }

    /// <summary>
    /// If this output formatter takes responsibility for outputting to the console (ie, CI specific log messages)
    /// then this should return `true`.
    /// </summary>
    bool SuppressDefaultConsoleOutput { get; }

    void Write(OctoVersionInfo octoVersionInfo);
    bool MatchesRuntimeEnvironment();
}