using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace OctoVersion.Core.VersionNumberCalculation;

public class VersionCalculator
{
    readonly ILogger _logger = Log.ForContext<VersionCalculator>();
    readonly Dictionary<SimpleCommit, SimpleVersion> _calculatedVersions = new();

    readonly SimpleCommit[] _commits;
    bool _cacheIsPrimed;

    internal VersionCalculator(SimpleCommit[] commits, string currentCommitHash)
    {
        _commits = commits;
        CurrentCommitHash = currentCommitHash;
    }

    public string CurrentCommitHash { get; }

    void EnsureCacheIsPrimed()
    {
        if (_cacheIsPrimed) return;

        // Traverse up the commit history in roughly chronological order (oldest to newest)
        // so that we limit the recursion depth. It's marginally less efficient this way but
        // it avoids stack overflows from recursing down 50,000 stack frames :)
        foreach (var commit in _commits.OrderBy(c => c.Timestamp)) GetVersionInternal(commit);

        _cacheIsPrimed = true;
    }

    public SimpleVersion GetVersion()
    {
        var commit = _commits.Single(c => c.Hash == CurrentCommitHash);
        return GetVersion(commit);
    }

    SimpleVersion GetVersion(SimpleCommit commit)
    {
        EnsureCacheIsPrimed();

        return GetVersionInternal(commit);
    }

    SimpleVersion GetVersionInternal(SimpleCommit commit)
    {
        // We do this to avoid recursing too many stack frames
        if (_calculatedVersions.TryGetValue(commit, out var alreadyCalculatedVersion))
            return alreadyCalculatedVersion;

        var taggedVersion = commit
            .TaggedWithVersions
            .OrderByDescending(v => v)
            .FirstOrDefault();

        if (taggedVersion != null)
        {
            _logger.Debug("Setting version for {Commit} using tag {Tag}", commit, taggedVersion);
            return taggedVersion;
        }

        var maxParentVersion = commit.Parents
                .Select(GetVersionInternal)
                .OrderByDescending(v => v)
                .FirstOrDefault()
            ?? new SimpleVersion(0, 0, 0);

        SimpleVersion version;
        if (commit.BumpsMajorVersion)
        {
            version = new SimpleVersion(maxParentVersion.Major + 1, 0, 0);
            _logger.Debug("Setting version for {Commit} to bump major version from {MaxParentVersion} to {NewVersion}", commit, maxParentVersion, version);
        }
        else if (commit.BumpsMinorVersion)
        {
            version = new SimpleVersion(maxParentVersion.Major, maxParentVersion.Minor + 1, 0);
            _logger.Debug("Setting version for {Commit} to bump minor version from {MaxParentVersion} to {NewVersion}", commit,  maxParentVersion, version);
        }
        else
        {
            version = new SimpleVersion(maxParentVersion.Major,
                maxParentVersion.Minor,
                maxParentVersion.Patch + 1);
            _logger.Debug("Setting version for {Commit} to increment patch version from {MaxParentVersion} to {NewVersion}", commit, maxParentVersion, version);
        }
        
        _calculatedVersions[commit] = version;

        return version;
    }
}
