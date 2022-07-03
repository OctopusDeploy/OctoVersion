using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Shellfish;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.ExtensionMethods;
using Serilog;

namespace OctoVersion.Core;

public static class OctoVersionRunnerWrapper
{
    public static void OctoVersion(out OctoVersionInfo versionInfo, Action<LoggerConfiguration> additionLogs)
    {
        var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>();
        var runner = new OctoVersionRunner(appSettings, configuration, additionLogs);
        runner.Run(out versionInfo);
    }

    public static void OctoVersionDiscoverLocalGitBranch(out string branch, Action<LoggerConfiguration> additionLogs)
    {
        var (_, configuration) = ConfigurationBootstrapper.BootstrapWithoutValidation<AppSettings>();
        LogBootstrapper.Bootstrap(configuration, additionLogs);

        var stdout = new List<string>();
        var stderr = new List<string>();

        var exitCode = ShellExecutor.ExecuteCommand(
            "git",
            "rev-parse --abbrev-ref HEAD",
            Environment.CurrentDirectory,
            log => { },
            log => { stdout.Add(log); },
            log => { stdout.Add(log); }
        );

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

        Log.Warning("The current Git branch has been automatically determined to be {branch}.", branch);
        Log.Warning($"It is STRONGLY RECOMMENDED to NOT rely on automatic branch detection on your build agents. It will fail in unexpected ways for tags, pull requests, commit hashes etc. Please set the {environmentVariableName} variable deterministically instead");
    }
}