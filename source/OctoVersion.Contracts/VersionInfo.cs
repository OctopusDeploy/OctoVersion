using System;

namespace OctoVersion.Contracts
{
    public class VersionInfo : IComparable
    {
        public VersionInfo(int major, int minor, int revision, string preReleaseTag = "", string buildMetadata = "")
        {
            Major = major;
            Minor = minor;
            Revision = revision;
            PreReleaseTag = preReleaseTag;
            BuildMetadata = buildMetadata;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Revision { get; }
        public string PreReleaseTag { get; }
        public string BuildMetadata { get; }

        public int CompareTo(object obj)
        {
            if (!(obj is VersionInfo other)) return 0;

            if (Major != other.Major) return Major.CompareTo(other.Major);
            if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
            return Revision.CompareTo(other.Revision);
        }

        public override string ToString()
        {
            var preReleaseToken = string.IsNullOrWhiteSpace(PreReleaseTag) ? string.Empty : $"-{PreReleaseTag}";
            var buildMetadataToken = string.IsNullOrWhiteSpace(BuildMetadata) ? string.Empty : $"+{BuildMetadata}";
            return $"{Major}.{Minor}.{Revision}{preReleaseToken}{buildMetadataToken}";
        }

        public static VersionInfo? TryParse(string versionString)
        {
            return VersionInfoParser.TryParse(versionString);
        }
    }
}