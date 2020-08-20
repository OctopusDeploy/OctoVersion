using System;
using System.IO;
using System.Linq;
using OctoVersion.Core;
using OctoVersion.Core.VersionNumberCalculation;
using OctoVersion.Tool.Configuration;
using OctoVersion.Tool.OutputFormatting.Console;
using OctoVersion.Tool.OutputFormatting.Json;
using OctoVersion.Tool.OutputFormatting.TeamCity;
using Serilog;

namespace OctoVersion.Tool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>();
            var outputFormatters = LoadBuildServerOutputFormatters(appSettings.OutputFormats);
            LogBootstrapper.Bootstrap(configuration, lc =>
            {
                foreach (var outputFormatter in outputFormatters) lc.WriteTo.Sink(outputFormatter.LogSink);

                // Special case: if we're writing to the console then use LiterateConsole
                if (outputFormatters.OfType<ConsoleOutputFormatter>().Any()) lc.WriteTo.LiterateConsole();
            });

            var currentDirectory = Directory.GetCurrentDirectory();
            Log.Debug("Executing in {Directory}", currentDirectory);
            Log.Debug("Running OctoVersion with {@AppSettings}", appSettings);
            foreach (var outputFormatter in outputFormatters)
                Log.Debug("Writing build output using {OutputFormatter}", outputFormatter.GetType().Name);

            VersionInfo version;
            using (Log.Logger.BeginTimedOperation("Calculating version"))
            {
                var versionCalculatorFactory = new VersionCalculatorFactory(currentDirectory);
                var calculator = versionCalculatorFactory.Create();
                version = calculator.GetVersion();
                Log.Information("Calculated version {Version}", version);
            }

            var structuredOutput = new StructuredOutputFactory(appSettings.CurrentBranch, appSettings.NonPreReleaseTags,
                appSettings.NonPreReleaseTagsRegex, appSettings.BuildMetadata).Create(version);

            foreach (var outputFormatter in outputFormatters)
                outputFormatter.Write(structuredOutput);
        }

        private static IOutputFormatter[] LoadBuildServerOutputFormatters(string[] outputFormatterNames)
        {
            var allFormatters = new IOutputFormatter[]
                {new ConsoleOutputFormatter(), new JsonOutputFormatter(), new TeamCityOutputFormatter()};

            var formatters = allFormatters
                .Where(f => outputFormatterNames.Any(n =>
                    f.GetType().Name.Equals($"{n}OutputFormatter", StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            return formatters;
        }
    }
}