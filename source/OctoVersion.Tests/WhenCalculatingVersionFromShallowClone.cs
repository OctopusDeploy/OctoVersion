using System;
using OctoVersion.Core.VersionNumberCalculation;
using Shouldly;
using Xunit;

namespace OctoVersion.Tests;

/// <summary>
/// Tests for VersionCalculator when the commit graph has been truncated, as happens
/// when VersionCalculatorFactory silently skips parents that fall outside a shallow
/// clone's available history.
/// </summary>
public class WhenCalculatingVersionFromShallowClone
{
    static SimpleCommit MakeCommit(string hash, DateTimeOffset timestamp, string message = "normal commit")
    {
        return new SimpleCommit(hash,
            message,
            timestamp,
            false,
            false);
    }

    static SimpleCommit MakeMajorBumpCommit(string hash, DateTimeOffset timestamp)
    {
        return new SimpleCommit(hash,
            "+semver: major",
            timestamp,
            true,
            false);
    }

    static SimpleCommit MakeMinorBumpCommit(string hash, DateTimeOffset timestamp)
    {
        return new SimpleCommit(hash,
            "+semver: minor",
            timestamp,
            false,
            true);
    }

    [Fact]
    public void WhenShallowBoundaryHasVersionTag_VersionIsCalculatedRelativeToTag()
    {
        // Simulates:
        //   C  (no parents — shallow boundary, tagged 2.0.0)
        //   B  (parent: C)
        //   A  (parent: B)  ← HEAD
        // Expected: HEAD = 2.0.2

        var baseTime = new DateTimeOffset(2024,
            1,
            1,
            0,
            0,
            0,
            TimeSpan.Zero);

        var commitC = MakeCommit("ccc", baseTime);
        var commitB = MakeCommit("bbb", baseTime.AddMinutes(1));
        var commitA = MakeCommit("aaa", baseTime.AddMinutes(2));

        commitC.TagWith(new SimpleVersion(2, 0, 0));
        commitB.AddParent(commitC);
        commitA.AddParent(commitB);

        var calculator = new VersionCalculator([commitA, commitB, commitC], commitA.Hash);
        var version = calculator.GetVersion();

        version.Major.ShouldBe(2);
        version.Minor.ShouldBe(0);
        version.Patch.ShouldBe(2);
    }

    [Fact]
    public void WhenShallowBoundaryHasNoTag_VersionCountsFromZero()
    {
        // Simulates:
        //   B  (no parents — shallow boundary, no tag)
        //   A  (parent: B)  ← HEAD
        // The shallow boundary commit itself counts as 0.0.1, so HEAD = 0.0.2

        var baseTime = new DateTimeOffset(2024,
            1,
            1,
            0,
            0,
            0,
            TimeSpan.Zero);

        var commitB = MakeCommit("bbb", baseTime);
        var commitA = MakeCommit("aaa", baseTime.AddMinutes(1));

        commitA.AddParent(commitB);

        var calculator = new VersionCalculator([commitA, commitB], commitA.Hash);
        var version = calculator.GetVersion();

        version.Major.ShouldBe(0);
        version.Minor.ShouldBe(0);
        version.Patch.ShouldBe(2);
    }

    [Fact]
    public void WhenShallowBoundaryIsTheCurrentCommit_VersionIsCalculatedFromZero()
    {
        // Simulates a very shallow clone — the HEAD commit itself is the boundary (no parents).
        //   A  (no parents — shallow boundary)  ← HEAD
        // Expected: 0.0.1

        var commitA = MakeCommit("aaa",
            new DateTimeOffset(2024,
                1,
                1,
                0,
                0,
                0,
                TimeSpan.Zero));

        var calculator = new VersionCalculator([commitA], commitA.Hash);
        var version = calculator.GetVersion();

        version.Major.ShouldBe(0);
        version.Minor.ShouldBe(0);
        version.Patch.ShouldBe(1);
    }

