using System;
using System.Linq;
using System.Text.RegularExpressions;
using OctoVersion.Core.VersionNumberCalculation;
using Serilog;

namespace OctoVersion.Core;

public class StructuredOutputFactory
{
    readonly string _overriddenBuildMetadata;
    readonly string _currentBranch;
    readonly string _currentSha;

    readonly ILogger _logger = Log.ForContext<StructuredOutputFactory>();
    readonly FullyQualifiedBranchFlattener _branchFlattener = new();
    readonly Sanitizer _sanitizer = new();
    readonly string[] _nonPreReleaseTags;
    readonly string _nonPreReleaseTagsRegex;
    readonly int? _overriddenMajorVersion;
    readonly int? _overriddenMinorVersion;
    readonly int? _overriddenPatchVersion;

    public StructuredOutputFactory(string[] nonPreReleaseTags,
        string nonPreReleaseTagsRegex,
        int? overriddenMajorVersion,
        int? overriddenMinorVersion,
        int? overriddenPatchVersion,
        string currentBranch,
        string currentSha,
        string? overriddenBuildMetadata)
    {
        _currentBranch = currentBranch;
        _currentSha = currentSha;
        _nonPreReleaseTags = nonPreReleaseTags;
        _nonPreReleaseTagsRegex = nonPreReleaseTagsRegex;
        _overriddenMajorVersion = overriddenMajorVersion;
        _overriddenMinorVersion = overriddenMinorVersion;
        _overriddenPatchVersion = overriddenPatchVersion;
        _overriddenBuildMetadata = overriddenBuildMetadata ?? string.Empty;
    }

    public OctoVersionInfo Create(SimpleVersion version)
    {
        var preReleaseTag = DerivePreReleaseTag();

        var major = version.Major;
        if (_overriddenMajorVersion.HasValue)
        {
            _logger.Debug("Overriding derived major version {DerivedMajorVersion} with {OverriddenMajorVersion}",
                version.Major,
                _overriddenMajorVersion.Value);
            major = _overriddenMajorVersion.Value;
        }

        var minor = version.Minor;
        if (_overriddenMinorVersion.HasValue)
        {
            _logger.Debug("Overriding derived minor version {DerivedMinorVersion} with {OverriddenMinorVersion}",
                version.Minor,
                _overriddenMinorVersion.Value);
            minor = _overriddenMinorVersion.Value;
        }

        var patch = version.Patch;
        if (_overriddenPatchVersion.HasValue)
        {
            _logger.Debug("Overriding derived patch version {DerivedPatchVersion} with {OverriddenPatchVersion}",
                version.Patch,
                _overriddenPatchVersion.Value);
            patch = _overriddenPatchVersion.Value;
        }

        var buildMetadata = DeriveBuildMetadata();
        if (!string.IsNullOrEmpty(_overriddenBuildMetadata))
        {
            var sanitizedBuildMetadata = _sanitizer.Sanitize(_overriddenBuildMetadata);
            _logger.Debug("Overriding derived build metadata {DerivedBuildMetadata} with {OverriddenBuildMetadata}",
                buildMetadata,
                sanitizedBuildMetadata);
            buildMetadata = sanitizedBuildMetadata;
        }

        var result = new OctoVersionInfo(major,
            minor,
            patch,
            preReleaseTag,
            buildMetadata);
        return result;
    }

    string DerivePreReleaseTag()
    {
        if (_nonPreReleaseTags.Any(t => t.Equals(_currentBranch, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.Debug(
                "{CurrentBranch} is contained within the set of non-pre-release branches {@NonPreReleaseBranches}. No pre-release tag is being added.",
                _currentBranch,
                _nonPreReleaseTags);
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
                    _currentBranch,
                    _nonPreReleaseTagsRegex);
                return string.Empty;
            }
        }

        var flattenedBranchName = _branchFlattener.Flatten(_currentBranch);
        var preReleaseTag = _sanitizer.Sanitize(flattenedBranchName);

        _logger.Debug("Using pre-release tag {PreReleaseTag}", preReleaseTag);
        return preReleaseTag;
    }

    string DeriveBuildMetadata()
    {
        var flattenedBranchName = _branchFlattener.Flatten(_currentBranch);
        var sanitizedBranchName = _sanitizer.Sanitize(flattenedBranchName);
        var buildMetadata = $"Branch.{sanitizedBranchName}.Sha.{_currentSha}";

        _logger.Debug("Using build metadata {BuildMetadata}", buildMetadata);
        return buildMetadata;
    }
}