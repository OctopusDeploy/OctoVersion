using System;

namespace OctoVersion.Core
{
    public class OctoVersionInfo : SemanticVersion
    {
        public OctoVersionInfo(
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

        public OctoVersionInfo(SemanticVersion semanticVersion) : base(
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
        public string FullSemVer => $"{MajorMinorPatch}{PreReleaseTagWithDash}";
        public string InformationalVersion => $"{MajorMinorPatch}{PreReleaseTagWithDash}{BuildMetadataWithPlus}";
        string NuGetCompatiblePreReleaseWithDash => PreReleaseTagWithDash.Substring(0, Math.Min(PreReleaseTagWithDash.Length, 20)).Replace("_", "-");
        public string NuGetVersion => $"{MajorMinorPatch}{NuGetCompatiblePreReleaseWithDash}";

        public override string ToString()
        {
            return FullSemVer;
        }
    }
}