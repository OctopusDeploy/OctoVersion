using System;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using OctoVersion.Core;
using OctoVersion.Runner;

// ReSharper disable UnusedMember.Global
[assembly: CakeNamespaceImport("OctoVersion.Core")]

namespace Cake.OctoVersion
{
    public static class CakeExtensions
    {
        [CakeMethodAlias]
        public static void OctoVersion(this ICakeContext context, out OctoVersionInfo versionInfo)
        {
            OctoVersionExtension.OctoVersion(out versionInfo);
        }

        [CakeMethodAlias]
        public static void OctoVersionDiscoverLocalGitBranch(this ICakeContext context, out string branch)
        {
            OctoVersionExtension.OctoVersionDiscoverLocalGitBranch(out branch);

            //context.Log.Warning("The current Git branch has been automatically determined to be {0}.", branch);
            //context.Log.Warning($"It is STRONGLY RECOMMENDED to NOT rely on automatic branch detection on your build agents. It will fail in unexpected ways for tags, pull requests, commit hashes etc. Please set the {environmentVariableName} variable deterministically instead.");
        }
    }
}