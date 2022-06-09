using System;
using System.Linq;

namespace OctoVersion.Core;

public class SemanticVersion : IComparable
{
    public SemanticVersion(int major,
        int minor,
        int patch,
        string preReleaseTag,
        string buildMetadata)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreReleaseTag = preReleaseTag;
        BuildMetadata = buildMetadata;
    }

    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }
    public string PreReleaseTag { get; }
    public string BuildMetadata { get; }

    public static SemanticVersion? TryParse(string fullSemVer)
    {
        if (fullSemVer == null) throw new ArgumentNullException(nameof(fullSemVer));

        try
        {
            var indexOfEndOfBuildDigits = fullSemVer.IndexOfAny("-+".ToCharArray());
            if (indexOfEndOfBuildDigits < 0) indexOfEndOfBuildDigits = fullSemVer.Length;

            var versionNumberDigitGroupings = fullSemVer.Substring(0, indexOfEndOfBuildDigits);
            var versionNumberTokens = versionNumberDigitGroupings.Split(new[] { "." }, StringSplitOptions.None);

            var majorToken = versionNumberTokens.First();
            var minorToken = versionNumberTokens.Skip(1).FirstOrDefault();
            var patchToken = versionNumberTokens.Skip(2).FirstOrDefault();
            var major = majorToken != null ? int.Parse(majorToken) : 0;
            var minor = minorToken != null ? int.Parse(minorToken) : 0;
            var patch = patchToken != null ? int.Parse(patchToken) : 0;

            var remainderAfterDigitGroups = fullSemVer.Substring(indexOfEndOfBuildDigits);

            string preReleaseTag;
            string buildMetadata;
            if (remainderAfterDigitGroups == string.Empty)
            {
                preReleaseTag = string.Empty;
                buildMetadata = string.Empty;
            }
            else if (remainderAfterDigitGroups.StartsWith("-"))
            {
                // we have a prerelease tag
                var indexOfFirstPlus = remainderAfterDigitGroups.IndexOf("+");
                if (indexOfFirstPlus < 0)
                {
                    preReleaseTag = remainderAfterDigitGroups.Substring(1);
                    buildMetadata = string.Empty;
                }
                else
                {
                    preReleaseTag = remainderAfterDigitGroups.Substring(1, indexOfFirstPlus - 1);
                    buildMetadata = remainderAfterDigitGroups.Substring(indexOfFirstPlus + 1);
                }
            }
            else
            {
                // we just have build metadata
                preReleaseTag = string.Empty;
                buildMetadata = remainderAfterDigitGroups.Substring(1);
            }

            return new SemanticVersion(major,
                minor,
                patch,
                preReleaseTag,
                buildMetadata);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public int CompareTo(object obj)
    {
        if (!(obj is SemanticVersion other)) return -1;

        if (Major != other.Major) return Major.CompareTo(other.Major);
        if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
        if (Patch != other.Patch) return Patch.CompareTo(other.Patch);
        if (PreReleaseTag != other.PreReleaseTag) return string.Compare(PreReleaseTag, other.PreReleaseTag, StringComparison.Ordinal);
        // IMPORTANT: According to the SemVer 2.0 standard, we do not compare based on the build metadata.
        return 0;
    }
}