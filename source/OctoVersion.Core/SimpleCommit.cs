﻿using System;
using System.Collections.Generic;
using LibGit2Sharp;
using OctoVersion.Contracts;

namespace OctoVersion.Core
{
    internal class SimpleCommit
    {
        public SimpleCommit(string hash, string message, DateTimeOffset timestamp, bool bumpsMajorVersion,
            bool bumpsMinorVersion)
        {
            Hash = hash;
            Message = message;
            Timestamp = timestamp;
            BumpsMajorVersion = bumpsMajorVersion;
            BumpsMinorVersion = bumpsMinorVersion;
        }

        public string Hash { get; }
        public string Message { get; }
        public DateTimeOffset Timestamp { get; }
        public bool BumpsMajorVersion { get; }
        public bool BumpsMinorVersion { get; }
        public ICollection<SimpleCommit> Parents { get; } = new HashSet<SimpleCommit>();
        public ICollection<VersionInfo> TaggedWithVersions { get; } = new HashSet<VersionInfo>();

        public void AddParent(SimpleCommit parent)
        {
            Parents.Add(parent);
        }

        public void TagWith(VersionInfo versionInfo)
        {
            TaggedWithVersions.Add(versionInfo);
        }

        public static SimpleCommit FromCommit(Commit commit)
        {
            var result = new SimpleCommit(commit.Sha, commit.MessageShort, commit.Committer.When,
                commit.BumpsMajorVersion(),
                commit.BumpsMinorVersion());
            return result;
        }

        public override string ToString()
        {
            return $"{Hash} {Message}";
        }
    }
}