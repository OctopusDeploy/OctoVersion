using System;
using LibGit2Sharp;

namespace OctoVersion.Core.ExtensionMethods;

static class CommitExtensionMethods
{
    public static bool BumpsMajorVersion(this Commit commit)
    {
        return commit.Message.CommitMessageBumpsMajorVersion();
    }

    public static bool BumpsMinorVersion(this Commit commit)
    {
        return commit.Message.CommitMessageBumpsMinorVersion();
    }
}