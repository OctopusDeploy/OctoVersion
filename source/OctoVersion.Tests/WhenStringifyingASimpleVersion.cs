using System;
using System.Collections.Generic;
using OctoVersion.Core;
using OctoVersion.Core.VersionTemplates;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests
{
    public class WhenStringifyingASemanticVersion
    {
        [Theory]
        [MemberData(nameof(TestCases))]
        public void TheOutputShouldBeCorrect(SemanticVersion version, string expected)
        {
            var versionParser = new VersionParser("{major}.{minor}.{patch}-{preReleaseTag}.{build}");
            new OctoVersionInfo(version, versionParser).ToString().ShouldBe(expected);
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[]
            {
                new SemanticVersion(0,
                    0,
                    0,
                    string.Empty,
                    null,
                    string.Empty),
                "0.0.0"
            };
            yield return new object[]
            {
                new SemanticVersion(1,
                    2,
                    3,
                    string.Empty,
                    null,
                    string.Empty),
                "1.2.3"
            };
            yield return new object[]
            {
                new SemanticVersion(1,
                    2,
                    3,
                    "pre",
                    null,
                    string.Empty),
                "1.2.3-pre.0"
            };
            yield return new object[]
            {
                new SemanticVersion(1,
                    2,
                    3,
                    string.Empty,
                    null,
                    "build"),
                "1.2.3"
            };
            yield return new object[]
            {
                new SemanticVersion(1,
                    2,
                    3,
                    "pre",
                    null,
                    "build"),
                "1.2.3-pre.0"
            };
        }
    }
}