using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.Extensions.Configuration;
using OctoVersion.Core.Configuration;
using OctoVersion.Core.ExtensionMethods;

namespace OctoVersion.Core;

public static class ConfigurationBootstrapper
{
    // ReSharper disable once StringLiteralTypo
    public const string EnvironmentVariablePrefix = "OCTOVERSION_";

    public static (T, IConfigurationRoot) Bootstrap<T>(params string[] args) where T : IAppSettings, new()
    {
        var (appSettings, configuration) = BootstrapWithoutValidation<T>(args);

        var validationContext = new ValidationContext(appSettings);
        Validator.ValidateObject(appSettings, validationContext);

        return (appSettings, configuration);
    }

    public static (T, IConfigurationRoot) BootstrapWithoutValidation<T>(params string[] args) where T : IAppSettings, new()
    {
        var configFilePath = BestEffortConfigFilePath();

        var configuration = new ConfigurationBuilder()
            .Apply(configurationBuilder =>
            {
                if (configFilePath != null) configurationBuilder.AddJsonFile(configFilePath.FullName);
            })
            .AddEnvironmentVariables(EnvironmentVariablePrefix)
            .AddCommandLine(args)
            .Build();

        var appSettings = new T();
        configuration.Bind(appSettings);

        appSettings.ContributeSaneArrayArgs(args);
        appSettings.ApplyDefaultsIfRequired();

        return (appSettings, configuration);
    }

    static FileInfo? BestEffortConfigFilePath()
    {
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory != null)
        {
            var configFilePath = new FileInfo(Path.Combine(directory.FullName, "octoversion.json"));
            if (configFilePath.Exists) return configFilePath;

            directory = directory.Parent;
        }

        return null;
    }
}