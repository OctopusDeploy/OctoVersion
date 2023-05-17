using System;
using System.Collections.Generic;
using System.Linq;
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
        var result = sampleData.ToOctoVersion();
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
        SampleDataBuilder ForDefaultScenario()
        {
            return new SampleDataBuilder()
                .WithNonPreReleaseTags(new[] { "refs/heads/main" })
                .WithNonPreReleaseTagsRegex(string.Empty)
                .WithOverriddenMajorVersion(null)
                .WithOverriddenMinorVersion(null)
                .WithOverriddenPatchVersion(null)
                .WithCurrentBranch("refs/heads/main")
                .WithCurrentSha("a1b2c3d4e5")
                .WithOverriddenBuildMetadata(null)
                .WithVersion(new SimpleVersion(1, 2, 3))
                .WithExpectedInformationalVersion("1.2.3+Branch.main.Sha.a1b2c3d4e5");
        }

        yield return ForDefaultScenario()
            .ExpectAnInformationalVersionOf("1.2.3+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.3")
            .Build();
        yield return ForDefaultScenario()
            .WithOverriddenMajorVersion(9)
            .ExpectAnInformationalVersionOf("9.2.3+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("9.2.3")
            .Build();
        yield return ForDefaultScenario()
            .WithOverriddenMinorVersion(9)
            .ExpectAnInformationalVersionOf("1.9.3+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.9.3")
            .Build();
        yield return ForDefaultScenario()
            .WithOverriddenPatchVersion(9)
            .ExpectAnInformationalVersionOf("1.2.9+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.9")
            .Build();
        yield return ForDefaultScenario()
            .WithOverriddenBuildMetadata("custom build meta/data")
            .ExpectAnInformationalVersionOf("1.2.3+custom-build-meta-data")
            .ExpectAFullSemVerOf("1.2.3")
            .Build();
        yield return ForDefaultScenario()
            .WithCurrentSha("aaabbbccc")
            .ExpectAnInformationalVersionOf("1.2.3+Branch.main.Sha.aaabbbccc")
            .ExpectAFullSemVerOf("1.2.3")
            .Build();
        yield return ForDefaultScenario()
            .WithVersion(new SimpleVersion(9, 8, 7))
            .ExpectAnInformationalVersionOf("9.8.7+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("9.8.7")
            .Build();
        yield return ForDefaultScenario()
            .WithNonPreReleaseTags(new[] { "refs/heads/trunk" })
            .ExpectAnInformationalVersionOf("1.2.3-main+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.3-main")
            .Build();
        yield return ForDefaultScenario()
            .WithNonPreReleaseTags(Array.Empty<string>())
            .WithNonPreReleaseTagsRegex("refs/heads/m.*")
            .ExpectAnInformationalVersionOf("1.2.3+Branch.main.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.3")
            .Build();
        yield return ForDefaultScenario()
            .WithNonPreReleaseTags(Array.Empty<string>())
            .WithNonPreReleaseTagsRegex("refs/heads/m.*")
            .WithCurrentBranch("refs/heads/feature/versioning")
            .ExpectAnInformationalVersionOf("1.2.3-feature-versioning+Branch.feature-versioning.Sha.a1b2c3d4e5")
            .ExpectAFullSemVerOf("1.2.3-feature-versioning")
            .Build();
    }
}