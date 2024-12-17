using System;
using LibGit2Sharp;

namespace OctoVersion.Core.ExtensionMethods;

static class CommitExtensionMethods
{
    public static bool BumpsMajorVersion(this Commit commit)
        => commit.Message.CommitMessageBumpsMajorVersion();

    public static bool BumpsMinorVersion(this Commit commit)
        => commit.Message.CommitMessageBumpsMinorVersion();
}
