using System;
using System.Collections.Generic;
using OctoVersion.Core;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class FullyQualifiedBranchNameFlattenerFixture
{
    [Theory]
    [MemberData(nameof(TestCases))]
    public void OutputsShouldBeAsExpected(string input, string expected)
    {
        var flattener = new FullyQualifiedBranchFlattener();

        flattener.Flatten(input).ShouldBe(expected);
    }

    public static IEnumerable<object[]> TestCases()
    {
        yield return new object[] { "", "" };
        yield return new object[] { "foo", "foo" };
        yield return new object[] { "foo-bar", "foo-bar" };
        yield return new object[] { "release/1.2.3", "release/1.2.3" };
        yield return new object[] { "main", "main" };
        yield return new object[] { "refs/heads/main", "main" };
        yield return new object[] { "refs/tags/1.2.3", "1.2.3" };
        yield return new object[] { "refs/pull/1/head", "pull/1/head" };
        yield return new object[] { "refs/pull/1/merge", "pull/1/merge" };
    }
}