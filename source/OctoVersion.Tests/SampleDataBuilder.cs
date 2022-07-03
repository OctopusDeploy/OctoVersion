using System;
using OctoVersion.Core.VersionNumberCalculation;

namespace OctoVersion.Tests;

public class SampleDataBuilder
{
    string[]? nonPreReleaseTags;
    string? nonPreReleaseTagsRegex;
    int? overriddenMajorVersion;
    int? overriddenMinorVersion;
    int? overriddenPatchVersion;
    string? currentBranch;
    string? currentSha;
    string? overriddenBuildMetadata;
    SimpleVersion? version;
    string? expectedInformationalVersion;
    string? expectedFullSemVer;

    public SampleDataBuilder WithNonPreReleaseTags(string[] tags)
    {
        nonPreReleaseTags = tags;
        return this;
    }

    public SampleDataBuilder WithNonPreReleaseTagsRegex(string regex)
    {
        nonPreReleaseTagsRegex = regex;
        return this;
    }

    public SampleDataBuilder WithOverriddenMajorVersion(int? majorVersion)
    {
        overriddenMajorVersion = majorVersion;
        return this;
    }

    public SampleDataBuilder WithOverriddenMinorVersion(int? minorVersion)
    {
        overriddenMinorVersion = minorVersion;
        return this;
    }

    public SampleDataBuilder WithOverriddenPatchVersion(int? patchVersion)
    {
        overriddenPatchVersion = patchVersion;
        return this;
    }

    public SampleDataBuilder WithCurrentBranch(string branch)
    {
        currentBranch = branch;
        return this;
    }

    public SampleDataBuilder WithCurrentSha(string sha)
    {
        currentSha = sha;
        return this;
    }

    public SampleDataBuilder WithOverriddenBuildMetadata(string? buildMetadata)
    {
        overriddenBuildMetadata = buildMetadata;
        return this;
    }

    public SampleDataBuilder WithVersion(SimpleVersion newVersion)
    {
        version = newVersion;
        return this;
    }

    public SampleDataBuilder WithExpectedInformationalVersion(string expected)
    {
        expectedInformationalVersion = expected;
        return this;
    }

    public SampleDataBuilder ExpectAnInformationalVersionOf(string expected)
    {
        expectedInformationalVersion = expected;
        return this;
    }

    public SampleDataBuilder ExpectAFullSemVerOf(string expected)
    {
        expectedFullSemVer = expected;
        return this;
    }

    public SampleData Build()
    {
        return new SampleData(
            nonPreReleaseTags,
            nonPreReleaseTagsRegex,
            overriddenMajorVersion,
            overriddenMinorVersion,
            overriddenPatchVersion,
            currentBranch,
            currentSha,
            overriddenBuildMetadata,
            version,
            expectedInformationalVersion,
            expectedFullSemVer);
    }
}