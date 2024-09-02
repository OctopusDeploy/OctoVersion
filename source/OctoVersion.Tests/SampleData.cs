using System;
using OctoVersion.Core;
using OctoVersion.Core.VersionNumberCalculation;

namespace OctoVersion.Tests;

public class SampleData
{
    public SampleData(
        string[]? nonPreReleaseTags,
        string? nonPreReleaseTagsRegex,
        int? overriddenMajorVersion,
        int? overriddenMinorVersion,
        int? overriddenPatchVersion,
        string? currentBranch,
        string? currentSha,
        string? overriddenBuildMetadata,
        SimpleVersion? version,
        string? expectedInformationalVersion,
        string? expectedFullSemVer)
    {
        NonPreReleaseTags = nonPreReleaseTags ?? throw new ArgumentNullException(nameof(nonPreReleaseTags));
        NonPreReleaseTagsRegex = nonPreReleaseTagsRegex ?? throw new ArgumentNullException(nameof(nonPreReleaseTagsRegex));
        OverriddenMajorVersion = overriddenMajorVersion;
        OverriddenMinorVersion = overriddenMinorVersion;
        OverriddenPatchVersion = overriddenPatchVersion;
        CurrentBranch = currentBranch ?? throw new ArgumentNullException(nameof(currentBranch));
        CurrentSha = currentSha ?? throw new ArgumentNullException(nameof(currentSha));
        OverriddenBuildMetadata = overriddenBuildMetadata;
        Version = version ?? throw new ArgumentNullException(nameof(version));
        ExpectedInformationalVersion = expectedInformationalVersion;
        ExpectedFullSemVer = expectedFullSemVer;
    }

    public string[] NonPreReleaseTags { get; }
    public string NonPreReleaseTagsRegex { get; }
    public int? OverriddenMajorVersion { get; }
    public int? OverriddenMinorVersion { get; }
    public int? OverriddenPatchVersion { get; }
    public string CurrentBranch { get; }
    public string CurrentSha { get; }
    public string? OverriddenBuildMetadata { get; }
    public string? ExpectedInformationalVersion { get; }
    public string? ExpectedFullSemVer { get; }
    public SimpleVersion Version { get; }

    public OctoVersionInfo ToOctoVersion()
    {
        var factory = new StructuredOutputFactory(NonPreReleaseTags,
            NonPreReleaseTagsRegex,
            OverriddenMajorVersion,
            OverriddenMinorVersion,
            OverriddenPatchVersion,
            CurrentBranch,
            CurrentSha,
            OverriddenBuildMetadata);
        return factory.Create(Version);
    }
}