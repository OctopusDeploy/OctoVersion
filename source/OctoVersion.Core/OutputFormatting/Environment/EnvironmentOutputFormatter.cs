using System;
using System.Collections.Generic;
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
        foreach (var line in GetPropertiesToWrite(octoVersionInfo))
            System.Console.WriteLine(line);
    }

    internal static IEnumerable<string> GetPropertiesToWrite(OctoVersionInfo octoVersionInfo)
    {
        const string prefix = ConfigurationBootstrapper.EnvironmentVariablePrefix;

        var properties = octoVersionInfo.GetProperties();
        foreach (var property in properties)
        {
            var key = $"{prefix}{property.Name}";
            var line = $"{key}={property.Value}";
            yield return line;
        }
    }

    public bool MatchesRuntimeEnvironment()
    {
        return false;
    }
}