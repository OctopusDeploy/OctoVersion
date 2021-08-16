using System;
using Serilog.Core;

namespace OctoVersion.Core
{
    public interface IOutputFormatter
    {
        ILogEventSink LogSink { get; }
        string Name { get; }
        void Write(OctoVersionInfo octoVersionInfo);
        bool MatchesRuntimeEnvironment();
    }
}
