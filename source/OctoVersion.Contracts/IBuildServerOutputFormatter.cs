using Serilog.Core;

namespace OctoVersion.Contracts
{
    public interface IBuildServerOutputFormatter
    {
        string SupportedBuildServer { get; }
        ILogEventSink LogSink { get; }
        void WriteVersionInformation(VersionInfo versionInfo);
    }
}