    [Fact]
    public void WhenMergeCommitHasOneParentCutByShallowClone_VersionUsesRemainingParent()
    {
        // Simulates a merge where one branch was deeper than the shallow depth and its
        // root was stripped. The factory's TryGetValue skip means only one parent is linked.
        //
        //   C  (no parents — shallow boundary, tagged 3.0.0)
        //   B  (parent: C)
        //   A  (parent: B — the other merge parent was outside shallow history)  ← HEAD

        var baseTime = new DateTimeOffset(2024,
            1,
            1,
            0,
            0,
            0,
            TimeSpan.Zero);

        var commitC = MakeCommit("ccc", baseTime);
        var commitB = MakeCommit("bbb", baseTime.AddMinutes(1));
        var commitA = MakeCommit("aaa", baseTime.AddMinutes(2));

        commitC.TagWith(new SimpleVersion(3, 0, 0));
        commitB.AddParent(commitC);
        // commitA is a merge commit, but its other parent (e.g. "ddd") was outside the
        // shallow history so the factory skipped it — only commitB is linked.
        commitA.AddParent(commitB);

        var calculator = new VersionCalculator([commitA, commitB, commitC], commitA.Hash);
        var version = calculator.GetVersion();

        version.Major.ShouldBe(3);
        version.Minor.ShouldBe(0);
        version.Patch.ShouldBe(2);
    }

    [Fact]
    public void WhenVersionTagExistsWithinShallowHistory_TagTakesPrecedenceOverCountedCommits()
    {
        // Confirms that when a version tag exists somewhere in the available (truncated)
        // history, it still wins over the commit-counting approach — shallow or not.
        //
        //   D  (no parents — shallow boundary)
        //   C  (parent: D, tagged 1.5.0)
        //   B  (parent: C)
        //   A  (parent: B)  ← HEAD
        // Expected: HEAD = 1.5.2

        var baseTime = new DateTimeOffset(2024,
            1,
            1,
            0,
            0,
            0,
            TimeSpan.Zero);

        var commitD = MakeCommit("ddd", baseTime);
        var commitC = MakeCommit("ccc", baseTime.AddMinutes(1));
        var commitB = MakeCommit("bbb", baseTime.AddMinutes(2));
        var commitA = MakeCommit("aaa", baseTime.AddMinutes(3));

        commitC.TagWith(new SimpleVersion(1, 5, 0));
        commitC.AddParent(commitD);
        commitB.AddParent(commitC);
        commitA.AddParent(commitB);

        var calculator = new VersionCalculator([commitA, commitB, commitC, commitD], commitA.Hash);
        var version = calculator.GetVersion();

        version.Major.ShouldBe(1);
        version.Minor.ShouldBe(5);
        version.Patch.ShouldBe(2);
    }

    [Fact]
    public void WhenMajorBumpCommitIsAboveShallowBoundary_MajorVersionIsIncremented()
    {
        // Confirms that semver bump semantics still work correctly when the
        // base commit chain starts from a shallow boundary.
        //
        //   C  (no parents — shallow boundary, tagged 1.0.0)
        //   B  (parent: C, +semver: major)
        //   A  (parent: B)  ← HEAD
        // Expected: B = 2.0.0, HEAD = 2.0.1

        var baseTime = new DateTimeOffset(2024,
            1,
            1,
            0,
            0,
            0,
            TimeSpan.Zero);

        var commitC = MakeCommit("ccc", baseTime);
        var commitB = MakeMajorBumpCommit("bbb", baseTime.AddMinutes(1));
        var commitA = MakeCommit("aaa", baseTime.AddMinutes(2));

        commitC.TagWith(new SimpleVersion(1, 0, 0));
        commitB.AddParent(commitC);
        commitA.AddParent(commitB);

        var calculator = new VersionCalculator([commitA, commitB, commitC], commitA.Hash);
        var version = calculator.GetVersion();

        version.Major.ShouldBe(2);
        version.Minor.ShouldBe(0);
        version.Patch.ShouldBe(1);
    }

    [Fact]
    public void WhenMinorBumpCommitIsAboveShallowBoundary_MinorVersionIsIncremented()
    {
        // Confirms that minor version bumps work correctly when the
        // base commit chain starts from a shallow boundary.
        //
        //   C  (no parents — shallow boundary, tagged 1.0.0)
        //   B  (parent: C, +semver: minor)
        //   A  (parent: B)  ← HEAD
        // Expected: B = 1.1.0, HEAD = 1.1.1

        var baseTime = new DateTimeOffset(2024,
            1,
            1,
            0,
            0,
            0,
            TimeSpan.Zero);

        var commitC = MakeCommit("ccc", baseTime);
        var commitB = MakeMinorBumpCommit("bbb", baseTime.AddMinutes(1));
        var commitA = MakeCommit("aaa", baseTime.AddMinutes(2));

        commitC.TagWith(new SimpleVersion(1, 0, 0));
        commitB.AddParent(commitC);
        commitA.AddParent(commitB);

        var calculator = new VersionCalculator([commitA, commitB, commitC], commitA.Hash);
        var version = calculator.GetVersion();

        version.Major.ShouldBe(1);
        version.Minor.ShouldBe(1);
        version.Patch.ShouldBe(1);
    }
}