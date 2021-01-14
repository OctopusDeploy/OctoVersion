using System;
using Nuke.Common;
using OctoVersion.Core;
using OctoVersion.Runner;

namespace Nuke.OctoVersion
{
    public static class NukeExtensions
    {
        public static ITargetDefinition OctoVersion(this ITargetDefinition targetDefinition, out OctoVersionInfo versionInfo)
        {
            OctoVersionExtension.OctoVersion(out versionInfo);
            return targetDefinition;
        }

        public static ITargetDefinition OctoVersionDiscoverLocalGitBranch(this ITargetDefinition targetDefinition, out string branch)
        {
            OctoVersionExtension.OctoVersionDiscoverLocalGitBranch(out branch);
            return targetDefinition;
        }
    }
}
