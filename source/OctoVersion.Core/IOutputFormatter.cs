using System;
using Serilog;
using Serilog.Core;

namespace OctoVersion.Core
{
    public interface IOutputFormatter
    {
        string Name { get; }
        void Write(OctoVersionInfo octoVersionInfo);
        bool MatchesRuntimeEnvironment();
        bool SuppressDefaultConsoleOutput { get; }
        ILogEventSink LogSink { get; }
    }
}
