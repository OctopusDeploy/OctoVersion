using System.Reflection;
using OctoVersion.Core;
using Serilog.Core;

namespace OctoVersion.Tool.OutputFormatting.TeamCity
{
    public class TeamCityOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; } = new TeamCityLogSink();

        public void Write(StructuredOutput structuredOutput)
        {
            WriteBuildNumber(structuredOutput);
            WriteEnvironmentVariables(structuredOutput);
        }

        private static void WriteBuildNumber(StructuredOutput structuredOutput)
        {
            //##teamcity[buildNumber '<new build number>']
            var message = $"##teamcity[buildNumber '{structuredOutput.FullSemVer}']";
            System.Console.WriteLine(message);
        }

        private static void WriteEnvironmentVariables(StructuredOutput structuredOutput)
        {
            // ##teamcity[setParameter name='ddd' value='fff']

            const string prefix = ConfigurationBootstrapper.EnvironmentVariablePrefix;
            var properties = structuredOutput.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var key = $"env.{prefix}{property.Name}";
                var value = property.GetValue(structuredOutput)?.ToString() ?? string.Empty;
                var message = $"##teamcity[setParameter name='{key}' value='{value}']";
                System.Console.WriteLine(message);
            }
        }
    }
}