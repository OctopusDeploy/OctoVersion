using System;
using OctoVersion.Core;
using OctoVersion.Core.Configuration;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

public class InputsAndOutputNames
{
    [Fact]
    public void MustMatch()
    {
        // When exported to the environment, the expectation is that we should be able to re-import these on a subsequent
        // run without re-deriving everything. This means that our output names need to match our input names.
        nameof(AppSettings.Major).ShouldBe(nameof(OctoVersionInfo.Major));
        nameof(AppSettings.Minor).ShouldBe(nameof(OctoVersionInfo.Minor));
        nameof(AppSettings.Patch).ShouldBe(nameof(OctoVersionInfo.Patch));
        nameof(AppSettings.PreReleaseTag).ShouldBe(nameof(OctoVersionInfo.PreReleaseTag));
        nameof(AppSettings.BuildMetadata).ShouldBe(nameof(OctoVersionInfo.BuildMetadata));
    }
}