using System;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.OctoVersion.Logging;
using OctoVersion.Core;
using OctoVersion.Runner;
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
            OctoVersionExtension.OctoVersion(out versionInfo, lc => WriteLogToSink(lc, outputFormatter));
        }

        [CakeMethodAlias]
        public static void OctoVersionDiscoverLocalGitBranch(this ICakeContext context, out string branch)
        {
            var outputFormatter = GetOutputFormatter(context);
            OctoVersionExtension.OctoVersionDiscoverLocalGitBranch(out branch, lc => WriteLogToSink(lc, outputFormatter));
        }

        static void WriteLogToSink(LoggerConfiguration loggerConfiguration, IOutputFormatter outputFormatter) =>
            loggerConfiguration.WriteTo.Sink(outputFormatter.LogSink);

        static IOutputFormatter GetOutputFormatter(ICakeContext context) => 
            new CakeOutputFormatter(context);
    }
}