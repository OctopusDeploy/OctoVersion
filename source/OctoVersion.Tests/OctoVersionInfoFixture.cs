using System;
using System.Linq;
using OctoVersion.Core;
using OctoVersion.Core.VersionNumberCalculation;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class OctoVersionInfoFixture
{
    [Fact]
    public void GetPropertiesReadsPropertiesFromBothOctoVersionInfoAndBaseClass()
    {
        var octoVersionInfo = new OctoVersionInfo(1,
            2,
            3,
            string.Empty,
            string.Empty);

        var properties = octoVersionInfo.GetProperties().ToArray();

        properties.ShouldContain(("FullSemVer", "1.2.3"), "we should read properties from OctoVersionInfo class");
        properties.ShouldContain(("Major", "1"), "we should read properties from the base class, Semanticversion");
    }

    [Fact]
    public void ToStringRepresentationShowsTheFullSemVer()
    {
        var sampleData = new SampleDataBuilder()
            .WithNonPreReleaseTags(new[] { "refs/heads/main" })
            .WithNonPreReleaseTagsRegex(string.Empty)
            .WithCurrentBranch("refs/heads/main")
            .WithCurrentSha("a1b2c3d4e5")
            .WithVersion(new SimpleVersion(1, 2, 3))
            .WithExpectedInformationalVersion("1.2.3+Branch.main.Sha.a1b2c3d4e5");
        var octoVersionInfo = sampleData.Build().ToOctoVersion();

        octoVersionInfo.ToString().ShouldBe("1.2.3", "we should get the full sem ver as the default string representation");
    }
}