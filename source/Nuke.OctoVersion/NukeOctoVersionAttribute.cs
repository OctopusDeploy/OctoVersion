using System;
using System.Reflection;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.ValueInjection;
using OctoVersion.Core;
using Serilog;

namespace Nuke.OctoVersion
{
    [UsedImplicitly(ImplicitUseKindFlags.Default)]
    public class NukeOctoVersionAttribute : ValueInjectionAttributeBase
    {
        // This is the same was as how NukeBuild determines if it's a local build or not as seen here:
        // https://github.com/nuke-build/nuke/blob/772b22391bda0929d758f4089e2da55e7566fb8d/source/Nuke.Common/NukeBuild.Statics.cs#L114
        static bool IsLocalBuild => NukeBuild.Host == HostType.Console;

        public override object GetValue(MemberInfo member, object instance)
        {
            if (IsLocalBuild)
                OctoVersionDiscoverLocalGitBranch();

            return OctoVersion();
        }

        static OctoVersionInfo OctoVersion()
        {
            var outputFormatter = GetOutputFormatter();
            OctoVersionRunnerWrapper.OctoVersion(out var octoVersionInfo,
                lc => WriteLogToSink(lc, outputFormatter));

            return octoVersionInfo;
        }

        static void OctoVersionDiscoverLocalGitBranch()
        {
            var outputFormatter = GetOutputFormatter();
            OctoVersionRunnerWrapper.OctoVersionDiscoverLocalGitBranch(out _,
                lc => WriteLogToSink(lc, outputFormatter));
        }

        static void WriteLogToSink(LoggerConfiguration loggerConfiguration, IOutputFormatter outputFormatter)
        {
            loggerConfiguration.WriteTo.Sink(outputFormatter.LogSink);
        }

        static IOutputFormatter GetOutputFormatter()
        {
            return new NukeOutputFormatter();
        }
    }
}