using System;
using System.Collections.Generic;
using OctoVersion.Core;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class WhenCappingTheVersionLength
{
    //78 characters of pre-release tag, for a 91 character version
    const string LongBranchPreReleaseTag = "feature-2451-retention-policies-for-nested-project-groups-and-shared-tenancies";
    const string LongBranchBuildMetadata = "Branch.feature-2451-retention-policies-for-nested-project-groups-and-shared-tenancies.Sha.abc1234def5";

    static readonly OctoVersionInfo LongVersionWithNoMaxLength = new(2026,
        3,
        11281,
        LongBranchPreReleaseTag,
        LongBranchBuildMetadata,
        null);

    static readonly OctoVersionInfo LongVersionWithAMaxLengthLongerThanTheVersion = new(2026,
        3,
        11281,
        LongBranchPreReleaseTag,
        LongBranchBuildMetadata,
        200);

    //63 characters is the Kubernetes limit on a metadata label value
    static readonly OctoVersionInfo LongVersionCappedTo63 = new(2026,
        3,
        11281,
        LongBranchPreReleaseTag,
        LongBranchBuildMetadata,
        63);

    static readonly OctoVersionInfo VersionWhereTheCapLandsMidIdentifier = new(2026,
        3,
        11281,
        LongBranchPreReleaseTag,
        LongBranchBuildMetadata,
        40);

    static readonly OctoVersionInfo VersionWhereTheCapLandsOnADash = new(2026,
        3,
        11281,
        LongBranchPreReleaseTag,
        LongBranchBuildMetadata,
        36);

    static readonly OctoVersionInfo VersionWhereTheCapLandsOnADot = new(1,
        1,
        47,
        "renovate-microsoft.aspnetcore.mvc.newtonsoftjson-10.x",
        "Branch.renovate-microsoft.aspnetcore.mvc.newtonsoftjson-10.x.Sha.72529493c8ccfae2784cd604b7784294b03f388d",
        26);

    static readonly OctoVersionInfo VersionWithoutAPreReleaseTag = new(2026,
        3,
        11281,
        "",
        LongBranchBuildMetadata,
        8);

    static readonly OctoVersionInfo VersionCappedBelowTheNumericComponents = new(2026,
        3,
        11281,
        LongBranchPreReleaseTag,
        LongBranchBuildMetadata,
        10);

    public static IEnumerable<object[]> FullSemVerTestCases()
    {
        //format is input/output/because
        yield return new object[]
        {
            LongVersionWithNoMaxLength,
            "2026.3.11281-feature-2451-retention-policies-for-nested-project-groups-and-shared-tenancies",
            "no maximum length was set, so the version is untouched"
        };
        yield return new object[]
        {
            LongVersionWithAMaxLengthLongerThanTheVersion,
            "2026.3.11281-feature-2451-retention-policies-for-nested-project-groups-and-shared-tenancies",
            "the version already fits within the maximum length"
        };
        yield return new object[]
        {
            LongVersionCappedTo63,
            "2026.3.11281-feature-2451-retention-policies-for-nested-project",
            "the pre-release tag should be shortened until the version fits"
        };
        yield return new object[]
        {
            VersionWhereTheCapLandsMidIdentifier,
            "2026.3.11281-feature-2451-retention-poli",
            "the whole budget should be used, even part-way through an identifier"
        };
        yield return new object[]
        {
            VersionWhereTheCapLandsOnADash,
            "2026.3.11281-feature-2451-retention",
            "a shortened pre-release tag should not end with a dash"
        };
        yield return new object[]
        {
            VersionWhereTheCapLandsOnADot,
            "1.1.47-renovate-microsoft",
            "a shortened pre-release tag must not end with a dot"
        };
        yield return new object[]
        {
            VersionWithoutAPreReleaseTag,
            "2026.3.11281",
            "there is no pre-release tag to shorten"
        };
        yield return new object[]
        {
            VersionCappedBelowTheNumericComponents,
            "2026.3.11281",
            "the numeric version components are never shortened, so the pre-release tag is dropped entirely"
        };
    }

    [Theory]
    [MemberData(nameof(FullSemVerTestCases))]
    public void TheFullSemVerShouldBeCorrect(OctoVersionInfo input, string expected, string because)
    {
        input.FullSemVer.ShouldBe(expected, because);
    }

    public static IEnumerable<object[]> PreReleaseTagTestCases()
    {
        //format is input/output/because
        yield return new object[] { LongVersionWithNoMaxLength, LongBranchPreReleaseTag, "no maximum length was set" };
        yield return new object[] { LongVersionCappedTo63, "feature-2451-retention-policies-for-nested-project", "the pre-release tag is where the shortening happens" };
        yield return new object[] { VersionCappedBelowTheNumericComponents, "", "there was no room for a pre-release tag" };
    }

    [Theory]
    [MemberData(nameof(PreReleaseTagTestCases))]
    public void ThePreReleaseTagShouldBeCorrect(OctoVersionInfo input, string expected, string because)
    {
        //the shortened tag is what every output format reports, so that a version re-imported from one run matches the next
        input.PreReleaseTag.ShouldBe(expected, because);
        input.PreReleaseTagWithDash.ShouldBe(expected == string.Empty ? string.Empty : $"-{expected}", because);
    }

    [Fact]
    public void TheInformationalVersionShouldUseTheShortenedPreReleaseTag()
    {
        //build metadata does not count towards the maximum length; it is not part of the version that consumers constrain
        LongVersionCappedTo63.InformationalVersion
            .ShouldBe($"2026.3.11281-feature-2451-retention-policies-for-nested-project+{LongBranchBuildMetadata}");
    }

    [Fact]
    public void TheNuGetVersionShouldStillBeTruncatedToItsOwnLimit()
    {
        //NuGet's own 20 character truncation of the pre-release tag is unchanged, and applies on top of the shortened tag
        LongVersionCappedTo63.NuGetVersion.ShouldBe("2026.3.11281-feature-2451-retent");
    }
}