using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

[Collection("TheseTestsModifyEnvironmentVariables")] //Cant run these in parallel, as they modify global state (environment variables)
public class ConfigurationBootstrapperFixture : IDisposable
{
    public void Dispose()
    {
        foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
        {
            var key = (string)environmentVariable.Key;
            if (key.StartsWith(ConfigurationBootstrapper.EnvironmentVariablePrefix))
                Environment.SetEnvironmentVariable(key, "");
        }
    }

    [Fact]
    public void ThrowsExceptionWhenNoArgsArePassed()
    {
        var args = new string[0];
        var ex = Assert.Throws<ValidationException>(() => ConfigurationBootstrapper.Bootstrap<AppSettings>(args));
        ex.Message.ShouldBe("At least one of CurrentBranch or FullSemVer must be provided.");
    }

    [Fact]
    public void WhenPassingCurrentBranch()
    {
        var args = new[] { "--CurrentBranch", "main" };
        var (appSettings, _) = ConfigurationBootstrapper.Bootstrap<AppSettings>(args);
        appSettings.CurrentBranch.ShouldBe("main");
        appSettings.NonPreReleaseTags.ShouldBeEquivalentTo(new[] { "main", "master" }); //defaults should be applied
    }

    [Fact]
    public void WhenPassingFullSemVer()
    {
        var args = new[] { "--FullSemVer", "1.0.0" };
        var (appSettings, _) = ConfigurationBootstrapper.Bootstrap<AppSettings>(args);
        appSettings.FullSemVer.ShouldBe("1.0.0");
        appSettings.NonPreReleaseTags.ShouldBeEquivalentTo(new[] { "main", "master" }); //defaults should be applied
    }

    [Theory]
    [MemberData(nameof(OutputFormatTestCases))]
    public void WhenPassingOutputFormats(string[] args, string[] expectedOutputFormats)
    {
        var fullArgs = new[] { "--CurrentBranch", "main" }.Concat(args).ToArray();
        var (appSettings, _) = ConfigurationBootstrapper.Bootstrap<AppSettings>(fullArgs);
        appSettings.OutputFormats.ShouldBeEquivalentTo(expectedOutputFormats);
    }

    public static IEnumerable<object[]> OutputFormatTestCases()
    {
        // sane formats
        yield return new object[] { new[] { "--outputformat", "json" }, new[] { "json" } };
        yield return new object[] { new[] { "--outputformat", "json", "--outputformat", "console" }, new[] { "json", "console" } };

        yield return new object[] { new[] { "--outputformats", "json" }, new[] { "json" } };
        yield return new object[] { new[] { "--outputformats", "json", "--outputformats", "console" }, new[] { "json", "console" } };

        // dotnet configuration formats
        yield return new object[] { new[] { "--outputformat:0", "json" }, new[] { "json" } };
        yield return new object[] { new[] { "--outputformat:0", "json", "--outputformat:1", "console" }, new[] { "json", "console" } };
    }

    [Fact]
    public void WhenEnvironmentVariableAndCommandLineArgExistTheCommandLineArgWins()
    {
        Environment.SetEnvironmentVariable("OCTOVERSION_OutputFormats__0", "TeamCity");
        var fullArgs = new[] { "--CurrentBranch", "main" }.Concat(new[] { "--outputformat", "json" }).ToArray();
        var (appSettings, _) = ConfigurationBootstrapper.Bootstrap<AppSettings>(fullArgs);
        appSettings.OutputFormats.ShouldBeEquivalentTo(new[] { "json" });
    }

    [Theory]
    [MemberData(nameof(OutputFormatEnvironmentVariableTestCases))]
    public void WhenPassingOutputFormatsViaEnvironmentVariable((string Name, string Value)[] environmentVariables, string[] expectedOutputFormats)
    {
        Environment.SetEnvironmentVariable("OCTOVERSION_CurrentBranch", "main");
        foreach (var (name, value) in environmentVariables)
            Environment.SetEnvironmentVariable(name, value);
        var (appSettings, _) = ConfigurationBootstrapper.Bootstrap<AppSettings>();
        appSettings.OutputFormats.ShouldBeEquivalentTo(expectedOutputFormats);
    }

    public static IEnumerable<object[]> OutputFormatEnvironmentVariableTestCases()
    {
        yield return new object[]
        {
            new[] { new ValueTuple<string, string>("OCTOVERSION_OutputFormats__0", "main") },
            new[] { "main" }
        };
        yield return new object[]
        {
            new[] { new ValueTuple<string, string>("OCTOVERSION_OutputFormats__0", "main"), new ValueTuple<string, string>("OCTOVERSION_OutputFormats__1", "release") },
            new[] { "main", "release" }
        };
    }

    [Theory]
    [MemberData(nameof(NonPreReleaseTagsTestCases))]
    public void WhenPassingNonPreReleaseTags(string[] args, string[] expectedNonPreReleaseTags)
    {
        var fullArgs = new[] { "--CurrentBranch", "main" }.Concat(args).ToArray();
        var (appSettings, _) = ConfigurationBootstrapper.Bootstrap<AppSettings>(fullArgs);
        appSettings.NonPreReleaseTags.ShouldBeEquivalentTo(expectedNonPreReleaseTags);
    }

    public static IEnumerable<object[]> NonPreReleaseTagsTestCases()
    {
        // sane formats
        yield return new object[] { new[] { "--nonprereleasetags", "main" }, new[] { "main" } };
        yield return new object[] { new[] { "--nonprereleasetags", "main", "--nonprereleasetags", "release" }, new[] { "main", "release" } };

        // dotnet configuration formats
        yield return new object[] { new[] { "--nonprereleasetags:0", "main" }, new[] { "main" } };
        yield return new object[] { new[] { "--nonprereleasetags:0", "main", "--nonprereleasetags:1", "release" }, new[] { "main", "release" } };
    }

    [Theory]
    [MemberData(nameof(NonPreReleaseTagsEnvironmentVariableTestCases))]
    public void WhenPassingNonPreReleaseTagsViaEnvironmentVariable((string Name, string Value)[] environmentVariables, string[] expectedNonPreReleaseTags)
    {
        Environment.SetEnvironmentVariable("OCTOVERSION_CurrentBranch", "main");
        foreach (var (name, value) in environmentVariables)
            Environment.SetEnvironmentVariable(name, value);

        var (appSettings, _) = ConfigurationBootstrapper.Bootstrap<AppSettings>();
        appSettings.NonPreReleaseTags.ShouldBeEquivalentTo(expectedNonPreReleaseTags);
    }

    public static IEnumerable<object[]> NonPreReleaseTagsEnvironmentVariableTestCases()
    {
        yield return new object[]
        {
            new[] { new ValueTuple<string, string>("OCTOVERSION_NonPreReleaseTags__0", "main") },
            new[] { "main" }
        };
        yield return new object[]
        {
            new[] { new ValueTuple<string, string>("OCTOVERSION_NonPreReleaseTags__0", "main"), new ValueTuple<string, string>("OCTOVERSION_NonPreReleaseTags__1", "release") },
            new[] { "main", "release" }
        };
    }
}