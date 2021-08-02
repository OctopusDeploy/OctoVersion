﻿using System.IO;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace OctoVersion.Core.OutputFormatting.GitHubActions
{
    public class GitHubActionsOutputFormatter : IOutputFormatter
    {
        public const string GitHubSetEnvTempFileEnvironmentVariableName = "GITHUB_ENV";

        ILogEventSink IOutputFormatter.LogSink => new GitHubActionsLogSink();

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            WriteOutputVariables(octoVersionInfo);
            WriteEnvironmentVariables(octoVersionInfo);
        }

        static void WriteOutputVariables(OctoVersionInfo octoVersionInfo)
        {
            // ::set-output name='OctoVersion_ddd'::'fff']

            var properties = octoVersionInfo.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var configurationVariableKey = $"OctoVersion_{property.Name}";

                var value = property.GetValue(octoVersionInfo)?.ToString() ?? string.Empty;

                var configurationVariableMessage = $"::set-output name={configurationVariableKey}::{value}";
                System.Console.WriteLine(configurationVariableMessage);
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
            var gitHubSetEnvFilePath = System.Environment.GetEnvironmentVariable(GitHubSetEnvTempFileEnvironmentVariableName);

            if (gitHubSetEnvFilePath != null)
            {
                GitHubActionsLogSink.Log(LogEventLevel.Information, $"Writing version variables to {GitHubSetEnvTempFileEnvironmentVariableName} file ({gitHubSetEnvFilePath}) for '{nameof(GitHubActionsOutputFormatter)}'.");
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
                GitHubActionsLogSink.Log(LogEventLevel.Warning, $"Unable to write GitVersion variables to ${GitHubSetEnvTempFileEnvironmentVariableName} because the environment variable is not set.");
            }
        }
    }
}