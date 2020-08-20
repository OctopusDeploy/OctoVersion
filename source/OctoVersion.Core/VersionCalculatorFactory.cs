using System.Linq;
using LibGit2Sharp;
using OctoVersion.Contracts;
using Serilog;

namespace OctoVersion.Core
{
    public class VersionCalculatorFactory
    {
        private readonly ILogger _logger;
        private readonly Repository _repository;

        public VersionCalculatorFactory(string repositorySearchPath, ILogger logger)
        {
            _logger = logger;

            var gitRepositoryPath = Repository.Discover(repositorySearchPath);
            _logger.Debug("Located Git repository in {GitRepositoryPath}", gitRepositoryPath);
            _repository = new Repository(gitRepositoryPath);
        }

        public VersionCalculator Create()
        {
            var allCommits = _repository.Commits.ToArray();
            _logger.Debug("Analyzing {NumberOfCommits} commits", allCommits.Length);

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
            var allTags = _repository.Tags.ToArray();
            _logger.Debug("Analyzing {NumberOfTags} tags", allTags.Length);

            foreach (var tag in allTags)
            {
                var version = VersionInfo.TryParse(tag.FriendlyName);
                if (version == null) continue;

                // tags can reference to commits which have been removed. In this case we don't care.
                if (!commits.TryGetValue(tag.Target.Sha, out var commit)) continue;

                _logger.Verbose("{CommitHash} is tagged with {VersionTag}", commit.Hash, version);
                commit.TagWith(version);
            }

            var currentCommitHash = allCommits.First().Sha;
            var calculator = new VersionCalculator(commits.Values.ToArray(), currentCommitHash);
            return calculator;
        }
    }
}