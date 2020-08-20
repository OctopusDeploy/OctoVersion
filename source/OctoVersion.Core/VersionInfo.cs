using System;

namespace OctoVersion.Core
{
    public class VersionInfo : IComparable
    {
        public VersionInfo(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Revision { get; }

        public int CompareTo(object obj)
        {
            if (!(obj is VersionInfo other)) return 0;

            if (Major != other.Major) return Major.CompareTo(other.Major);
            if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
            return Revision.CompareTo(other.Revision);
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Revision}";
        }

        public static VersionInfo? TryParse(string versionString)
        {
            try
            {
                var tokens = versionString.Split(new[] {"."}, StringSplitOptions.None);
                var major = int.Parse(tokens[0]);
                var minor = int.Parse(tokens[1]);
                var revision = int.Parse(tokens[2]);
                return new VersionInfo(major, minor, revision);
            }
            catch (Exception)
            {
                //TODO This is so very YOLO. It's going to need a whole lot more love...
                return null;
            }
        }
    }
}