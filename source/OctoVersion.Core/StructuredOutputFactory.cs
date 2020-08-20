using System;
using System.Linq;
using System.Text.RegularExpressions;
using Serilog;

namespace OctoVersion.Core
{
    public class StructuredOutputFactory
    {
        private readonly string _buildMetadata;
        private readonly string _currentBranch;

        private readonly ILogger _logger = Log.ForContext<StructuredOutputFactory>();
        private readonly string[] _nonPreReleaseTags;
        private readonly string _nonPreReleaseTagsRegex;

        public StructuredOutputFactory(string currentBranch, string[] nonPreReleaseTags, string nonPreReleaseTagsRegex,
            string buildMetadata)
        {
            _currentBranch = currentBranch;
            _nonPreReleaseTags = nonPreReleaseTags;
            _nonPreReleaseTagsRegex = nonPreReleaseTagsRegex;
            _buildMetadata = buildMetadata;
        }

        public StructuredOutput Create(VersionInfo versionInfo)
        {
            var preReleaseTag = DerivePreReleaseTag();
            var result = new StructuredOutput(versionInfo.Major, versionInfo.Minor, versionInfo.Revision, preReleaseTag,
                _buildMetadata);
            return result;
        }

        private string DerivePreReleaseTag()
        {
            if (_nonPreReleaseTags.Any(t => t.Equals(_currentBranch, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.Debug(
                    "{CurrentBranch} is contained within the set of non-pre-release branches {@NonPreReleaseBranches}. No pre-release tag is being added.",
                    _currentBranch, _nonPreReleaseTags);
                return string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(_nonPreReleaseTagsRegex))
            {
                _logger.Debug("A non-pre-release regular expression has been provided. Checking against that.");
                var regex = new Regex(_nonPreReleaseTagsRegex, RegexOptions.Compiled);
                if (regex.IsMatch(_currentBranch))
                {
                    _logger.Debug(
                        "{CurrentBranch} matches the non-pre-release regular expression {NonPreReleaseTagsRegularExpression}. No pre-release tag is being added.",
                        _currentBranch, _nonPreReleaseTagsRegex);
                    return string.Empty;
                }
            }

            _logger.Debug("Using pre-release tag {PreReleaseTag}", _currentBranch);
            return _currentBranch;
        }
    }
}