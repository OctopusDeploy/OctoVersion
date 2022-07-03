using System;
using System.Linq;

namespace OctoVersion.Core.VersionNumberCalculation;

public class SimpleVersion : IComparable
{
    public SimpleVersion(int major, int minor, int patch)
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
        if (!(obj is SimpleVersion other)) return -1;

        if (Major != other.Major) return Major.CompareTo(other.Major);
        if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
        return Patch.CompareTo(other.Patch);
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }

    public static SimpleVersion? TryParse(string versionString)
    {
        try
        {
            if (versionString.StartsWith("v"))
                versionString = versionString.Substring(1);

            var indexOfEndOfDigits = versionString.IndexOfAny("-+".ToCharArray());
            if (indexOfEndOfDigits > 0)
            {
                var suffix = versionString.Substring(indexOfEndOfDigits);
                if (suffix.StartsWith("-")) return null; // No pre-release versions - they don't contribute to our version calculations

                versionString = versionString.Substring(0, indexOfEndOfDigits);
            }

            var versionNumberTokens = versionString.Split(new[] { "." }, StringSplitOptions.None);

            var majorToken = versionNumberTokens.First();
            var minorToken = versionNumberTokens.Skip(1).FirstOrDefault();
            var patchToken = versionNumberTokens.Skip(2).FirstOrDefault();
            var major = majorToken != null ? int.Parse(majorToken) : 0;
            var minor = minorToken != null ? int.Parse(minorToken) : 0;
            var patch = patchToken != null ? int.Parse(patchToken) : 0;
            return new SimpleVersion(major, minor, patch);
        }
        catch (Exception)
        {
            return null;
        }
    }
}