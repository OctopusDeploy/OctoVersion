using System;
using System.Reflection;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Environment
{
    public class EnvironmentOutputFormatter : IOutputFormatter
    {
        public string Name => "Environment";

        public EnvironmentOutputFormatter(AppSettings appSettings)
        {
        }

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

        public void ConfigureLogSink(LoggerConfiguration lc) => lc.WriteTo.Sink(new NullSink());
    }
}
