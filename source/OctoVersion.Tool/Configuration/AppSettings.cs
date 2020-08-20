using System.ComponentModel.DataAnnotations;
using OctoVersion.BuildServers.TeamCity;

namespace OctoVersion.Tool.Configuration
{
    public class AppSettings
    {
        [Required] public string CurrentBranch { get; set; } = "main";

        [Required] public string[] NonPreReleaseTags { get; set; } = {"main", "master"};

        public string NonPreReleaseTagsRegex { get; set; } = "^release/.*$";

        public int? OverrideMajorVersion { get; set; }
        public int? OverrideMinorVersion { get; set; }
        public int? OverrideRevision { get; set; }

        public string BuildMetadata { get; set; } = string.Empty;

        public string BuildServerOutputFormatter { get; set; } = TeamCityBuildServerOutputFormatter.BuildServerName;
    }
}