using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OctoVersion.Core;

public class Sanitizer
{
    static readonly Regex SafeCharactersRegex = new("[\\w-.]", RegexOptions.Compiled);
    static readonly Regex DuplicateCharactersRegex = new("-+", RegexOptions.Compiled);
    static readonly Regex MultipleDotsRegex = new("\\.+", RegexOptions.Compiled);

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