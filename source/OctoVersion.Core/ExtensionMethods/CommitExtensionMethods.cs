using System;
using LibGit2Sharp;

namespace OctoVersion.Core.ExtensionMethods;

static class CommitExtensionMethods
{
    public static bool BumpsMajorVersion(this Commit commit)
    {
        if (commit.Message == null) return false;

        if (commit.Message.Contains("+semver: breaking")) return true;
        if (commit.Message.Contains("+semver: major")) return true;
        return false;
    }

    public static bool BumpsMinorVersion(this Commit commit)
    {
        if (commit.Message == null) return false;

        if (commit.Message.Contains("+semver: feature")) return true;
        if (commit.Message.Contains("+semver: minor")) return true;
        return false;
    }
}