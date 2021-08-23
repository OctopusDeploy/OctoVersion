using System;
using System.IO;
using Newtonsoft.Json;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.Logging;
using Serilog;
using Serilog.Core;

namespace OctoVersion.Core.OutputFormatting.Json
{
    public class JsonFileOutputFormatter : IOutputFormatter
    {
        readonly AppSettings appSettings;

        static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        public string Name => "JsonFile";

        public JsonFileOutputFormatter(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        public void Write(OctoVersionInfo octoVersionInfo)
        {
            if (string.IsNullOrEmpty(appSettings.OutputJsonFile))
            {
                Log.Warning($"{Name} output requested, but no {nameof(appSettings.OutputJsonFile)} provided");
                return;
            }
            Log.Information("Writing versionInfo to {outputJsonFile}", appSettings.OutputJsonFile);
            var json = JsonConvert.SerializeObject(octoVersionInfo, Settings);
            File.WriteAllText(appSettings.OutputJsonFile, json);
        }

        public bool MatchesRuntimeEnvironment()
        {
            return !string.IsNullOrEmpty(appSettings.OutputJsonFile);
        }

        public void ConfigureLogSink(LoggerConfiguration lc) => lc.WriteTo.LiterateConsole();
    }
}
