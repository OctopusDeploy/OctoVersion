using System;
using System.Linq;
using System.Text.RegularExpressions;
using OctoVersion.Core.VersionNumberCalculation;

namespace OctoVersion.Core.VersionTemplates
{
    /// <summary>
    /// Examples
    ///
    /// {major}.{minor}.{build}-{preReleaseTag}
    ///
    /// {major}.{minor}.{patch}-{preReleaseTag}.{build}
    ///
    /// </summary>
    class VersionParser
    {
        //https://semver.org/spec/v2.0.0.html#spec-item-9
        static readonly Regex InvalidPreReleaseCharacters = new Regex("[^0-9A-Za-z-\\.]", RegexOptions.Compiled);

        readonly string[] _tokenMap;

        public VersionParser(string versionTemplate)
        {
            _tokenMap = SplitString(versionTemplate).tokens
                    .Union(new [] { "{metadata}" }) // if there is always an implied metadata token at the end
                    .ToArray();
        }

        public string GetMajorMinorPatch(OctoVersionInfo octoVersionInfo)
        {
            // using the first 3 tokens in the template, which may not actually be Major, Minor, Patch
            return $"{GetValueForToken(octoVersionInfo, _tokenMap[0])}.{GetValueForToken(octoVersionInfo, _tokenMap[1])}.{GetValueForToken(octoVersionInfo, _tokenMap[2])}";
        }

        public string GetPreReleaseTagWithDash(OctoVersionInfo octoVersionInfo)
        {
            var result = octoVersionInfo.PreReleaseTag;

            // append any token that appear after the preReleaseTag in the template
            var indexOfPreReleaseToken = 0;
            while (indexOfPreReleaseToken < _tokenMap.Length - 1 && _tokenMap[indexOfPreReleaseToken] != "{preReleaseTag}")
                indexOfPreReleaseToken++;
            for (var i = indexOfPreReleaseToken + 1; i < _tokenMap.Length - 1; i++)
            {
                result += $".{GetValueForToken(octoVersionInfo, _tokenMap[i])}";
            }

            return $"-{InvalidPreReleaseCharacters.Replace(result, "-")}";
        }

        public string GetNuGetCompatiblePreReleaseWithDash(OctoVersionInfo octoVersionInfo)
        {
            var result = octoVersionInfo.PreReleaseTag;
            if (string.IsNullOrWhiteSpace(result))
                return string.Empty;

            // append any token that appear after the preReleaseTag in the template
            var indexOfPreReleaseToken = 0;
            while (indexOfPreReleaseToken < _tokenMap.Length - 1 && _tokenMap[indexOfPreReleaseToken] != "{preReleaseTag}")
                indexOfPreReleaseToken++;
            var suffix = string.Empty;
            for (var i = indexOfPreReleaseToken + 1; i < _tokenMap.Length - 1; i++)
            {
                suffix += $".{GetValueForToken(octoVersionInfo, _tokenMap[i])}";
            }

            result = result.Substring(0, Math.Min(result.Length, 19 - suffix.Length)) + suffix;

            return $"-{InvalidPreReleaseCharacters.Replace(result, "-")}";
        }

        string GetValueForToken(OctoVersionInfo octoVersionInfo, string token)
        {
            switch (token)
            {
                case "{major}":
                    return octoVersionInfo.Major.ToString();
                case "{minor}":
                    return octoVersionInfo.Minor.ToString();
                case "{patch}":
                    return octoVersionInfo.Patch.ToString();
                case "{preReleaseTag}":
                    return octoVersionInfo.PreReleaseTag.ToString();
                case "{build}":
                    return octoVersionInfo.Build.ToString();
                case "{metadata}":
                    return octoVersionInfo.BuildMetadata.ToString();
            }

            return string.Empty;
        }

