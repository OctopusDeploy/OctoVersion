using System;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.ExtensionMethods;

// ReSharper disable UnusedMember.Global

[assembly: CakeNamespaceImport("OctoVersion.Core")]

namespace Cake.OctoVersion
{
    public static class OctoVersionCakeExtension
    {
        [CakeMethodAlias]
        public static void OctoVersion(this ICakeContext context, out OctoVersionInfo versionInfo)
        {
            var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>();
            var runner = new OctoVersionRunner(appSettings, configuration);
            runner.Run(out versionInfo);
        }

        [CakeMethodAlias]
        public static void OctoVersionDiscoverLocalGitBranch(this ICakeContext context, out string branch)
        {
            var processSettings = new ProcessSettings
            {
                Arguments = "rev-parse --abbrev-ref HEAD",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var process = context.ProcessRunner.Start("git", processSettings);
            process.WaitForExit();
            var stdout = process.GetStandardOutput().ToArray();
            var stderr = process.GetStandardError().ToArray();

            var exitCode = process.GetExitCode();
            if (exitCode != 0)
                throw new Exception("Calling git binary to determine local branch failed.")
                    .WithData(nameof(stdout), stdout)
                    .WithData(nameof(stderr), stderr);

            var bareBranch = stdout.FirstOrDefault() ?? throw new Exception("Failed to determine local branch.")
                .WithData(nameof(stdout), stdout)
                .WithData(nameof(stderr), stderr);

            branch = $"refs/heads/{bareBranch}";

            var environmentVariableName = $"{ConfigurationBootstrapper.EnvironmentVariablePrefix}{nameof(AppSettings.CurrentBranch)}";
            Environment.SetEnvironmentVariable(environmentVariableName, branch);

            context.Log.Warning("The current Git branch has been automatically determined to be {0}.", branch);
            context.Log.Warning($"It is STRONGLY RECOMMENDED to NOT rely on automatic branch detection on your build agents. It will fail in unexpected ways for tags, pull requests, commit hashes etc. Please set the {environmentVariableName} variable deterministically instead.");
        }
    }
}