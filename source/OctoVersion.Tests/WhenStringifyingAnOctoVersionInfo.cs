using System;
using System.Collections.Generic;
using OctoVersion.Core;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class WhenStringifyingAnOctoVersionInfo
{
    static readonly OctoVersionInfo AllVersionPartsSetTo0 = new(0,
        0,
        0,
        "",
        "");

    static readonly OctoVersionInfo AllVersionPartsSetToValues = new(1,
        2,
        3,
        "",
        "");

    static readonly OctoVersionInfo VersionWithPreReleaseTag = new(1,
        2,
        3,
        "pre",
        "");

    static readonly OctoVersionInfo VersionWithPreReleaseTagAndBuildMetadata = new(1,
        2,
        3,
        "pre",
        "build");

    static readonly OctoVersionInfo VersionWithLongPreReleaseTagAndBuildMetadata = new(2021,
        1,
        3,
        "mark-genericDocumentStore",
        "Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550");

    static readonly OctoVersionInfo VersionWithLongPreReleaseTagWithSlashesAndBuildMetadata = new(2021,
        1,
        3,
        "dependabot/npm_and_yarn/source/TentacleArmy.Web/ini-1.3.6",
        "Branch.dependabot/npm_and_yarn/source/TentacleArmy.Web/ini-1.3.6.Sha.069392d9d2d37ddb6009998b92e70963badcc666");

    //xunit is blurgh - IEnumerable of object[]? ick.
    public static IEnumerable<object[]> PreReleaseTagWithDashTestCases()
    {
        //format is input/output/because
        yield return new object[] { AllVersionPartsSetTo0, "", "there is no pre-release tag" };
        yield return new object[] { AllVersionPartsSetToValues, "", "there is no pre-release tag" };
        yield return new object[] { VersionWithPreReleaseTag, "-pre", "it should append the pre-release tag" };
        yield return new object[] { VersionWithPreReleaseTagAndBuildMetadata, "-pre", "it should append the pre-release tag, but not the build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagAndBuildMetadata, "-mark-genericDocumentStore", "it should add the pre-release tag, and ignore the build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagWithSlashesAndBuildMetadata, "-dependabot-npm-and-yarn-source-TentacleArmy.Web-ini-1.3.6", "it should add the pre-release tag, and ignore the build metadata" };
    }

    [Theory]
    [MemberData(nameof(PreReleaseTagWithDashTestCases))]
    public void ThePreReleaseTagWithDashShouldBeCorrect(OctoVersionInfo input, string expected, string because)
    {
        input.PreReleaseTagWithDash.ShouldBe(expected, because);
    }

    public static IEnumerable<object[]> MajorMinorPatchTestCases()
    {
        //format is input/output/because
        yield return new object[] { AllVersionPartsSetTo0, "0.0.0", "it should concat major.minor.patch" };
        yield return new object[] { AllVersionPartsSetToValues, "1.2.3", "it should concat major.minor.patch" };
        yield return new object[] { VersionWithPreReleaseTag, "1.2.3", "it should concat major.minor.patch and ignore the pre-release tag" };
        yield return new object[] { VersionWithPreReleaseTagAndBuildMetadata, "1.2.3", "it should concat major.minor.patch and ignore the pre-release tag and build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagAndBuildMetadata, "2021.1.3", "it should concat major.minor.patch and ignore the pre-release tag and build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagWithSlashesAndBuildMetadata, "2021.1.3", "it should concat major.minor.patch and ignore the pre-release tag and build metadata" };
    }

    [Theory]
    [MemberData(nameof(MajorMinorPatchTestCases))]
    public void TheMajorMinorPatchShouldBeCorrect(OctoVersionInfo input, string expected, string because)
    {
        input.MajorMinorPatch.ShouldBe(expected, because);
    }

    public static IEnumerable<object[]> BuildMetadataWithPlusTestCases()
    {
        //format is input/output/because
        yield return new object[] { AllVersionPartsSetTo0, "", "there is no build metadata" };
        yield return new object[] { AllVersionPartsSetToValues, "", "there is no build metadata" };
        yield return new object[] { VersionWithPreReleaseTag, "", "there is no build metadata" };
        yield return new object[] { VersionWithPreReleaseTagAndBuildMetadata, "+build", "it should return the build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagAndBuildMetadata, "+Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550", "it should return the build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagWithSlashesAndBuildMetadata, "+Branch.dependabot-npm-and-yarn-source-TentacleArmy.Web-ini-1.3.6.Sha.069392d9d2d37ddb6009998b92e70963badcc666", "it should return the build metadata" };
    }

    [Theory]
    [MemberData(nameof(BuildMetadataWithPlusTestCases))]
    public void TheBuildMetadataWithPlusShouldBeCorrect(OctoVersionInfo input, string expected, string because)
    {
        input.BuildMetadataWithPlus.ShouldBe(expected, because);
    }

    public static IEnumerable<object[]> FullSemVerTestCases()
    {
        //format is input/output/because
        yield return new object[] { AllVersionPartsSetTo0, "0.0.0", "it should return the major.minor.patch" };
        yield return new object[] { AllVersionPartsSetToValues, "1.2.3", "it should return the major.minor.patch" };
        yield return new object[] { VersionWithPreReleaseTag, "1.2.3-pre", "it should return the major.minor.patch and pre-release" };
        yield return new object[] { VersionWithPreReleaseTagAndBuildMetadata, "1.2.3-pre", "it should return the major.minor.patch and pre-release but not the build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagAndBuildMetadata, "2021.1.3-mark-genericDocumentStore", "it should return the major.minor.patch and pre-release but not the build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagWithSlashesAndBuildMetadata, "2021.1.3-dependabot-npm-and-yarn-source-TentacleArmy.Web-ini-1.3.6", "it should return the major.minor.patch and pre-release but not the build metadata" };
    }

    [Theory]
    [MemberData(nameof(FullSemVerTestCases))]
    public void TheFullSemVerShouldBeCorrect(OctoVersionInfo input, string expected, string because)
    {
        input.FullSemVer.ShouldBe(expected, because);
    }

    public static IEnumerable<object[]> NuGetVersionTestCases()
    {
        //format is input/output/because
        yield return new object[] { AllVersionPartsSetTo0, "0.0.0", "it should grab major.minor.patch" };
        yield return new object[] { AllVersionPartsSetToValues, "1.2.3", "it should grab major.minor.patch" };
        yield return new object[] { VersionWithPreReleaseTag, "1.2.3-pre", "it should append the pre-release tag" };
        yield return new object[] { VersionWithPreReleaseTagAndBuildMetadata, "1.2.3-pre", "it should return the major.minor.patch and pre-release but not the build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagAndBuildMetadata, "2021.1.3-mark-genericDocumen", "it should return the major.minor.patch and trim the pre-release to 20 chars" };
        yield return new object[] { VersionWithLongPreReleaseTagWithSlashesAndBuildMetadata, "2021.1.3-dependabot-npm-and-", "it should return the major.minor.patch and trim the pre-release to 20 chars" };
    }

    [Theory]
    [MemberData(nameof(NuGetVersionTestCases))]
    public void TheNuGetVersionShouldBeCorrect(OctoVersionInfo input, string expected, string because)
    {
        input.NuGetVersion.ShouldBe(expected, because);
    }

    public static IEnumerable<object[]> InformationalVersionTestCases()
    {
        //format is input/output/because
        yield return new object[] { AllVersionPartsSetTo0, "0.0.0", "it should grab major.minor.patch" };
        yield return new object[] { AllVersionPartsSetToValues, "1.2.3", "it should grab major.minor.patch" };
        yield return new object[] { VersionWithPreReleaseTag, "1.2.3-pre", "it should append the pre-release tag" };
        yield return new object[] { VersionWithPreReleaseTagAndBuildMetadata, "1.2.3-pre+build", "it should return the major.minor.patch and pre-release and build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagAndBuildMetadata, "2021.1.3-mark-genericDocumentStore+Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550", "it should return the major.minor.patch and pre-release and build metadata" };
        yield return new object[] { VersionWithLongPreReleaseTagWithSlashesAndBuildMetadata, "2021.1.3-dependabot-npm-and-yarn-source-TentacleArmy.Web-ini-1.3.6+Branch.dependabot-npm-and-yarn-source-TentacleArmy.Web-ini-1.3.6.Sha.069392d9d2d37ddb6009998b92e70963badcc666", "it should return the major.minor.patch and pre-release and build metadata" };
    }

    [Theory]
    [MemberData(nameof(InformationalVersionTestCases))]
    public void TheInformationalVersionShouldBeCorrect(OctoVersionInfo input, string expected, string because)
    {
        input.InformationalVersion.ShouldBe(expected, because);
    }
}