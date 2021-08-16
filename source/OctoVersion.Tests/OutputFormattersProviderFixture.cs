using System;
using System.Collections;
using System.Linq;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests
{
    [Collection("TheseTestsModifyEnvironmentVariables")] //Cant run these in parallel, as they modify global state (environment variables)
    public class OutputFormattersProviderFixture : IDisposable
    {
        readonly string? originalTeamCityVersionEnvVar;

        public OutputFormattersProviderFixture()
        {
            originalTeamCityVersionEnvVar = Environment.GetEnvironmentVariable("TEAMCITY_VERSION");
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable("TEAMCITY_VERSION", originalTeamCityVersionEnvVar);
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
            results.ShouldBeEquivalentTo(new [] { "Console" }, "it should default to safe and show some output");
        }

        [Fact]
        public void UsesRequestedOutputFormatter()
        {
            var appSettings = new AppSettings
            {
                DetectEnvironment = false,
                OutputFormats = new [] { "Json" }
            };
            var results = new OutputFormattersProvider().GetFormatters(appSettings).Select(x => x.Name).ToArray();
            results.ShouldBeEquivalentTo(new [] { "Json" }, "it should use the requested output format");
        }

        [Fact]
        public void SupportsMultipleRequestedOutputFormatter()
        {
            var appSettings = new AppSettings
            {
                DetectEnvironment = false,
                OutputFormats = new [] { "Json", "Environment" }
            };
            var results = new OutputFormattersProvider().GetFormatters(appSettings).Select(x => x.Name).ToArray();
            results.ShouldBeEquivalentTo(new [] { "Json", "Environment" }, "it should allow multiple output formats");
        }

        [Fact]
        public void DetectsTeamCityFromEnvironmentIfRequested()
        {
            Environment.SetEnvironmentVariable("TEAMCITY_VERSION", "1.2.3");
            var appSettings = new AppSettings
            {
                DetectEnvironment = true,
                OutputFormats = new [] { "Json" }
            };
            var results = new OutputFormattersProvider().GetFormatters(appSettings).Select(x => x.Name).ToArray();
            results.ShouldBeEquivalentTo(new [] { "TeamCity" }, "it should have prioritised auto-detection over the provided value");
        }
    }
}
