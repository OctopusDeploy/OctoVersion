using OctoVersion.Core;
using Serilog.Core;

namespace OctoVersion.Tool.OutputFormatting.TeamCity
{
    public class TeamCityOutputFormatter : IOutputFormatter
    {
        public ILogEventSink LogSink { get; } = new TeamCityLogSink();

        public void Write(StructuredOutput structuredOutput)
        {
            //##teamcity[buildNumber '<new build number>']
            var message = $"##teamcity[buildNumber '{structuredOutput.FullSemVer}']";
            System.Console.WriteLine(message);
        }
    }
}