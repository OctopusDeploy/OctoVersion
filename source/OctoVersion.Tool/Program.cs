using System;
using System.IO;
using System.Linq;
using OctoVersion.BuildServers.TeamCity;
using OctoVersion.Contracts;
using OctoVersion.Core;
using OctoVersion.Tool.BuildServerOutputFormatting;
using OctoVersion.Tool.Configuration;
using Serilog;

namespace OctoVersion.Tool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var (appSettings, configuration) = ConfigurationBootstrapper.Bootstrap<AppSettings>();
            var outputFormatter = LoadBuildServerOutputFormatter(appSettings.BuildServerOutputFormatter);
            LogBootstrapper.Bootstrap(configuration, lc =>
            {
                lc.WriteTo.Sink(outputFormatter.LogSink);

                // Special case: if we're not writing to any build server then dump to console
                if (outputFormatter is NullBuildServerOutputFormatter)
                    lc.WriteTo.LiterateConsole();
            });

            var currentDirectory = Directory.GetCurrentDirectory();
            Log.Debug("Executing in {Directory}", currentDirectory);
            Log.Debug("Running OctoVersion with {@AppSettings}", appSettings);
            Log.Debug("Writing build output using {BuildServerOutputFormatter}", outputFormatter.GetType().Name);

            VersionInfo version;
            using (Log.Logger.BeginTimedOperation("Calculating version"))
            {
                var versionCalculatorFactory = new VersionCalculatorFactory(currentDirectory,
                    Log.Logger.ForContext<VersionCalculatorFactory>());
                var calculator = versionCalculatorFactory.Create();
                version = calculator.GetVersion();
                Log.Information("Calculated version {Version}", version);
            }

            outputFormatter.WriteVersionInformation(version);
        }

        private static IBuildServerOutputFormatter LoadBuildServerOutputFormatter(string buildServerOutputFormatter)
        {
            var allFormatters = new IBuildServerOutputFormatter[] {new TeamCityBuildServerOutputFormatter()};

            var formatter = allFormatters
                                .FirstOrDefault(f => f.SupportedBuildServer.Equals(buildServerOutputFormatter,
                                    StringComparison.OrdinalIgnoreCase))
                            ?? new NullBuildServerOutputFormatter();

            return formatter;
        }
    }
}