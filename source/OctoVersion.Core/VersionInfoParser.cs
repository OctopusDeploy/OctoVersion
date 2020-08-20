using System;
using System.Linq;

namespace OctoVersion.Core
{
    internal static class VersionInfoParser
    {
        public static VersionInfo? TryParse(string versionString)
        {
            try
            {
                var (major, minor, revision) = ExtractVersionNumbers(versionString);
                return new VersionInfo(major, minor, revision);
            }
            catch (Exception)
            {
                //TODO This is so very YOLO. It's going to need a whole lot more love...
                return null;
            }
        }

        private static (int, int, int) ExtractVersionNumbers(string versionString)
        {
            var versionNumberToken = versionString;

            // Strip out build metadata first. If that contains a hyphen, it's fine - it's safe to ignore.
            versionNumberToken = versionNumberToken.Split(new[] {"+"}, StringSplitOptions.RemoveEmptyEntries).First();

            // We've stripped out build metadata, so if there's a remaining hyphen then it's a prerelease package, which is not supported.
            if (versionNumberToken.Contains("-")) throw new Exception("Prelease versions are not supported");

            versionNumberToken = versionNumberToken.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries).First();

            var tokens = versionNumberToken.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);
            var majorToken = tokens.FirstOrDefault();
            var minorToken = tokens.Skip(1).FirstOrDefault();
            var revisionToken = tokens.Skip(2).FirstOrDefault();

            var major = majorToken != null ? int.Parse(majorToken) : 0;
            var minor = minorToken != null ? int.Parse(minorToken) : 0;
            var revision = revisionToken != null ? int.Parse(revisionToken) : 0;

            return (major, minor, revision);
        }
    }
}