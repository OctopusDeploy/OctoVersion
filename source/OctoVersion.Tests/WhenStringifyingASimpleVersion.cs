using System;
using System.Collections.Generic;
using OctoVersion.Core;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class WhenStringifyingASemanticVersion
{
    [Theory]
    [MemberData(nameof(TestCases))]
    public void TheOutputShouldBeCorrect(SemanticVersion version, string expected)
    {
        new OctoVersionInfo(version).ToString().ShouldBe(expected);
    }

    public static IEnumerable<object[]> TestCases()
    {
        yield return new object[]
        {
            new SemanticVersion(0,
                0,
                0,
                string.Empty,
                string.Empty),
            "0.0.0"
        };
        yield return new object[]
        {
            new SemanticVersion(1,
                2,
                3,
                string.Empty,
                string.Empty),
            "1.2.3"
        };
        yield return new object[]
        {
            new SemanticVersion(1,
                2,
                3,
                "pre",
                string.Empty),
            "1.2.3-pre"
        };
        yield return new object[]
        {
            new SemanticVersion(1,
                2,
                3,
                string.Empty,
                "build"),
            "1.2.3"
        };
        yield return new object[]
        {
            new SemanticVersion(1,
                2,
                3,
                "pre",
                "build"),
            "1.2.3-pre"
        };
    }
}