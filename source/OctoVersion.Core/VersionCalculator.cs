using System.Collections.Generic;
using System.Linq;
using OctoVersion.Contracts;

namespace OctoVersion.Core
{
    public class VersionCalculator
    {
        private readonly Dictionary<SimpleCommit, VersionInfo> _calculatedVersions =
            new Dictionary<SimpleCommit, VersionInfo>();

        private readonly SimpleCommit[] _commits;
        private bool _cacheIsPrimed;

        internal VersionCalculator(SimpleCommit[] commits, string currentCommitHash)
        {
            _commits = commits;
            CurrentCommitHash = currentCommitHash;
        }

        public string CurrentCommitHash { get; }

        private void EnsureCacheIsPrimed()
        {
            if (_cacheIsPrimed) return;

            // Traverse up the commit history in roughly chronological order (oldest to newest)
            // so that we limit the recursion depth. It's marginally less efficient this way but
            // it avoids stack overflows from recursing down 50,000 stack frames :)
            foreach (var commit in _commits.OrderBy(c => c.Timestamp)) GetVersionInternal(commit);

            _cacheIsPrimed = true;
        }

        public VersionInfo GetVersion()
        {
            EnsureCacheIsPrimed();

            return GetVersion(CurrentCommitHash);
        }

        public VersionInfo GetVersion(string commitHash)
        {
            EnsureCacheIsPrimed();

            var commit = _commits.Single(c => c.Hash == commitHash);
            return GetVersion(commit);
        }

        internal VersionInfo GetVersion(SimpleCommit commit)
        {
            EnsureCacheIsPrimed();

            return GetVersionInternal(commit);
        }

        private VersionInfo GetVersionInternal(SimpleCommit commit)
        {
            // We do this to avoid recursing too many stack frames
            if (_calculatedVersions.TryGetValue(commit, out var alreadyCalculatedVersion))
                return alreadyCalculatedVersion;

            var taggedVersion = commit
                .TaggedWithVersions
                .OrderByDescending(v => v)
                .FirstOrDefault();

            if (taggedVersion != null) return taggedVersion;

            var maxParentVersion = commit.Parents
                                       .SelectMany(c => c.Parents)
                                       .Select(GetVersionInternal)
                                       .OrderByDescending(v => v)
                                       .FirstOrDefault()
                                   ?? new VersionInfo(0, 0, 0);

            VersionInfo versionInfo;
            if (commit.BumpsMajorVersion)
                versionInfo = new VersionInfo(maxParentVersion.Major + 1, 0, 0);
            else if (commit.BumpsMinorVersion)
                versionInfo = new VersionInfo(maxParentVersion.Major, maxParentVersion.Minor + 1, 0);
            else
                versionInfo = new VersionInfo(maxParentVersion.Major, maxParentVersion.Minor,
                    maxParentVersion.Revision + 1);
            _calculatedVersions[commit] = versionInfo;

            return versionInfo;
        }
    }
}