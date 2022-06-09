using System;

namespace OctoVersion.Core;

public class FullyQualifiedBranchFlattener
{
    public string Flatten(string potentiallyFullyQualifiedBranchName)
    {
        var flattenedBranchName = potentiallyFullyQualifiedBranchName
                .Replace("refs/heads/", string.Empty)
                .Replace("refs/tags/", string.Empty)
                .Replace("refs/pull/", "pull/")
            ;

        return flattenedBranchName;
    }
}