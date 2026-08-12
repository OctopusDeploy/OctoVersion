using System;
using System.Collections.Generic;
using System.Reflection;
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
        string buildMetadata,
        int? maxVersionLength) : base(major,
        minor,
        patch,
        CapPreReleaseTag(major,
            minor,
            patch,
            preReleaseTag,
            maxVersionLength),
        buildMetadata)
    {
    }

    public OctoVersionInfo(SemanticVersion semanticVersion, int? maxVersionLength) : this(
        semanticVersion.Major,
        semanticVersion.Minor,
        semanticVersion.Patch,
        semanticVersion.PreReleaseTag,
        semanticVersion.BuildMetadata,
        maxVersionLength)
    {
    }

    public string PreReleaseTagWithDash => string.IsNullOrWhiteSpace(PreReleaseTag) ? string.Empty : $"-{InvalidPreReleaseCharacters.Replace(PreReleaseTag, "-")}";
    public string MajorMinorPatch => $"{Major}.{Minor}.{Patch}";
    public string BuildMetadataWithPlus => string.IsNullOrWhiteSpace(BuildMetadata) ? string.Empty : $"+{InvalidBuildMetadataCharacters.Replace(BuildMetadata, "-")}";
    public string FullSemVer => $"{MajorMinorPatch}{PreReleaseTagWithDash}";
    public string InformationalVersion => $"{MajorMinorPatch}{PreReleaseTagWithDash}{BuildMetadataWithPlus}";
    public string NuGetVersion => $"{MajorMinorPatch}{NuGetCompatiblePreReleaseWithDash}";

    string NuGetCompatiblePreReleaseWithDash
    {
        get
        {
            var truncated = PreReleaseTagWithDash.Substring(0, Math.Min(PreReleaseTagWithDash.Length, 20));

            //a trailing dot is an empty pre-release identifier, which is not a valid version
            return truncated.TrimEnd('.');
        }
    }

    /// <summary>
    /// Shortens the pre-release tag until <see cref="FullSemVer" /> fits within <paramref name="maxVersionLength" />.
    /// The numeric version components are never shortened; if they alone exceed the maximum then no pre-release tag is emitted.
    /// </summary>
    static string CapPreReleaseTag(int major,
        int minor,
        int patch,
        string preReleaseTag,
        int? maxVersionLength)
    {
        if (maxVersionLength == null || string.IsNullOrWhiteSpace(preReleaseTag)) return preReleaseTag;

        //the pre-release tag is separated from the numeric components by a dash
        var availableLength = maxVersionLength.Value - $"{major}.{minor}.{patch}".Length - 1;
        if (availableLength >= preReleaseTag.Length) return preReleaseTag;
        if (availableLength <= 0) return string.Empty;

        //a trailing dot is an empty pre-release identifier, which is not a valid version; a trailing dash is just noise
        return preReleaseTag.Substring(0, availableLength).TrimEnd('.', '-');
    }

    public override string ToString()
    {
        return FullSemVer;
    }

    internal IEnumerable<(string Name, string Value)> GetProperties()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(this)?.ToString() ?? string.Empty;
            yield return (property.Name, value);
        }
    }
}