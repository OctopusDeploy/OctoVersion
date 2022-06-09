using System;
using System.Linq;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

[Collection("TheseTestsModifyEnvironmentVariables")] //Cant run these in parallel, as they modify global state (environment variables)
public class OutputFormattersProviderFixture : IDisposable
{
    readonly string? originalTeamCityVersionEnvVar;
    readonly string? originalGitHubActionsEnvVar;

    public OutputFormattersProviderFixture()
    {
        // Backup any current values of env vars used for platform detection
        originalTeamCityVersionEnvVar = Environment.GetEnvironmentVariable("TEAMCITY_VERSION");
        originalGitHubActionsEnvVar = Environment.GetEnvironmentVariable("GITHUB_ACTIONS");

        // Clear env vars used for platform detection
        Environment.SetEnvironmentVariable("TEAMCITY_VERSION", null);
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", null);
    }

    public void Dispose()
    {
        // Restore previous values
        Environment.SetEnvironmentVariable("TEAMCITY_VERSION", originalTeamCityVersionEnvVar);
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", originalGitHubActionsEnvVar);
    }

    [Fact]
    public void DefaultsToConsoleIfNoneRequested()
    {
        var appSettings = new AppSettings
        {
            DetectEnvironment = false,
            OutputFormats = Array.Empty<string>()
        };
        var results = new OutputFormattersProvider().GetFormatters(appSettings).Select(x => x.Name).ToArray();
        results.ShouldBeEquivalentTo(new[] { "Console" }, "it should default to safe and show some output");
    }

    [Fact]
    public void UsesRequestedOutputFormatter()
    {
        var appSettings = new AppSettings
        {
            DetectEnvironment = false,
            OutputFormats = new[] { "Json" }
        };
        var results = new OutputFormattersProvider().GetFormatters(appSettings).Select(x => x.Name).ToArray();
        results.ShouldBeEquivalentTo(new[] { "Json" }, "it should use the requested output format");
    }

    [Fact]
    public void SupportsMultipleRequestedOutputFormatter()
    {
        var appSettings = new AppSettings
        {
            DetectEnvironment = false,
            OutputFormats = new[] { "Json", "Environment" }
        };
        var results = new OutputFormattersProvider().GetFormatters(appSettings).Select(x => x.Name).ToArray();
        results.ShouldBeEquivalentTo(new[] { "Json", "Environment" }, "it should allow multiple output formats");
    }

    [Fact]
    public void DetectsTeamCityFromEnvironmentIfRequested()
    {
        Environment.SetEnvironmentVariable("TEAMCITY_VERSION", "1.2.3");
        var appSettings = new AppSettings
        {
            DetectEnvironment = true,
            OutputFormats = new[] { "Json" }
        };
        var results = new OutputFormattersProvider().GetFormatters(appSettings).Select(x => x.Name).ToArray();
        results.ShouldBeEquivalentTo(new[] { "TeamCity" }, "it should have prioritised auto-detection over the provided value");
    }

    [Fact]
    public void DetectsGitHubActionsFromEnvironmentIfRequested()
    {
        Environment.SetEnvironmentVariable("GITHUB_ACTIONS", "true");
        var appSettings = new AppSettings
        {
            DetectEnvironment = true,
            OutputFormats = new[] { "Json" }
        };
        var results = new OutputFormattersProvider().GetFormatters(appSettings).Select(x => x.Name).ToArray();
        results.ShouldBeEquivalentTo(new[] { "GitHubActions" }, "it should have prioritised auto-detection over the provided value");
    }
}