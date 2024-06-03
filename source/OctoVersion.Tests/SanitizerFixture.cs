using System;
using System.Collections.Generic;
using OctoVersion.Core;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class SanitizerFixture
{
    [Theory]
    [MemberData(nameof(TestCases))]
    public void OutputsShouldBeAsExpected(string input, string expected)
    {
        new Sanitizer().Sanitize(input).ShouldBe(expected);
    }

    public static IEnumerable<object[]> TestCases()
    {
        yield return new object[] { "", "" };
        yield return new object[] { "foo", "foo" };
        yield return new object[] { "foo-bar", "foo-bar" };
        yield return new object[] { "foo+bar", "foo-bar" }; // The + character indicates the start of build metadata
        yield return new object[] { "release/1.2.3", "release-1.2.3" };
        yield return new object[] { "release/1..2", "release-1.2" };
        yield return new object[] { "release/1...2", "release-1.2" };
        yield return new object[] { ".leading-and-trailing-", "leading-and-trailing" };
        yield return new object[] { "dots..dashes--slashes//and::colons", "dots.dashes-slashes-and-colons" };
    }
}