        static (string[] tokens, string metadata) SplitString(string template)
        {
            // if there is metadata it must always be at the end. It may contain . and -, so we split based in it first
            var versionPlusMetadata = template.Split(new[] { "+" }, StringSplitOptions.None);

            // the remainder of the version split is then done on what's before the metadata
            var versionTokens = versionPlusMetadata
                .First()
                .Split(new[] { "." }, StringSplitOptions.None)
                .SelectMany(x =>
                {
                    var preReleaseIndex = x.IndexOf("-", StringComparison.OrdinalIgnoreCase);
                    if (preReleaseIndex < 0)
                        return new[] { x };
                    return new[] { x.Substring(0, preReleaseIndex), x.Substring(preReleaseIndex + 1) };
                })
                .ToList();

            var metadata = string.Empty;
            if (versionPlusMetadata.Length > 1)
                metadata = versionPlusMetadata.Skip(1).First();

            return (versionTokens.ToArray(), metadata);
        }

        int GetMarkerIndexInTemplate(string marker)
        {
            for (int i = 0; i < _tokenMap.Length; i++)
                if (_tokenMap[i].Contains(marker))
                    return i;
            return -1;
        }

        string GetValue(string[] values, string marker)
        {
            var index = GetMarkerIndexInTemplate(marker);
            if (index < 0 || index >= values.Length)
                return string.Empty;
            return values[index];
        }

        public SimpleVersion? TryParseSimpleVersion(string versionString)
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

                var versionNumberTokens = SplitString(versionString);

                var majorToken = GetValue(versionNumberTokens.tokens, "{major}");
                var minorToken = GetValue(versionNumberTokens.tokens, "{minor}");
                var patchToken = GetValue(versionNumberTokens.tokens, "{patch}");
                var buildToken = GetValue(versionNumberTokens.tokens, "{build}");
                var major = !string.IsNullOrWhiteSpace(majorToken) ? int.Parse(majorToken) : 0;
                var minor = !string.IsNullOrWhiteSpace(minorToken) ? int.Parse(minorToken) : patchToken != string.Empty || buildToken != string.Empty ? throw new InvalidOperationException("Unable to parse '{minor}'") : 0;
                var patch = !string.IsNullOrWhiteSpace(patchToken) ? int.Parse(patchToken) : 0;
                var build = !string.IsNullOrWhiteSpace(buildToken) ? int.Parse(buildToken) : 0;
                return new SimpleVersion(major, minor, patch, build);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public SemanticVersion? TryParseSemanticVersion(string fullSemVer)
        {
            if (fullSemVer == null) throw new ArgumentNullException(nameof(fullSemVer));
            if (string.IsNullOrWhiteSpace(fullSemVer)) return null;

            try
            {
                var versionNumberTokens = SplitString(fullSemVer);

                var majorToken = GetValue(versionNumberTokens.tokens, "{major}");
                var minorToken = GetValue(versionNumberTokens.tokens, "{minor}");
                var patchToken = GetValue(versionNumberTokens.tokens, "{patch}");
                var buildToken = GetValue(versionNumberTokens.tokens, "{build}");
                var major = !string.IsNullOrWhiteSpace(majorToken) ? int.Parse(majorToken) : 0;
                var minor = !string.IsNullOrWhiteSpace(minorToken) ? int.Parse(minorToken) : patchToken != string.Empty || buildToken != string.Empty ? throw new InvalidOperationException("Unable to parse '{minor}'") : 0;
                var patch = !string.IsNullOrWhiteSpace(patchToken) ? int.Parse(patchToken) : 0;
                var build = !string.IsNullOrWhiteSpace(buildToken) ? int.Parse(buildToken) : 0;

                var preReleaseTag = InvalidPreReleaseCharacters.Replace(GetValue(versionNumberTokens.tokens, "{preReleaseTag}"), "-");
                var buildMetadata = versionNumberTokens.metadata;

                return new SemanticVersion(major,
                    minor,
                    patch,
                    preReleaseTag,
                    build,
                    buildMetadata);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}