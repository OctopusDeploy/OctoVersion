using System;
using System.Linq;

namespace OctoVersion.Contracts
{
    internal static class VersionInfoParser
    {
        public static VersionInfo? TryParse(string versionString)
        {
            try
            {
                var (major, minor, revision) = ExtractVersionNumbers(versionString);
                var preReleaseToken = ExtractPreReleaseToken(versionString);
                var buildMetadataToken = ExtractBuildMetadataToken(versionString);
                return new VersionInfo(major, minor, revision, preReleaseToken, buildMetadataToken);
            }
            catch (Exception)
            {
                //TODO This is so very YOLO. It's going to need a whole lot more love...
                return null;
            }
        }

        private static (int, int, int) ExtractVersionNumbers(string versionString)
        {
            var versionNumberToken = versionString.Split(new[] {"-"}, StringSplitOptions.RemoveEmptyEntries).First();
            versionNumberToken = versionNumberToken.Split(new[] {"+"}, StringSplitOptions.RemoveEmptyEntries).First();
            var tokens = versionNumberToken.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);
            var majorToken = tokens.FirstOrDefault();
            var minorToken = tokens.Skip(1).FirstOrDefault();
            var revisionToken = tokens.Skip(2).FirstOrDefault();

            var major = majorToken != null ? int.Parse(majorToken) : 0;
            var minor = minorToken != null ? int.Parse(minorToken) : 0;
            var revision = revisionToken != null ? int.Parse(revisionToken) : 0;

            return (major, minor, revision);
        }

        private static string ExtractPreReleaseToken(string versionString)
        {
            string preReleaseToken = string.Empty;

            var hyphenIndex = versionString.IndexOf('-');
            if (hyphenIndex >= 0)
            {
                preReleaseToken = versionString.Substring(hyphenIndex);

                var plusIndex = preReleaseToken.IndexOf('+');
                if (plusIndex >= 0) preReleaseToken = preReleaseToken.Substring(0, plusIndex);
            }

            return preReleaseToken;
        }

        private static string ExtractBuildMetadataToken(string versionString)
        {
            string buildMetadataToken = string.Empty;

            var hyphenIndex = versionString.IndexOf('+');
            if (hyphenIndex >= 0) buildMetadataToken = versionString.Substring(hyphenIndex);

            return buildMetadataToken;
        }
    }
}