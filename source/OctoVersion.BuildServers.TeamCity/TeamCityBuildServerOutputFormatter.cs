using System;
using OctoVersion.Contracts;
using Serilog.Core;

namespace OctoVersion.BuildServers.TeamCity
{
    public class TeamCityBuildServerOutputFormatter : IBuildServerOutputFormatter
    {
        public const string BuildServerName = "TeamCity";

        public string SupportedBuildServer => BuildServerName;

        public ILogEventSink LogSink { get; } = new TeamCityLogSink();

        public void WriteVersionInformation(VersionInfo versionInfo)
        {
            //##teamcity[buildNumber '<new build number>']
            var message = $"##teamcity[buildNumber '{versionInfo}']";
            Console.WriteLine(message);
        }
    }
}