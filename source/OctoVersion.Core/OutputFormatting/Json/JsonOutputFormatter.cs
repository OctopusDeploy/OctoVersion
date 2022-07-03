using System;
using Newtonsoft.Json;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Json;

public class JsonOutputFormatter : IOutputFormatter
{
    static readonly JsonSerializerSettings Settings = new()
    {
        Formatting = Formatting.Indented
    };

    public JsonOutputFormatter(AppSettings appSettings)
    {
    }

    public ILogEventSink LogSink { get; } = new NullSink();
    public string Name => "Json";

    public bool SuppressDefaultConsoleOutput => true; //dont write anything else to console please - it needs to be json

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