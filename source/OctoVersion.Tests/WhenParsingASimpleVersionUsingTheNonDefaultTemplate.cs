using System;
using System.Collections.Generic;
using OctoVersion.Core.VersionNumberCalculation;
using OctoVersion.Core.VersionTemplates;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests
{
    public class WhenParsingASimpleVersionUsingTheNonDefaultTemplate
    {
        [Theory]
        [MemberData(nameof(TestCases))]
        public void TheResultingVersionShouldBeCorrect(string input, SimpleVersion expected)
        {
            new VersionParser("{major}.{minor}.{build}-{preReleaseTag}").TryParseSimpleVersion(input).ShouldBe(expected);
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { "0.0.0", new SimpleVersion(0, 0, 0, 0) };
            yield return new object[] { "0.0", new SimpleVersion(0, 0, 0, 0) };
            yield return new object[] { "0", new SimpleVersion(0, 0, 0, 0) };
            yield return new object[] { "1", new SimpleVersion(1, 0, 0, 0) };
            yield return new object[] { "1.2", new SimpleVersion(1, 2, 0, 0) };
            yield return new object[] { "1.2.3", new SimpleVersion(1, 2, 0, 3) };
            yield return new object[] { "1.2.3-alpha", null };
            yield return new object[] { "1+some-build-info", new SimpleVersion(1, 0, 0, 0) };
            yield return new object[] { "1.2+some-build-info", new SimpleVersion(1, 2, 0, 0) };
            yield return new object[] { "1.2.3+some-build-info", new SimpleVersion(1, 2, 0, 3) };
            yield return new object[] { "v0.1.0", new SimpleVersion(0, 1, 0, 0) };
            yield return new object[] { "x0.1.0", null };
        }
    }
}