using System;
using System.IO;
using System.Linq;
using OctoVersion.Core;
using OctoVersion.Core.VersionNumberCalculation;
using OctoVersion.Tool.Configuration;
using OctoVersion.Tool.OutputFormatting.Console;
using Serilog;

namespace OctoVersion.Tool
{
    class Program
    {
        static void Main(string[] args)
        {
            var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>(args);
            var outputFormatters = new OutputFormattersProvider().GetFormatters(appSettings.OutputFormats);
            LogBootstrapper.Bootstrap(configuration,
                lc =>
                {
                    foreach (var outputFormatter in outputFormatters) lc.WriteTo.Sink(outputFormatter.LogSink);

                    // Special case: if we're writing to the console then use LiterateConsole
                    if (outputFormatters.OfType<ConsoleOutputFormatter>().Any()) lc.WriteTo.LiterateConsole();
                });

            var currentDirectory = Directory.GetCurrentDirectory();
            Log.Debug("Executing in {Directory}", currentDirectory);

            var repositorySearchPath = string.IsNullOrWhiteSpace(appSettings.RepositoryPath)
                ? currentDirectory
                : appSettings.RepositoryPath;
            Log.Debug("Running OctoVersion with {@AppSettings}", appSettings);
            foreach (var outputFormatter in outputFormatters)
                Log.Debug("Writing build output using {OutputFormatter}", outputFormatter.GetType().Name);

            StructuredOutput structuredOutput;
            if (appSettings.Major.HasValue && appSettings.Minor.HasValue && appSettings.Patch.HasValue && appSettings.PreReleaseTag != null && appSettings.BuildMetadata != null)
            {
                Log.Information("Adopting previously-provided version information. Not calculating a new version number.");
                structuredOutput = new StructuredOutput(appSettings.Major.Value,
                    appSettings.Minor.Value,
                    appSettings.Patch.Value,
                    appSettings.PreReleaseTag,
                    appSettings.BuildMetadata);
            }
            else
            {
                using (Log.Logger.BeginTimedOperation("Calculating version"))
                {
                    var versionCalculatorFactory = new VersionCalculatorFactory(repositorySearchPath);
                    var calculator = versionCalculatorFactory.Create();
                    var version = calculator.GetVersion();
                    structuredOutput = new StructuredOutputFactory(appSettings.NonPreReleaseTags,
                            appSettings.NonPreReleaseTagsRegex,
                            appSettings.Major,
                            appSettings.Minor,
                            appSettings.Patch,
                            appSettings.CurrentBranch,
                            appSettings.BuildMetadata)
                        .Create(version);
                }
            }

            Log.Information("Version is {FullSemVer}", structuredOutput.FullSemVer);

            foreach (var outputFormatter in outputFormatters)
                outputFormatter.Write(structuredOutput);
        }
    }
}