using System;
using OctoVersion.Core.ExtensionMethods;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class WhenParsingCommitMessages
{
    [Theory]
    [InlineData("This is a commit message", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("This is a commit message +semver: breaking", true)]
    [InlineData("This is a commit message +semver:breaking", true)]
    [InlineData("This is a commit message +SemVer:Breaking", true)]
    [InlineData("This is a commit message +semver: major", true)]
    [InlineData("This is a commit message +semver:major", true)]
    [InlineData("This is a commit message +SemVer:Major", true)]
    public void ParsesMajorVersionBumpCorrectly(string? commitMessage, bool expectedResult)
    {
        var result = commitMessage.CommitMessageBumpsMajorVersion();
        result.ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("This is a commit message", false)]
    [InlineData("This is a commit message +semver: feature", true)]
    [InlineData("This is a commit message +semver:feature", true)]
    [InlineData("This is a commit message +SemVer:Feature", true)]
    [InlineData("This is a commit message +semver: minor", true)]
    [InlineData("This is a commit message +semver:minor", true)]
    [InlineData("This is a commit message +SemVer:Minor", true)]
    public void ParsesMinorVersionBumpCorrectly(string? commitMessage, bool expectedResult)
    {
        var result = commitMessage.CommitMessageBumpsMinorVersion();
        result.ShouldBe(expectedResult);
    }
}