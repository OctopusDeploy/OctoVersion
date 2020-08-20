using System.Collections.Generic;
using OctoVersion.Contracts;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests
{
    public class WhenParsingAVersion
    {
        [Theory]
        [MemberData(nameof(TestCases))]
        public void TheResultingVersionShouldBeCorrect(string input, VersionInfo expected)
        {
            VersionInfo.TryParse(input).ShouldBe(expected);
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {"0.0.0", new VersionInfo(0, 0, 0)};
            yield return new object[] {"0.0", new VersionInfo(0, 0, 0)};
            yield return new object[] {"0", new VersionInfo(0, 0, 0)};
            yield return new object[] {"1", new VersionInfo(1, 0, 0)};
            yield return new object[] {"1.2", new VersionInfo(1, 2, 0)};
            yield return new object[] {"1.2.3", new VersionInfo(1, 2, 3)};
            yield return new object[] {"1.2.3-alpha", null};
            yield return new object[] {"1+some-build-info", new VersionInfo(1, 0, 0)};
            yield return new object[] {"1.2+some-build-info", new VersionInfo(1, 2, 0)};
            yield return new object[] {"1.2.3+some-build-info", new VersionInfo(1, 2, 3)};
        }
    }
}