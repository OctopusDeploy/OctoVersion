using System;

namespace OctoVersion.Core
{
    public class StructuredOutput
    {
        public StructuredOutput(int major,
            int minor,
            int patch,
            string preReleaseTag,
            string buildMetadata)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            PreReleaseTag = preReleaseTag;
            BuildMetadata = buildMetadata;
        }

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }
        public string MajorMinorPatch => $"{Major}.{Minor}.{Patch}";
        public string PreReleaseTag { get; }

        public string PreReleaseTagWithDash =>
            string.IsNullOrWhiteSpace(PreReleaseTag) ? string.Empty : $"-{PreReleaseTag}";

        public string BuildMetadata { get; }

        public string BuildMetadataWithPlus =>
            string.IsNullOrWhiteSpace(BuildMetadata) ? string.Empty : $"+{BuildMetadata}";

        public string FullSemVer => $"{MajorMinorPatch}{PreReleaseTagWithDash}{BuildMetadataWithPlus}";

        public override string ToString()
        {
            return FullSemVer;
        }
    }
}