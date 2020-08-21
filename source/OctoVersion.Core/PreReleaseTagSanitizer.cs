using System.Linq;
using System.Text.RegularExpressions;

namespace OctoVersion.Core
{
    public class PreReleaseTagSanitizer
    {
        private static readonly Regex SafeCharactersRegex = new Regex("[\\w-.]", RegexOptions.Compiled);
        private static readonly Regex DuplicateCharactersRegex = new Regex("-+", RegexOptions.Compiled);
        private static readonly Regex MultipleDotsRegex = new Regex("\\.+", RegexOptions.Compiled);

        public string Sanitize(string input)
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