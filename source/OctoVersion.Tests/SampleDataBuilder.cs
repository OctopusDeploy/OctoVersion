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
        this.nonPreReleaseTags = tags;
        return this;
    }

    public SampleDataBuilder WithNonPreReleaseTagsRegex(string regex)
    {
        this.nonPreReleaseTagsRegex = regex;
        return this;
    }

    public SampleDataBuilder WithOverriddenMajorVersion(int? majorVersion)
    {
        this.overriddenMajorVersion = majorVersion;
        return this;
    }

    public SampleDataBuilder WithOverriddenMinorVersion(int? minorVersion)
    {
        this.overriddenMinorVersion = minorVersion;
        return this;
    }

    public SampleDataBuilder WithOverriddenPatchVersion(int? patchVersion)
    {
        this.overriddenPatchVersion = patchVersion;
        return this;
    }

    public SampleDataBuilder WithCurrentBranch(string branch)
    {
        this.currentBranch = branch;
        return this;
    }

    public SampleDataBuilder WithCurrentSha(string sha)
    {
        this.currentSha = sha;
        return this;
    }

    public SampleDataBuilder WithOverriddenBuildMetadata(string? buildMetadata)
    {
        this.overriddenBuildMetadata = buildMetadata;
        return this;
    }

    public SampleDataBuilder WithVersion(SimpleVersion newVersion)
    {
        this.version = newVersion;
        return this;
    }

    public SampleDataBuilder WithExpectedInformationalVersion(string expected)
    {
        this.expectedInformationalVersion = expected;
        return this;
    }
        
    public SampleDataBuilder ExpectAnInformationalVersionOf(string expected)
    {
        this.expectedInformationalVersion = expected;
        return this;
    }

    public SampleDataBuilder ExpectAFullSemVerOf(string expected)
    {
        this.expectedFullSemVer = expected;
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
