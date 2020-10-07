using System;

namespace OctoVersion.Core
{
    public class StructuredOutput : SemanticVersion
    {
        public StructuredOutput(
            int major,
            int minor,
            int patch,
            string preReleaseTag,
            string buildMetadata) : base(major,
            minor,
            patch,
            preReleaseTag,
            buildMetadata)
        {
        }

        public StructuredOutput(SemanticVersion semanticVersion) : base(
            semanticVersion.Major,
            semanticVersion.Minor,
            semanticVersion.Patch,
            semanticVersion.PreReleaseTag,
            semanticVersion.BuildMetadata)
        {
        }

        public string PreReleaseTagWithDash => string.IsNullOrWhiteSpace(PreReleaseTag) ? string.Empty : $"-{PreReleaseTag}";
        public string MajorMinorPatch => $"{Major}.{Minor}.{Patch}";
        public string BuildMetadataWithPlus => string.IsNullOrWhiteSpace(BuildMetadata) ? string.Empty : $"+{BuildMetadata}";
        public string FullSemVer => $"{MajorMinorPatch}{PreReleaseTagWithDash}{BuildMetadataWithPlus}";

        public override string ToString()
        {
            return FullSemVer;
        }
    }
}