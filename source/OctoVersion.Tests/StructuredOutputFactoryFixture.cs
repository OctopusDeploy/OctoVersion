using System;
using System.Collections.Generic;
using System.Linq;
using OctoVersion.Core;
using OctoVersion.Core.VersionNumberCalculation;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class StructuredOutputFactoryFixture
{
    [Theory]
    [MemberData(nameof(XUnitFormattedTestCases))]
    public void CalculatesOctoVersionCorrectly(SampleData sampleData)
    {
        var factory = new StructuredOutputFactory(sampleData.NonPreReleaseTags,
            sampleData.NonPreReleaseTagsRegex,
            sampleData.OverriddenMajorVersion,
            sampleData.OverriddenMinorVersion,
            sampleData.OverriddenPatchVersion,
            sampleData.CurrentBranch,
            sampleData.CurrentSha,
            sampleData.OverriddenBuildMetadata);
        var result = factory.Create(sampleData.Version);
        result.InformationalVersion.ShouldBe(sampleData.ExpectedInformationalVersion);
        result.FullSemVer.ShouldBe(sampleData.ExpectedFullSemVer);
    }

    //map from a sane type into what xunit expects
    public static IEnumerable<object[]> XUnitFormattedTestCases()
    {
        return TestCases().Select(sampleData => new object[] { sampleData });
    }

    static IEnumerable<SampleData> TestCases()
    {
        SampleData ForDefaultScenario()
        {
            return new SampleData
            {
                NonPreReleaseTags = new[] { "refs/heads/main" },
                NonPreReleaseTagsRegex = string.Empty,
                OverriddenMajorVersion = null,
                OverriddenMinorVersion = null,
                OverriddenPatchVersion = null,
                CurrentBranch = "refs/heads/main",
                CurrentSha = "a1b2c3d4e5",
                OverriddenBuildMetadata = null,
                Version = new SimpleVersion(1, 2, 3),
                ExpectedInformationalVersion = "1.2.3+Branch.main.Sha.a1b2c3d4e5"
            };
        }

        yield return ForDefaultScenario()
            .ExpectAnInformationalVersionOf("1.2.3+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.3");
        yield return ForDefaultScenario()
            .WithOverriddenMajorVersion(9)
            .ExpectAnInformationalVersionOf("9.2.3+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("9.2.3");
        yield return ForDefaultScenario()
            .WithOverriddenMinorVersion(9)
            .ExpectAnInformationalVersionOf("1.9.3+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.9.3");
        yield return ForDefaultScenario()
            .WithOverriddenPatchVersion(9)
            .ExpectAnInformationalVersionOf("1.2.9+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.9");
        yield return ForDefaultScenario()
            .WithOverriddenBuildMetadata("custom build meta/data")
            .ExpectAnInformationalVersionOf("1.2.3+custom-build-meta-data")
            .ExpectAFullSemVerOf("1.2.3");
        yield return ForDefaultScenario()
            .WithCurrentSha("aaabbbccc")
            .ExpectAnInformationalVersionOf("1.2.3+Branch.main.Sha.aaabbbccc")
            .ExpectAFullSemVerOf("1.2.3");
        yield return ForDefaultScenario()
            .WithVersion(new SimpleVersion(9, 8, 7))
            .ExpectAnInformationalVersionOf("9.8.7+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("9.8.7");
        yield return ForDefaultScenario()
            .WithNonPreReleaseTags(new[] { "refs/heads/trunk" })
            .ExpectAnInformationalVersionOf("1.2.3-main+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.3-main");
        yield return ForDefaultScenario()
            .WithNonPreReleaseTags(new string[0])
            .WithNonPreReleaseTagsRegex("refs/heads/m.*")
            .ExpectAnInformationalVersionOf("1.2.3+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.3");
        yield return ForDefaultScenario()
            .WithNonPreReleaseTags(new string[0])
            .WithNonPreReleaseTagsRegex("refs/heads/m.*")
            .WithCurrentBranch("refs/heads/feature/versioning")
            .ExpectAnInformationalVersionOf("1.2.3-feature-versioning+Branch.feature-versioning.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.3-feature-versioning");
    }
}

public class SampleData
{
    public string[] NonPreReleaseTags { get; set; }
    public string NonPreReleaseTagsRegex { get; set; }
    public int? OverriddenMajorVersion { get; set; }
    public int? OverriddenMinorVersion { get; set; }
    public int? OverriddenPatchVersion { get; set; }
    public string CurrentBranch { get; set; }
    public string CurrentSha { get; set; }
    public string OverriddenBuildMetadata { get; set; }
    public string ExpectedInformationalVersion { get; set; }
    public string ExpectedFullSemVer { get; set; }
    public SimpleVersion Version { get; set; }

    public SampleData WithNonPreReleaseTags(string[] nonPreReleaseTags)
    {
        NonPreReleaseTags = nonPreReleaseTags;
        return this;
    }

    public SampleData WithNonPreReleaseTagsRegex(string nonPreReleaseTagsRegex)
    {
        NonPreReleaseTagsRegex = nonPreReleaseTagsRegex;
        return this;
    }

    public SampleData WithOverriddenMajorVersion(int? overriddenMajorVersion)
    {
        OverriddenMajorVersion = overriddenMajorVersion;
        return this;
    }

    public SampleData WithOverriddenMinorVersion(int? overriddenMinorVersion)
    {
        OverriddenMinorVersion = overriddenMinorVersion;
        return this;
    }

    public SampleData WithOverriddenPatchVersion(int? overriddenPatchVersion)
    {
        OverriddenPatchVersion = overriddenPatchVersion;
        return this;
    }

    public SampleData WithCurrentBranch(string currentBranch)
    {
        CurrentBranch = currentBranch;
        return this;
    }

    public SampleData WithCurrentSha(string currentSha)
    {
        CurrentSha = currentSha;
        return this;
    }

    public SampleData WithOverriddenBuildMetadata(string overriddenBuildMetadata)
    {
        OverriddenBuildMetadata = overriddenBuildMetadata;
        return this;
    }

    public SampleData WithVersion(SimpleVersion version)
    {
        Version = version;
        return this;
    }

    public SampleData ExpectAnInformationalVersionOf(string expected)
    {
        ExpectedInformationalVersion = expected;
        return this;
    }

    public SampleData ExpectAFullSemVerOf(string expected)
    {
        ExpectedFullSemVer = expected;
        return this;
    }
}