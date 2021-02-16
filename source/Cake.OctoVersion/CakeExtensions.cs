using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.OctoVersion.Logging;
using OctoVersion.Core;
using Serilog;

// ReSharper disable UnusedMember.Global
[assembly: CakeNamespaceImport("OctoVersion.Core")]

namespace Cake.OctoVersion
{
    public static class CakeExtensions
    {
        [CakeMethodAlias]
        public static void OctoVersion(this ICakeContext context, out OctoVersionInfo versionInfo)
        {
            var outputFormatter = GetOutputFormatter(context);
            OctoVersionRunnerWrapper.OctoVersion(out versionInfo, lc => WriteLogToSink(lc, outputFormatter));
        }

        [CakeMethodAlias]
        public static void OctoVersionDiscoverLocalGitBranch(this ICakeContext context, out string branch)
        {
            var outputFormatter = GetOutputFormatter(context);
            OctoVersionRunnerWrapper.OctoVersionDiscoverLocalGitBranch(out branch, lc => WriteLogToSink(lc, outputFormatter));
        }

        static void WriteLogToSink(LoggerConfiguration loggerConfiguration, IOutputFormatter outputFormatter)
        {
            loggerConfiguration.WriteTo.Sink(outputFormatter.LogSink);
        }

        static IOutputFormatter GetOutputFormatter(ICakeContext context)
        {
            return new CakeOutputFormatter(context);
        }
    }
}