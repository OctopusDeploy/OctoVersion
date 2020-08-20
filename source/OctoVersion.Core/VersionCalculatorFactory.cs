using System.Linq;
using LibGit2Sharp;

namespace OctoVersion.Core
{
    public class VersionCalculatorFactory
    {
        private readonly Repository _repository;

        public VersionCalculatorFactory(string repositoryPath)
        {
            var actualRepositoryPath = Repository.Discover(repositoryPath);
            _repository = new Repository(actualRepositoryPath);
        }

        public VersionCalculator Create()
        {
            var allCommits = _repository.Commits.ToArray();

            // Map all the commits into a useful collection
            var commits = allCommits
                .Select(SimpleCommit.FromCommit)
                .ToDictionary(c => c.Hash, c => c);

            // Establish parent/child relationships
            foreach (var commit in allCommits)
            {
                if (!commit.Parents.Any()) continue;

                var simpleCommit = commits[commit.Sha];
                foreach (var parent in commit.Parents)
                {
                    var simpleParent = commits[parent.Sha];
                    simpleCommit.AddParent(simpleParent);
                }
            }

            // Apply all relevant version tags to each commit
            foreach (var tag in _repository.Tags)
            {
                var version = VersionInfo.TryParse(tag.FriendlyName);
                if (version == null) continue;

                // tags can reference to commits which have been removed. In this case we don't care.
                if (!commits.TryGetValue(tag.Target.Sha, out var commit)) continue;

                commit.TagWith(version);
            }

            var currentCommitHash = allCommits.First().Sha;
            var calculator = new VersionCalculator(commits.Values.ToArray(), currentCommitHash);
            return calculator;
        }
    }
}