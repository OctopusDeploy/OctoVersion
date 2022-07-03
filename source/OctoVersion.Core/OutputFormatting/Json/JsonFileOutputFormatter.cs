using System;
using System.IO;
using Newtonsoft.Json;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Json;

public class JsonFileOutputFormatter : IOutputFormatter
{
    static readonly JsonSerializerSettings Settings = new()
    {
        Formatting = Formatting.Indented
    };

    readonly AppSettings appSettings;

    public JsonFileOutputFormatter(AppSettings appSettings)
    {
        this.appSettings = appSettings;
    }

    public ILogEventSink LogSink { get; } = new NullSink();
    public string Name => "JsonFile";

    public bool SuppressDefaultConsoleOutput => false; //we'd still like to keep the standard console output logging

    public void Write(OctoVersionInfo octoVersionInfo)
    {
        if (string.IsNullOrEmpty(appSettings.OutputJsonFile))
        {
            Log.Warning($"{Name} output requested, but no {nameof(appSettings.OutputJsonFile)} provided");
            return;
        }

        Log.Information("Writing version info to {outputJsonFile}", appSettings.OutputJsonFile);
        var json = JsonConvert.SerializeObject(octoVersionInfo, Settings);
        File.WriteAllText(appSettings.OutputJsonFile, json);
    }

    public bool MatchesRuntimeEnvironment()
    {
        return !string.IsNullOrEmpty(appSettings.OutputJsonFile);
    }
}