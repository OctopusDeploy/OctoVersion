using System.Collections.Generic;
using OctoVersion.Contracts;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests
{
    public class WhenStringifyingAVersion
    {
        [Theory]
        [MemberData(nameof(TestCases))]
        public void TheOutputShouldBeCorrect(VersionInfo version, string expected)
        {
            version.ToString().ShouldBe(expected);
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] {new VersionInfo(0, 0, 0), "0.0.0"};
            yield return new object[] {new VersionInfo(1, 2, 3), "1.2.3"};
            yield return new object[] {new VersionInfo(1, 2, 3, "alpha"), "1.2.3-alpha"};
            yield return new object[] {new VersionInfo(1, 2, 3, "alpha", "some-build-info"), "1.2.3-alpha+some-build-info"};
            yield return new object[] {new VersionInfo(1, 2, 3, "", "some-build-info"), "1.2.3+some-build-info"};
        }
    }
}