using System;
using OctoVersion.Core;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests
{
    public class WhenStringifyingAnOctoVersionInfo
    {
        [Theory]
        [InlineData(0,
            0,
            0,
            "",
            "",
            "")]
        [InlineData(1,
            2,
            3,
            "",
            "",
            "")]
        [InlineData(1,
            2,
            3,
            "pre",
            "",
            "-pre")]
        [InlineData(1,
            2,
            3,
            "pre",
            "build",
            "-pre")]
        [InlineData(2021,
            1,
            3,
            "mark-genericDocumentStore",
            "Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550",
            "-mark-genericDocumentStore")]
        public void ThePreReleaseTagWithDashShouldBeCorrect(int major,
            int minor,
            int patch,
            string preReleaseTag,
            string buildMetadata,
            string expected)
        {
            new OctoVersionInfo(major,
                minor,
                patch,
                preReleaseTag,
                buildMetadata).PreReleaseTagWithDash.ShouldBe(expected);
        }

        [Theory]
        [InlineData(0,
            0,
            0,
            "",
            "",
            "0.0.0")]
        [InlineData(1,
            2,
            3,
            "",
            "",
            "1.2.3")]
        [InlineData(1,
            2,
            3,
            "pre",
            "",
            "1.2.3")]
        [InlineData(1,
            2,
            3,
            "pre",
            "build",
            "1.2.3")]
        [InlineData(2021,
            1,
            3,
            "mark-genericDocumentStore",
            "Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550",
            "2021.1.3")]
        public void TheMajorMinorPatchShouldBeCorrect(int major,
            int minor,
            int patch,
            string preReleaseTag,
            string buildMetadata,
            string expected)
        {
            new OctoVersionInfo(major,
                minor,
                patch,
                preReleaseTag,
                buildMetadata).MajorMinorPatch.ShouldBe(expected);
        }

        [Theory]
        [InlineData(0,
            0,
            0,
            "",
            "",
            "")]
        [InlineData(1,
            2,
            3,
            "",
            "",
            "")]
        [InlineData(1,
            2,
            3,
            "pre",
            "",
            "")]
        [InlineData(1,
            2,
            3,
            "pre",
            "build",
            "+build")]
        [InlineData(2021,
            1,
            3,
            "mark-genericDocumentStore",
            "Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550",
            "+Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550")]
        public void TheBuildMetadataWithPlusShouldBeCorrect(int major,
            int minor,
            int patch,
            string preReleaseTag,
            string buildMetadata,
            string expected)
        {
            new OctoVersionInfo(major,
                minor,
                patch,
                preReleaseTag,
                buildMetadata).BuildMetadataWithPlus.ShouldBe(expected);
        }

        [Theory]
        [InlineData(0,
            0,
            0,
            "",
            "",
            "0.0.0")]
        [InlineData(1,
            2,
            3,
            "",
            "",
            "1.2.3")]
        [InlineData(1,
            2,
            3,
            "pre",
            "",
            "1.2.3-pre")]
        [InlineData(1,
            2,
            3,
            "pre",
            "build",
            "1.2.3-pre")]
        [InlineData(2021,
            1,
            3,
            "mark-genericDocumentStore",
            "Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550",
            "2021.1.3-mark-genericDocumentStore")]
        public void TheFullSemVerShouldBeCorrect(int major,
            int minor,
            int patch,
            string preReleaseTag,
            string buildMetadata,
            string expected)
        {
            new OctoVersionInfo(major,
                minor,
                patch,
                preReleaseTag,
                buildMetadata).FullSemVer.ShouldBe(expected);
        }

        [Theory]
        [InlineData(0,
            0,
            0,
            "",
            "",
            "0.0.0")]
        [InlineData(1,
            2,
            3,
            "",
            "",
            "1.2.3")]
        [InlineData(1,
            2,
            3,
            "pre",
            "",
            "1.2.3-pre")]
        [InlineData(1,
            2,
            3,
            "pre",
            "build",
            "1.2.3-pre")]
        [InlineData(2021,
            1,
            3,
            "mark-genericDocumentStore",
            "Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550",
            "2021.1.3-mark-genericDocumen")]
        [InlineData(2021,
            1,
            3,
            "dependabot/npm_and_yarn/source/TentacleArmy.Web/ini-1.3.6",
            "Branch.dependabot/npm_and_yarn/source/TentacleArmy.Web/ini-1.3.6.Sha.069392d9d2d37ddb6009998b92e70963badcc666",
            "2021.1.3-dependabot/npm-and-")]
        public void TheNuGetVersionShouldBeCorrect(int major,
            int minor,
            int patch,
            string preReleaseTag,
            string buildMetadata,
            string expected)
        {
            new OctoVersionInfo(major,
                minor,
                patch,
                preReleaseTag,
                buildMetadata).NuGetVersion.ShouldBe(expected);
        }

        [Theory]
        [InlineData(0,
            0,
            0,
            "",
            "",
            "0.0.0")]
        [InlineData(1,
            2,
            3,
            "",
            "",
            "1.2.3")]
        [InlineData(1,
            2,
            3,
            "pre",
            "",
            "1.2.3-pre")]
        [InlineData(1,
            2,
            3,
            "pre",
            "build",
            "1.2.3-pre+build")]
        [InlineData(2021,
            1,
            3,
            "mark-genericDocumentStore",
            "Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550",
            "2021.1.3-mark-genericDocumentStore+Branch.mark-genericDocumentStore.Sha.fb13016f3a21d7c2058fb74ab25f19e5311c6550")]
        public void TheInformationalVersionShouldBeCorrect(int major,
            int minor,
            int patch,
            string preReleaseTag,
            string buildMetadata,
            string expected)
        {
            new OctoVersionInfo(major,
                minor,
                patch,
                preReleaseTag,
                buildMetadata).InformationalVersion.ShouldBe(expected);
        }
    }
}