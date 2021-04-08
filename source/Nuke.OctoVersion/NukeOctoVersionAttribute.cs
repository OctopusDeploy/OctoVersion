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
        public override object GetValue(MemberInfo member, object instance)
        {
            if (NukeBuild.IsLocalBuild)
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
