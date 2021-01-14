using System;
using System.Diagnostics;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.ExtensionMethods;

namespace OctoVersion.Runner
{
    public static class OctoVersionExtension
    {
        public static void OctoVersion(out OctoVersionInfo versionInfo)
        {
            var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>();
            var runner = new OctoVersionRunner(appSettings, configuration);
            runner.Run(out versionInfo);
        }
        
        public static void OctoVersionDiscoverLocalGitBranch(out string branch)
        {
            var startProcessInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "rev-parse --abbrev-ref HEAD",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            
            var process = Process.Start(startProcessInfo);
            if (process == null)
            {
                throw new Exception("Failed to start the git process. Perhaps you don't have git installed globally?");
            }
            
            process.WaitForExit();
            var stdout = process.StandardOutput;
            var stderr = process.StandardError;

            var exitCode = process.ExitCode;
            if (exitCode != 0)
                throw new Exception("Calling git binary to determine local branch failed.")
                    .WithData(nameof(stdout), stdout)
                    .WithData(nameof(stderr), stderr);

            var bareBranch = stdout.ReadLine() ?? throw new Exception("Failed to determine local branch.")
                .WithData(nameof(stdout), stdout)
                .WithData(nameof(stderr), stderr);

            branch = $"refs/heads/{bareBranch}";

            var environmentVariableName = $"{ConfigurationBootstrapper.EnvironmentVariablePrefix}{nameof(AppSettings.CurrentBranch)}";
            Environment.SetEnvironmentVariable(environmentVariableName, branch);

            //context.Log.Warning("The current Git branch has been automatically determined to be {0}.", branch);
            //context.Log.Warning($"It is STRONGLY RECOMMENDED to NOT rely on automatic branch detection on your build agents. It will fail in unexpected ways for tags, pull requests, commit hashes etc. Please set the {environmentVariableName} variable deterministically instead.");
        }
    }
}