using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.VersionNumberCalculation;
using Serilog;

namespace OctoVersion.Core;

public class OctoVersionRunner
{
    static readonly string ApplicationVersion = typeof(OctoVersionRunner).Assembly.GetCustomAttributes<AssemblyInformationalVersionAttribute>().Single().InformationalVersion;

    readonly AppSettings appSettings;
    readonly IConfigurationRoot configuration;
    readonly Action<LoggerConfiguration> additionalLogConfiguration;

    public OctoVersionRunner(AppSettings appSettings, IConfigurationRoot configuration)
        : this(appSettings, configuration, loggerConfiguration => { })
    {
    }

    public OctoVersionRunner(AppSettings appSettings, IConfigurationRoot configuration, Action<LoggerConfiguration> additionalLogConfiguration)
    {
        this.appSettings = appSettings;
        this.configuration = configuration;
        this.additionalLogConfiguration = additionalLogConfiguration;
    }

    public void Run(out OctoVersionInfo versionInfo)
    {
        var outputFormatters = new OutputFormattersProvider().GetFormatters(appSettings);
        LogBootstrapper.Bootstrap(configuration,
            lc =>
            {
                foreach (var outputFormatter in outputFormatters) lc.WriteTo.Sink(outputFormatter.LogSink);

                // Special case: wire up LiterateConsole unless any formatters have said not to
                if (!outputFormatters.Any(f => f.SuppressDefaultConsoleOutput)) lc.WriteTo.LiterateConsole();
                additionalLogConfiguration(lc);
            });
        Log.Debug("Running OctoVersion {OctoVersionVersion} with {@AppSettings}", ApplicationVersion, appSettings);

        var currentDirectory = Directory.GetCurrentDirectory();
        Log.Debug("Executing in {Directory}", currentDirectory);

        foreach (var outputFormatter in outputFormatters)
            Log.Debug("Writing build output using {OutputFormatter}", outputFormatter.GetType().Name);

        if (!string.IsNullOrWhiteSpace(appSettings.FullSemVer))
        {
            Log.Information("Adopting previously-provided version information {FullSemVer}. Not calculating a new version number.", appSettings.FullSemVer);
            var semanticVersion = SemanticVersion.TryParse(appSettings.FullSemVer);
            if (semanticVersion == null) throw new Exception("Failed to parse semantic version string");

            versionInfo = new OctoVersionInfo(semanticVersion);
        }
        else
        {
            using (Log.Logger.BeginTimedOperation("Calculating version"))
            {
                var repositorySearchPath = string.IsNullOrWhiteSpace(appSettings.RepositoryPath)
                    ? currentDirectory
                    : appSettings.RepositoryPath;

                var versionCalculatorFactory = new VersionCalculatorFactory(repositorySearchPath);
                var calculator = versionCalculatorFactory.Create();
                var version = calculator.GetVersion();
                var currentSha = calculator.CurrentCommitHash;
                versionInfo = new StructuredOutputFactory(appSettings.NonPreReleaseTags,
                        appSettings.NonPreReleaseTagsRegex,
                        appSettings.Major,
                        appSettings.Minor,
                        appSettings.Patch,
                        appSettings.CurrentBranch,
                        currentSha,
                        appSettings.BuildMetadata)
                    .Create(version);
            }
        }

        Log.Information("Version is {FullSemVer}", versionInfo.FullSemVer);

        foreach (var outputFormatter in outputFormatters)
            outputFormatter.Write(versionInfo);
    }
}