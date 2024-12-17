using System;

namespace OctoVersion.Core.ExtensionMethods
{
    static class StringExtensionMethods
    {
        public static bool CommitMessageBumpsMajorVersion(this string? commitMessage)
        {
            if (commitMessage == null) return false;

            var normalizedCommitMessage = commitMessage.Replace(" ", "");
            if (normalizedCommitMessage.Contains("+semver:breaking", StringComparison.OrdinalIgnoreCase)) return true;
            if (normalizedCommitMessage.Contains("+semver:major", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
        public static bool CommitMessageBumpsMinorVersion(this string? commitMessage)
        {
            if (commitMessage == null) return false;
            var normalizedCommitMessage = commitMessage.Replace(" ", "");
            if (normalizedCommitMessage.Contains("+semver:feature", StringComparison.OrdinalIgnoreCase)) return true;
            if (normalizedCommitMessage.Contains("+semver:minor", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}
