using System;
using System.Collections.Generic;
using OctoVersion.Core;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class WhenParsingASemanticVersion
{
    [Theory]
    [MemberData(nameof(TestCases))]
    public void TheResultingVersionShouldBeCorrect(string input, SemanticVersion expected)
    {
        var fullSemanticVersionInfo = SemanticVersion.TryParse(input);
        fullSemanticVersionInfo.ShouldBe(expected);

        if (fullSemanticVersionInfo == null) return;

        // Special comparison here because the .CompareTo method in FullSemanticVersion is not allowed to compare based on build metadata
        fullSemanticVersionInfo.BuildMetadata.ShouldBe(expected.BuildMetadata);
    }

    public static IEnumerable<object[]> TestCases()
    {
        yield return new object[] { "", null };
        yield return new object[] { "spit.spot", null };
        yield return new object[] { "1.2.x", null };
        yield return new object[] { "1.x.3", null };
        yield return new object[] { "x.2.3", null };
        yield return new object[] { "1..2", null };
        yield return new object[]
        {
            "0.0.0", new SemanticVersion(0,
                0,
                0,
                string.Empty,
                string.Empty)
        };
        yield return new object[]
        {
            "0.0", new SemanticVersion(0,
                0,
                0,
                string.Empty,
                string.Empty)
        };
        yield return new object[]
        {
            "0", new SemanticVersion(0,
                0,
                0,
                string.Empty,
                string.Empty)
        };
        yield return new object[]
        {
            "1", new SemanticVersion(1,
                0,
                0,
                string.Empty,
                string.Empty)
        };
        yield return new object[]
        {
            "1.2", new SemanticVersion(1,
                2,
                0,
                string.Empty,
                string.Empty)
        };
        yield return new object[]
        {
            "1.2.3", new SemanticVersion(1,
                2,
                3,
                string.Empty,
                string.Empty)
        };
        yield return new object[]
        {
            "1.2.3-alpha", new SemanticVersion(1,
                2,
                3,
                "alpha",
                string.Empty)
        };
        yield return new object[]
        {
            "1+some-build-info", new SemanticVersion(1,
                0,
                0,
                string.Empty,
                "some-build-info")
        };
        yield return new object[]
        {
            "1.2+some-build-info", new SemanticVersion(1,
                2,
                0,
                string.Empty,
                "some-build-info")
        };
        yield return new object[]
        {
            "1.2.3+some-build-info", new SemanticVersion(1,
                2,
                3,
                string.Empty,
                "some-build-info")
        };
        yield return new object[]
        {
            "1.2.3-pre-release+some-build-info", new SemanticVersion(1,
                2,
                3,
                "pre-release",
                "some-build-info")
        };
    }
}