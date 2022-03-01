using System;

namespace OctoVersion.Core
{
    public class SemanticVersion : IComparable
    {
        public SemanticVersion(int major,
            int minor,
            int? patch,
            string? preReleaseTag,
            int? build,
            string buildMetadata)
        {
            Major = major;
            Minor = minor;
            Patch = patch.GetValueOrDefault();
            Build = build.GetValueOrDefault();
            PreReleaseTag = preReleaseTag ?? string.Empty;
            BuildMetadata = buildMetadata;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public int Build { get; }
        public string PreReleaseTag { get; }
        public string BuildMetadata { get; }

        public int CompareTo(object obj)
        {
            if (!(obj is SemanticVersion other)) return -1;

            if (Major != other.Major) return Major.CompareTo(other.Major);
            if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
            if (Patch != other.Patch) return Patch.CompareTo(other.Patch);
            if (Build != other.Build) return Build.CompareTo(other.Build);
            if (PreReleaseTag != other.PreReleaseTag) return string.Compare(PreReleaseTag, other.PreReleaseTag, StringComparison.Ordinal);
            // IMPORTANT: According to the SemVer 2.0 standard, we do not compare based on the build metadata.
            return 0;
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}.{Build}";
        }
    }
}