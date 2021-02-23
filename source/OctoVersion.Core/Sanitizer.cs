using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OctoVersion.Core
{
    public static class Sanitizer
    {
        static readonly Regex SafeCharactersRegex = new Regex("[\\w-.]", RegexOptions.Compiled);
        static readonly Regex DuplicateCharactersRegex = new Regex("-+", RegexOptions.Compiled);
        static readonly Regex MultipleDotsRegex = new Regex("\\.+", RegexOptions.Compiled);

        public static string Sanitize(string input)
        {
            var preReleaseTag = input;

            preReleaseTag = new string(preReleaseTag
                .Select(c => SafeCharactersRegex.IsMatch(new string(c, 1)) ? c : '-')
                .ToArray());
            preReleaseTag = DuplicateCharactersRegex.Replace(preReleaseTag, "-");
            preReleaseTag = MultipleDotsRegex.Replace(preReleaseTag, ".");
            preReleaseTag = preReleaseTag.Trim(' ', '-', '.');

            return preReleaseTag;
        }
    }
}
