using System;
using System.ComponentModel.DataAnnotations;
using OctoVersion.BuildServers.TeamCity;

namespace OctoVersion.Tool.Configuration
{
    public class AppSettings
    {
        [Required] public string CurrentBranch { get; set; } = "main";

        [Required] public string[] NonPreReleaseTags { get; set; } = Array.Empty<string>();

        public string NonPreReleaseTagsRegex { get; set; }

        public int BuildCounter { get; set; } = 0;

        public string BuildServerOutputFormatter { get; set; } = TeamCityBuildServerOutputFormatter.BuildServerName;
    }
}