using System;
using System.Reflection;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Environment;

public class EnvironmentOutputFormatter : IOutputFormatter
{
    public EnvironmentOutputFormatter(AppSettings appSettings)
    {
    }

    public ILogEventSink LogSink { get; } = new NullSink();
    public string Name => "Environment";

    public bool SuppressDefaultConsoleOutput => false; //we'd still like to keep the standard console output logging

    public void Write(OctoVersionInfo octoVersionInfo)
    {
        const string prefix = ConfigurationBootstrapper.EnvironmentVariablePrefix;

        var properties = octoVersionInfo.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var key = $"{prefix}{property.Name}";
            var value = property.GetValue(octoVersionInfo)?.ToString() ?? string.Empty;
            var line = $"{key}={value}";
            System.Console.WriteLine(line);
        }
    }

    public bool MatchesRuntimeEnvironment()
    {
        return false;
    }
}