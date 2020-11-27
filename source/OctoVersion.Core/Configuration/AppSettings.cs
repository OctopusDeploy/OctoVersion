using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OctoVersion.Core.Configuration
{
    public interface IAppSettings
    {
        void ApplyDefaultsIfRequired();
    }

    public class AppSettings : IAppSettings, IValidatableObject
    {
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

        // If this is set, it will override all of the other values and OctoVersion will just adopt it wholesale.
        public string FullSemVer { get; set; }

        public string[] OutputFormats { get; set; } = Array.Empty<string>();

        public void ApplyDefaultsIfRequired()
        {
            if (!NonPreReleaseTags.Any()) NonPreReleaseTags = new[] { "main", "master" };
            if (!OutputFormats.Any()) OutputFormats = new[] { "Console" };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(CurrentBranch) && string.IsNullOrWhiteSpace(FullSemVer))
                yield return new ValidationResult($"At least one of {nameof(CurrentBranch)} or {nameof(FullSemVer)} must be provided.", new[] { nameof(CurrentBranch), nameof(FullSemVer) });
        }
    }
}