using System;
using System.Collections.Generic;
using System.Linq;
using LibGit2Sharp;
using OctoVersion.Core.Exceptions;
using Serilog;
using RepositoryNotFoundException = OctoVersion.Core.Exceptions.RepositoryNotFoundException;

namespace OctoVersion.Core.VersionNumberCalculation;

public class VersionCalculatorFactory
{
    readonly ILogger _logger = Log.ForContext<VersionCalculatorFactory>();
    readonly Repository _repository;
    readonly bool _allowShallowClone;

    public VersionCalculatorFactory(string repositorySearchPath, bool allowShallowClone = false)
    {
        var gitRepositoryPath = Repository.Discover(repositorySearchPath);
        if (gitRepositoryPath == null)
            throw new RepositoryNotFoundException("Unable to resolve Git repository path.");
        _logger.Debug("Located Git repository in {GitRepositoryPath}", gitRepositoryPath);
        _repository = new Repository(gitRepositoryPath);
        _allowShallowClone = allowShallowClone;
    }

    public VersionCalculator Create()
    {
        if (_repository.Info.IsShallow)
        {
            if (_allowShallowClone)
                _logger.Warning("This repository is a shallow clone. Version calculation may be inaccurate if the commit history does not reach a commit containing version information.");
            else
                throw new RepositoryIsShallowCloneException("This repository is a shallow clone; it does not contain enough history to resolve the version correctly.");
        }

        Commit[] allCommits;
        using (_logger.BeginTimedOperation("Loading commits"))
        {
            allCommits = _repository.Commits.ToArray();
            _logger.Debug("Repository contains {NumberOfCommits} commits", allCommits.Length);
        }

        Dictionary<string, SimpleCommit> commits;
        using (_logger.BeginTimedOperation("Mapping commits into internal representation"))
        {
            commits = allCommits
                .Select(SimpleCommit.FromCommit)
                .ToDictionary(c => c.Hash, c => c);
        }

        // Establish parent/child relationships
        using (_logger.BeginTimedOperation("Establishing parent/child relationships"))
        {
            foreach (var commit in allCommits)
            {
                if (!commit.Parents.Any()) continue;

                var simpleCommit = commits[commit.Sha];
                foreach (var parent in commit.Parents)
                {
                    if (!commits.TryGetValue(parent.Sha, out var simpleParent))
                    {
                        if (_allowShallowClone)
                        {
                            // In a shallow clone, boundary commits may reference parents outside the available history. Skip them rather than blowing up with a KeyNotFoundException.
                            continue;
                        }
                        else
                        {
                            throw new KeyNotFoundException("Unable to find parent commit with hash " + parent.Sha);
                        }
                    }
                    
                    simpleCommit.AddParent(simpleParent);
                }
            }
        }

        Tag[] allTags;
        using (_logger.BeginTimedOperation("Loading tags"))
        {
            allTags = _repository.Tags.ToArray();
            _logger.Debug("Repository contains {NumberOfTags} tags", allTags.Length);
        }

        using (_logger.BeginTimedOperation("Applying relevant version tags to each commit"))
        {
            foreach (var tag in allTags)
            {
                var version = SimpleVersion.TryParse(tag.FriendlyName);
                if (version == null) continue;

                // tags can reference to commits which have been removed. In this case we don't care.
                if (!commits.TryGetValue(tag.Target.Sha, out var commit)) continue;

                _logger.Verbose("{CommitHash} is tagged with {VersionTag}", commit.Hash, version);
                commit.TagWith(version);
            }
        }

        var currentCommitHash = allCommits.First().Sha;
        var calculator = new VersionCalculator(commits.Values.ToArray(), currentCommitHash);
        return calculator;
    }
}