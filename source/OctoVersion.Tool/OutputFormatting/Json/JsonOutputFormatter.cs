using System;
using Newtonsoft.Json;
using OctoVersion.Core;
using OctoVersion.Tool.Logging;
using Serilog.Core;

namespace OctoVersion.Tool.OutputFormatting.Json
{
    public class JsonOutputFormatter : IOutputFormatter
    {
        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public ILogEventSink LogSink { get; } = new NullSink();

        public void Write(StructuredOutput structuredOutput)
        {
            var json = JsonConvert.SerializeObject(structuredOutput, Settings);
            System.Console.WriteLine(json);
        }
    }
}