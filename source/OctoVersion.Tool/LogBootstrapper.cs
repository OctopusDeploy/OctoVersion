using System;
using Microsoft.Extensions.Configuration;
using OctoVersion.Core;
using OctoVersion.Core.ExtensionMethods;
using Serilog;

namespace OctoVersion.Tool
{
    public static class LogBootstrapper
    {
        public static void Bootstrap(IConfigurationRoot configuration, Action<LoggerConfiguration> additionalConfiguration)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "OctoVersion")
                .Enrich.WithProperty("ApplicationVersion", typeof(Program).Assembly.GetName().Version)
                .Apply(additionalConfiguration)
                .CreateLogger();
        }
    }
}