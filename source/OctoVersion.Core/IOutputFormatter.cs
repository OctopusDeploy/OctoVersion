using System;
using Serilog;

namespace OctoVersion.Core
{
    public interface IOutputFormatter
    {
        string Name { get; }
        void Write(OctoVersionInfo octoVersionInfo);
        bool MatchesRuntimeEnvironment();
        void ConfigureLogSink(LoggerConfiguration lc);
    }
}
