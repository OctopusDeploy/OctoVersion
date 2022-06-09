using System;
using Microsoft.Extensions.Configuration;
using OctoVersion.Core.ExtensionMethods;
using Serilog;

namespace OctoVersion.Core;

public static class LogBootstrapper
{
    public static void Bootstrap(IConfigurationRoot configuration, Action<LoggerConfiguration> additionalConfiguration)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "OctoVersion")
            .Enrich.WithProperty("ApplicationVersion", typeof(LogBootstrapper).Assembly.GetName().Version)
            .Apply(additionalConfiguration)
            .CreateLogger();
    }
}