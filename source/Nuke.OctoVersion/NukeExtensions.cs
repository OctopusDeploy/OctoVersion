using System;
using Nuke.Common;
using OctoVersion.Core;
using OctoVersion.Runner;
using Serilog;

namespace Nuke.OctoVersion
{
    public static class NukeExtensions
    {
        public static ITargetDefinition OctoVersion(this ITargetDefinition targetDefinition, out OctoVersionInfo versionInfo)
        {
            var outputFormatter = GetOutputFormatter();
            OctoVersionExtension.OctoVersion(out versionInfo, 
                lc => WriteLogToSink(lc, outputFormatter));
            return targetDefinition;
        }

        public static ITargetDefinition OctoVersionDiscoverLocalGitBranch(this ITargetDefinition targetDefinition, out string branch)
        {
            var outputFormatter = GetOutputFormatter();
            OctoVersionExtension.OctoVersionDiscoverLocalGitBranch(out branch, 
                lc => WriteLogToSink(lc, outputFormatter));
            return targetDefinition;
        }
        
        static void WriteLogToSink(LoggerConfiguration loggerConfiguration, IOutputFormatter outputFormatter) =>
            loggerConfiguration.WriteTo.Sink(outputFormatter.LogSink);

        static IOutputFormatter GetOutputFormatter() => 
            new NukeOutputFormatter();
        
    }
}
