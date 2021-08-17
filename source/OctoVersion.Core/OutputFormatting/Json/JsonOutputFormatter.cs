using System;
using Newtonsoft.Json;
using OctoVersion.Core.Logging;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Json
{
    public class JsonOutputFormatter : IOutputFormatter
    {
        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public ILogEventSink LogSink { get; } = new NullSink();
        public string Name => "Json";

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            var json = JsonConvert.SerializeObject(octoVersionInfo, Settings);
            System.Console.WriteLine(json);
        }

        public bool MatchesRuntimeEnvironment()
        {
            return false;
        }
    }
}