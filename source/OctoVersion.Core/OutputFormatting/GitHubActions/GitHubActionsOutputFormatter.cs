using System;
using System.IO;
using System.Reflection;
using OctoVersion.Core.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Core.OutputFormatting.GitHubActions;

public class GitHubActionsOutputFormatter : IOutputFormatter
{
    // https://docs.github.com/en/actions/reference/environment-variables#default-environment-variables
    public const string GitHubActionsEnvironmentVariableName = "GITHUB_ACTIONS";
    public const string GitHubActionsEnvTempFileEnvironmentVariableName = "GITHUB_ENV";
    public const string GitHubActionsOutputFileEnvironmentVariableName = "GITHUB_OUTPUT";

    public GitHubActionsOutputFormatter(AppSettings appSettings)
    {
    }

    public ILogEventSink LogSink => new GitHubActionsLogSink();

    public string Name => "GitHubActions";

    public bool SuppressDefaultConsoleOutput => true; //we do our own logging via GitHubActions workflow commands

    public void Write(OctoVersionInfo octoVersionInfo)
    {
        WriteOutputVariables(octoVersionInfo);
        WriteEnvironmentVariables(octoVersionInfo);
    }

    public bool MatchesRuntimeEnvironment()
    {
        return string.Equals("true", System.Environment.GetEnvironmentVariable(GitHubActionsEnvironmentVariableName), StringComparison.InvariantCultureIgnoreCase);
    }

    static void WriteOutputVariables(OctoVersionInfo octoVersionInfo)
    {
        var properties = octoVersionInfo.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

        // https://docs.github.com/en/actions/using-workflows/workflow-commands-for-github-actions#setting-an-output-parameter
        // The outgoing parameters must be written to a temporary file (identified by the $GITHUB_OUTPUT environment
        // variable, which changes for every step in a workflow) which is then parsed. That file must also be UTF-8 or it will fail.
        var gitHubOutputFilePath = System.Environment.GetEnvironmentVariable(GitHubActionsOutputFileEnvironmentVariableName);

        if (gitHubOutputFilePath != null)
        {
            GitHubActionsLogSink.Log(LogEventLevel.Information, $"Writing version variables to {GitHubActionsOutputFileEnvironmentVariableName} file ({gitHubOutputFilePath}) for '{nameof(GitHubActionsOutputFormatter)}'.");
            using var streamWriter = File.AppendText(gitHubOutputFilePath);
            foreach (var property in properties)
            {
                var configurationVariableKey = $"octoversion_{property.Name.ToLowerInvariant()}";
                var value = property.GetValue(octoVersionInfo)?.ToString() ?? string.Empty;
                streamWriter.WriteLine($"{configurationVariableKey}={value}");
            }
        }
        else
        {
            GitHubActionsLogSink.Log(LogEventLevel.Warning, $"Unable to write output parameters because the environment variable ${GitHubActionsOutputFileEnvironmentVariableName} is not set.");
        }

    }
    static void WriteEnvironmentVariables(OctoVersionInfo octoVersionInfo)
    {
        // OCTOVERSION_ddd=fff

        const string prefix = ConfigurationBootstrapper.EnvironmentVariablePrefix;
        var properties = octoVersionInfo.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);

        // https://docs.github.com/en/actions/reference/workflow-commands-for-github-actions#environment-files
        // The outgoing environment variables must be written to a temporary file (identified by the $GITHUB_ENV environment
        // variable, which changes for every step in a workflow) which is then parsed. That file must also be UTF-8 or it will fail.
        var gitHubSetEnvFilePath = System.Environment.GetEnvironmentVariable(GitHubActionsEnvTempFileEnvironmentVariableName);

        if (gitHubSetEnvFilePath != null)
        {
            GitHubActionsLogSink.Log(LogEventLevel.Information, $"Writing version variables to {GitHubActionsEnvTempFileEnvironmentVariableName} file ({gitHubSetEnvFilePath}) for '{nameof(GitHubActionsOutputFormatter)}'.");
            using var streamWriter = File.AppendText(gitHubSetEnvFilePath);
            foreach (var property in properties)
            {
                var environmentVariableKey = $"{prefix}{property.Name}";
                var value = property.GetValue(octoVersionInfo)?.ToString() ?? string.Empty;
                streamWriter.WriteLine($"{environmentVariableKey}={value}");
            }
        }
        else
        {
            GitHubActionsLogSink.Log(LogEventLevel.Warning, $"Unable to write GitVersion variables to ${GitHubActionsEnvTempFileEnvironmentVariableName} because the environment variable is not set.");
        }
    }
}