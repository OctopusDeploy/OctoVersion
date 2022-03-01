using System;

namespace OctoVersion.Core.VersionNumberCalculation
{
    public class SimpleVersion : IComparable
    {
        public SimpleVersion(int major, int minor, int? patch, int? build)
        {
            Major = major;
            Minor = minor;
            Patch = patch.GetValueOrDefault();
            Build = build.GetValueOrDefault();
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        public int Build { get; }

        public int CompareTo(object obj)
        {
            if (!(obj is SimpleVersion other)) return -1;

            if (Major != other.Major) return Major.CompareTo(other.Major);
            if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
            if (Patch != other.Patch) return Patch.CompareTo(other.Patch);
            return Build.CompareTo(other.Build);
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}.{Build}";
        }
    }
}