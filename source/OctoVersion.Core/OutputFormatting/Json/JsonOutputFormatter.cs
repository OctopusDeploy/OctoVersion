using System;
using System.IO;
using Newtonsoft.Json;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Json
{
    public class JsonOutputFormatter : IOutputFormatter
    {
        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public string Name => "Json";

        public JsonOutputFormatter(AppSettings appSettings)
        {
        }

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            var json = JsonConvert.SerializeObject(octoVersionInfo, Settings);
            System.Console.WriteLine(json);
        }

        public bool MatchesRuntimeEnvironment()
        {
            return false;
        }

        public bool SuppressDefaultConsoleOutput => true;
        public ILogEventSink LogSink => new NullSink();
    }
}
