using System;
using OctoVersion.Contracts;
using OctoVersion.Tool.Logging;
using Serilog.Core;

namespace OctoVersion.Tool.BuildServerOutputFormatting
{
    internal class NullBuildServerOutputFormatter : IBuildServerOutputFormatter
    {
        public string SupportedBuildServer { get; } = "no-build-server";
        public ILogEventSink LogSink { get; } = new NullSink();

        public void WriteVersionInformation(VersionInfo versionInfo)
        {
            Console.WriteLine(versionInfo);
        }
    }
}