using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OctoVersion.Tool.Configuration
{
    public interface IAppSettings
    {
        void ApplyDefaultsIfRequired();
    }

    public class AppSettings : IAppSettings
    {
        [Required]
        public string CurrentBranch { get; set; }

        [Required]
        public string[] NonPreReleaseTags { get; set; } = Array.Empty<string>();

        public string NonPreReleaseTagsRegex { get; set; } = string.Empty;

        public string RepositoryPath { get; set; } = string.Empty;

        public int? Major { get; set; }
        public int? Minor { get; set; }
        public int? Patch { get; set; }
        public string PreReleaseTag { get; set; }
        public string BuildMetadata { get; set; }

        public string[] OutputFormats { get; set; } = Array.Empty<string>();

        public void ApplyDefaultsIfRequired()
        {
            if (!NonPreReleaseTags.Any()) NonPreReleaseTags = new[] { "main", "master" };
            if (!OutputFormats.Any()) OutputFormats = new[] { "Console" };
        }
    }
}