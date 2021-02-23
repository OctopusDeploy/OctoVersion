using System.Collections.Generic;
using System.Linq;
using OctoVersion.Core;
using OctoVersion.Core.VersionNumberCalculation;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests
{
    public class StructuredOutputFactoryFixture
    {
        [Theory]
        [MemberData(nameof(XUnitFormattedTestCases))]
        public void CalculatesOctoVersionCorrectly(SampleData sampleData)
        {
            var factory = new StructuredOutputFactory(sampleData.NonPreReleaseTags,
                sampleData.NonPreReleaseTagsRegex,
                sampleData.OverriddenMajorVersion,
                sampleData.OverriddenMinorVersion,
                sampleData.OverriddenPatchVersion,
                sampleData.CurrentBranch,
                sampleData.CurrentSha,
                sampleData.OverriddenBuildMetadata);
            var result = factory.Create(sampleData.Version);
            result.ToString().ShouldBe(sampleData.Expected);
        }

        //map from a sane type into what xunit expects
        public static IEnumerable<object[]> XUnitFormattedTestCases() => TestCases().Select(sampleData => new object[] { sampleData });

        static IEnumerable<SampleData> TestCases()
        {
            yield return DefaultScenario
                .ExpectResult("1.2.3+Branch.main.Sha.a1b2c3d4e5");
            yield return DefaultScenario
                .WithOverriddenMajorVersion(9)
                .ExpectResult("9.2.3+Branch.main.Sha.a1b2c3d4e5");
            yield return DefaultScenario
                .WithOverriddenMinorVersion(9)
                .ExpectResult("1.9.3+Branch.main.Sha.a1b2c3d4e5");
            yield return DefaultScenario
                .WithOverriddenPatchVersion(9)
                .ExpectResult("1.2.9+Branch.main.Sha.a1b2c3d4e5");
            yield return DefaultScenario
                .WithOverriddenBuildMetadata("custom build meta/data")
                .ExpectResult("1.2.3+custom-build-meta-data");
            yield return DefaultScenario
                .WithCurrentSha("aaabbbccc")
                .ExpectResult("1.2.3+Branch.main.Sha.aaabbbccc");
            yield return DefaultScenario
                .WithVersion(new SimpleVersion(9, 8, 7))
                .ExpectResult("9.8.7+Branch.main.Sha.a1b2c3d4e5");
            yield return DefaultScenario
                .WithNonPreReleaseTags(new[] { "refs/heads/trunk" })
                .ExpectResult("1.2.3-main+Branch.main.Sha.a1b2c3d4e5");
            yield return DefaultScenario
                .WithNonPreReleaseTags(new string[0])
                .WithNonPreReleaseTagsRegex("refs/heads/m.*")
                .ExpectResult("1.2.3+Branch.main.Sha.a1b2c3d4e5");
            yield return DefaultScenario
                .WithNonPreReleaseTags(new string[0])
                .WithNonPreReleaseTagsRegex("refs/heads/m.*")
                .WithCurrentBranch("refs/heads/feature/versioning")
                .ExpectResult("1.2.3-feature-versioning+Branch.feature-versioning.Sha.a1b2c3d4e5");
        }

        static SampleData DefaultScenario => new SampleData();
    }

    public class SampleData
    {
        public SampleData() {
            NonPreReleaseTags = new[] { "refs/heads/main" };
            NonPreReleaseTagsRegex = string.Empty;
            OverriddenMajorVersion = null;
            OverriddenMinorVersion = null;
            OverriddenPatchVersion = null;
            CurrentBranch = "refs/heads/main";
            CurrentSha = "a1b2c3d4e5";
            OverriddenBuildMetadata = null;
            Version = new SimpleVersion(1, 2, 3);
            Expected = "1.2.3+Branch.main.Sha.a1b2c3d4e5";
        }

        public string[] NonPreReleaseTags { get; private set; }
        public string NonPreReleaseTagsRegex { get; private set; }
        public int? OverriddenMajorVersion { get; private set; }
        public int? OverriddenMinorVersion { get; private set; }
        public int? OverriddenPatchVersion { get; private set; }
        public string CurrentBranch { get; private set; }
        public string CurrentSha { get; private set; }
        public string OverriddenBuildMetadata { get; private set; }
        public string Expected { get; private set; }
        public SimpleVersion Version { get; private set; }

        public SampleData WithNonPreReleaseTags(string[] nonPreReleaseTags) { NonPreReleaseTags = nonPreReleaseTags; return this; }
        public SampleData WithNonPreReleaseTagsRegex(string nonPreReleaseTagsRegex) { this.NonPreReleaseTagsRegex = nonPreReleaseTagsRegex; return this; }
        public SampleData WithOverriddenMajorVersion(int? overriddenMajorVersion) { this.OverriddenMajorVersion = overriddenMajorVersion; return this; }
        public SampleData WithOverriddenMinorVersion(int? overriddenMinorVersion) { this.OverriddenMinorVersion = overriddenMinorVersion; return this; }
        public SampleData WithOverriddenPatchVersion(int? overriddenPatchVersion) { this.OverriddenPatchVersion = overriddenPatchVersion; return this; }
        public SampleData WithCurrentBranch(string currentBranch) { this.CurrentBranch = currentBranch; return this; }
        public SampleData WithCurrentSha(string currentSha) { this.CurrentSha = currentSha; return this; }
        public SampleData WithOverriddenBuildMetadata(string overriddenBuildMetadata) { this.OverriddenBuildMetadata = overriddenBuildMetadata; return this; }
        public SampleData WithVersion(SimpleVersion version) { this.Version = version; return this; }
        public SampleData ExpectResult(string expected) { this.Expected = expected; return this; }
    }
}
