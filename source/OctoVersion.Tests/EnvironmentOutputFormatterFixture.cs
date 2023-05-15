using System.Linq;
using OctoVersion.Core.OutputFormatting.Environment;
using OctoVersion.Core.VersionNumberCalculation;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class EnvironmentOutputFormatterFixture
{
    /// <remarks>
    /// Relates to https://github.com/OctopusDeploy/OctoVersion/issues/98.
    /// </remarks>
    [Fact]
    public void ReadsPropertiesFromBothOctoVersionInfoAndBaseClass()
    {
        var sampleData = new SampleDataBuilder()
            .WithNonPreReleaseTags(new[] { "refs/heads/main" })
            .WithNonPreReleaseTagsRegex(string.Empty)
            .WithCurrentBranch("refs/heads/main")
            .WithCurrentSha("a1b2c3d4e5")
            .WithVersion(new SimpleVersion(1, 2, 3))
            .Build();
        
        var octoVersionInfo = sampleData.ToOctoVersion();
        
        var propertiesToWrite = EnvironmentOutputFormatter
            .GetPropertiesToWrite(octoVersionInfo)
            .OrderBy(x => x)
            .ToArray();
        
        propertiesToWrite.ShouldBeEquivalentTo(new [] {
            "OCTOVERSION_BuildMetadata=Branch.main.Sha.a1b2c3d4e5",
            "OCTOVERSION_BuildMetadataWithPlus=+Branch.main.Sha.a1b2c3d4e5",
            "OCTOVERSION_FullSemVer=1.2.3",
            "OCTOVERSION_InformationalVersion=1.2.3+Branch.main.Sha.a1b2c3d4e5",
            "OCTOVERSION_Major=1",
            "OCTOVERSION_MajorMinorPatch=1.2.3",
            "OCTOVERSION_Minor=2",
            "OCTOVERSION_NuGetVersion=1.2.3",
            "OCTOVERSION_Patch=3",
            "OCTOVERSION_PreReleaseTag=",
            "OCTOVERSION_PreReleaseTagWithDash=",
        });
    }
}
