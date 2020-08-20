using System;

namespace OctoVersion.Core
{
    public class VersionInfo : IComparable
    {
        public VersionInfo(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        public int CompareTo(object obj)
        {
            if (!(obj is VersionInfo other)) return 0;

            if (Major != other.Major) return Major.CompareTo(other.Major);
            if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
            return Patch.CompareTo(other.Patch);
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public static VersionInfo? TryParse(string versionString)
        {
            return VersionInfoParser.TryParse(versionString);
        }
    }
}