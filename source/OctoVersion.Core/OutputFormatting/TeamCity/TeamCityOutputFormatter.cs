using System;
using System.Reflection;
using OctoVersion.Core.Configuration;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.TeamCity
{
    public class TeamCityOutputFormatter : IOutputFormatter
    {
        public string Name => "TeamCity";

        public TeamCityOutputFormatter(IAppSettings appSettings)
        {
        }

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            WriteBuildNumber(octoVersionInfo);
            WriteEnvironmentVariables(octoVersionInfo);
        }

        public bool MatchesRuntimeEnvironment()
        {
            return !string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));
        }

        public bool SuppressDefaultConsoleOutput => true;
        public ILogEventSink LogSink => new TeamCityLogSink();

        static void WriteBuildNumber(OctoVersionInfo octoVersionInfo)
        {
            //##teamcity[buildNumber '<new build number>']
            var message = $"##teamcity[buildNumber '{octoVersionInfo.FullSemVer}']";
            System.Console.WriteLine(message);
        }

        static void WriteEnvironmentVariables(OctoVersionInfo octoVersionInfo)
        {
            // ##teamcity[setParameter name='ddd' value='fff']

            const string prefix = ConfigurationBootstrapper.EnvironmentVariablePrefix;
            var properties = octoVersionInfo.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var environmentVariableKey = $"env.{prefix}{property.Name}";
                var configurationVariableKey = $"OctoVersion.{property.Name}";

                var value = property.GetValue(octoVersionInfo)?.ToString() ?? string.Empty;

                var environmentVariableMessage = $"##teamcity[setParameter name='{environmentVariableKey}' value='{value}']";
                System.Console.WriteLine(environmentVariableMessage);

                var configurationVariableMessage = $"##teamcity[setParameter name='{configurationVariableKey}' value='{value}']";
                System.Console.WriteLine(configurationVariableMessage);
            }
        }
    }
}
