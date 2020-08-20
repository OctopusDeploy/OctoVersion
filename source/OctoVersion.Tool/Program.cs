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
    internal class Program
    {
        private static void Main(string[] args)
        {
            var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>(args);
            var outputFormatters = LoadBuildServerOutputFormatters(appSettings.OutputFormats);
            LogBootstrapper.Bootstrap(configuration, lc =>
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

            VersionInfo version;
            if (appSettings.Major.HasValue && appSettings.Minor.HasValue && appSettings.Patch.HasValue)
            {
                Log.Information(
                    "Adopting previously-provided version information. Not calculating a new version number.");
                version = new VersionInfo(appSettings.Major.Value, appSettings.Minor.Value, appSettings.Patch.Value);
            }
            else
            {
                using (Log.Logger.BeginTimedOperation("Calculating version"))
                {
                    var versionCalculatorFactory = new VersionCalculatorFactory(repositorySearchPath);
                    var calculator = versionCalculatorFactory.Create();
                    version = calculator.GetVersion();
                }
            }

            var structuredOutput = new StructuredOutputFactory(appSettings.CurrentBranch, appSettings.NonPreReleaseTags,
                    appSettings.NonPreReleaseTagsRegex, appSettings.Major, appSettings.Minor,
                    appSettings.Patch, appSettings.BuildMetadata)
                .Create(version);

            Log.Information("Version is {FullSemVer}", structuredOutput.FullSemVer);

            foreach (var outputFormatter in outputFormatters)
                outputFormatter.Write(structuredOutput);
        }

        private static IOutputFormatter[] LoadBuildServerOutputFormatters(string[] outputFormatterNames)
        {
            var allFormatters = typeof(Program).Assembly.DefinedTypes
                .Where(t => typeof(IOutputFormatter).IsAssignableFrom(t))
                .Where(t => !t.IsInterface)
                .Where(t => !t.IsAbstract)
                .Select(t => (IOutputFormatter) Activator.CreateInstance(t))
                .ToArray();

            var formatters = allFormatters
                .Where(f => outputFormatterNames.Any(n =>
                    f.GetType().Name.Equals($"{n}OutputFormatter", StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            return formatters;
        }
    }
}