using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OctoVersion.Core.Configuration;

public interface IAppSettings
{
    void ApplyDefaultsIfRequired();
    void ContributeSaneArrayArgs(string[] args);
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

    public bool DetectEnvironment { get; set; }

    public string OutputJsonFile { get; set; }

    public void ApplyDefaultsIfRequired()
    {
        if (!NonPreReleaseTags.Any())
            NonPreReleaseTags = new[] { "main", "master" };
    }

    /// <param name="strings"></param>
    /// <remarks>
    /// dotnet core doesn't allow multiple args of the same name
    /// for array types, it expects you to pass them like "--OutputFormats:0 Json"
    /// which is just... undiscoverable
    /// So, let's manually parse it for now
    /// Maybe one day in the future, change over to use Octopus.Commandline
    /// </remarks>
    public void ContributeSaneArrayArgs(string[] args)
    {
        var cliNonPreReleaseTags = ParseArgsForArrayArguments(args, nameof(NonPreReleaseTags));
        if (cliNonPreReleaseTags.Any())
            NonPreReleaseTags = cliNonPreReleaseTags;
        var cliOutputFormats = ParseArgsForArrayArguments(args, nameof(OutputFormats));
        if (cliOutputFormats.Any())
            OutputFormats = cliOutputFormats;
    }

    static string[] ParseArgsForArrayArguments(string[] args, string name)
    {
        var result = new List<string>();
        var addArg = false;
        foreach (var arg in args)
        {
            if (addArg)
            {
                result.Add(arg);
                addArg = false;
            }

            if (arg.StartsWith("--" + name.TrimEnd('s'), StringComparison.OrdinalIgnoreCase))
                addArg = true;
        }

        return result.ToArray();
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(CurrentBranch) && string.IsNullOrWhiteSpace(FullSemVer))
            yield return new ValidationResult($"At least one of {nameof(CurrentBranch)} or {nameof(FullSemVer)} must be provided.", new[] { nameof(CurrentBranch), nameof(FullSemVer) });
    }
}