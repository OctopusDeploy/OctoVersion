using System;
using System.Text.RegularExpressions;

namespace OctoVersion.Core;

public class OctoVersionInfo : SemanticVersion
{
    //https://semver.org/spec/v2.0.0.html#spec-item-9
    static readonly Regex InvalidPreReleaseCharacters = new("[^0-9A-Za-z-\\.]", RegexOptions.Compiled);

    //https://semver.org/spec/v2.0.0.html#spec-item-10
    static readonly Regex InvalidBuildMetadataCharacters = new("[^0-9A-Za-z-\\.]", RegexOptions.Compiled);

    public OctoVersionInfo(
        int major,
        int minor,
        int patch,
        string preReleaseTag,
        string buildMetadata) : base(major,
        minor,
        patch,
        preReleaseTag,
        buildMetadata)
    {
    }

    public OctoVersionInfo(SemanticVersion semanticVersion) : base(
        semanticVersion.Major,
        semanticVersion.Minor,
        semanticVersion.Patch,
        semanticVersion.PreReleaseTag,
        semanticVersion.BuildMetadata)
    {
    }

    public string PreReleaseTagWithDash => string.IsNullOrWhiteSpace(PreReleaseTag) ? string.Empty : $"-{InvalidPreReleaseCharacters.Replace(PreReleaseTag, "-")}";
    public string MajorMinorPatch => $"{Major}.{Minor}.{Patch}";
    public string BuildMetadataWithPlus => string.IsNullOrWhiteSpace(BuildMetadata) ? string.Empty : $"+{InvalidBuildMetadataCharacters.Replace(BuildMetadata, "-")}";
    public string FullSemVer => $"{MajorMinorPatch}{PreReleaseTagWithDash}";
    public string InformationalVersion => $"{MajorMinorPatch}{PreReleaseTagWithDash}{BuildMetadataWithPlus}";
    string NuGetCompatiblePreReleaseWithDash => PreReleaseTagWithDash.Substring(0, Math.Min(PreReleaseTagWithDash.Length, 20));
    public string NuGetVersion => $"{MajorMinorPatch}{NuGetCompatiblePreReleaseWithDash}";

    public override string ToString()
    {
        return FullSemVer;
    }
}