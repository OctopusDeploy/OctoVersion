using OctoVersion.Core;
using OctoVersion.Tool.Configuration;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests
{
    public class InputsAndOutputNames
    {
        [Fact]
        public void MustMatch()
        {
            // When exported to the environment, the expectation is that we should be able to re-import these on a subsequent
            // run without re-deriving everything. This means that our output names need to match our input names.
            nameof(AppSettings.Major).ShouldBe(nameof(StructuredOutput.Major));
            nameof(AppSettings.Minor).ShouldBe(nameof(StructuredOutput.Minor));
            nameof(AppSettings.Patch).ShouldBe(nameof(StructuredOutput.Patch));
            nameof(AppSettings.PreReleaseTag).ShouldBe(nameof(StructuredOutput.PreReleaseTag));
            nameof(AppSettings.BuildMetadata).ShouldBe(nameof(StructuredOutput.BuildMetadata));
        }
    }
}