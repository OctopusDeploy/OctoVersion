using System;
using System.Text.RegularExpressions;
using OctoVersion.Core.VersionTemplates;

namespace OctoVersion.Core
{
    public class OctoVersionInfo : SemanticVersion
    {
        readonly VersionParser _versionParser;

        //https://semver.org/spec/v2.0.0.html#spec-item-10
        static readonly Regex InvalidBuildMetadataCharacters = new Regex("[^0-9A-Za-z-\\.]", RegexOptions.Compiled);

        internal OctoVersionInfo(int major,
            int minor,
            int? patch,
            string preReleaseTag,
            int? build,
            string buildMetadata,
            VersionParser versionParser) : base(major,
            minor,
            patch,
            preReleaseTag,
            build,
            buildMetadata)
        {
            _versionParser = versionParser;
        }

        internal OctoVersionInfo(SemanticVersion semanticVersion,
            VersionParser versionParser) : base(
            semanticVersion.Major,
            semanticVersion.Minor,
            semanticVersion.Patch,
            semanticVersion.PreReleaseTag,
            semanticVersion.Build,
            semanticVersion.BuildMetadata)
        {
            _versionParser = versionParser;
        }

        public string PreReleaseTagWithDash => string.IsNullOrWhiteSpace(PreReleaseTag) ? string.Empty : _versionParser.GetPreReleaseTagWithDash(this);
        public string MajorMinorPatch => _versionParser.GetMajorMinorPatch(this);
        public string BuildMetadataWithPlus => string.IsNullOrWhiteSpace(BuildMetadata) ? string.Empty : $"+{InvalidBuildMetadataCharacters.Replace(BuildMetadata, "-")}";
        public string FullSemVer => $"{MajorMinorPatch}{PreReleaseTagWithDash}";
        public string InformationalVersion => $"{MajorMinorPatch}{PreReleaseTagWithDash}{BuildMetadataWithPlus}";
        string NuGetCompatiblePreReleaseWithDash => _versionParser.GetNuGetCompatiblePreReleaseWithDash(this);
        public string NuGetVersion => $"{MajorMinorPatch}{NuGetCompatiblePreReleaseWithDash}";

        public override string ToString()
        {
            return FullSemVer;
        }
    }
}