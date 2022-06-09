// ReSharper disable RedundantUsingDirective - prevent PrettyBot from getting confused about unused code.

using System;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.OctoVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    const string CiBranchNameEnvVariable = "OCTOVERSION_CurrentBranch";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Test filter expression", Name = "where")] readonly string TestFilter = string.Empty;

    [Solution] readonly Solution Solution;

    [Parameter("Whether to auto-detect the branch name - this is okay for a local build, but should not be used under CI.")] readonly bool AutoDetectBranch = IsLocalBuild;

    [OctoVersion(BranchParameter = nameof(BranchName), AutoDetectBranchParameter = nameof(AutoDetectBranch))] public OctoVersionInfo OctoVersionInfo;

    [Parameter("Branch name for OctoVersion to use to calculate the version number. Can be set via the environment variable " + CiBranchNameEnvVariable + ".", Name = CiBranchNameEnvVariable)]
    string BranchName { get; set; }

    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath LocalPackagesDirectory => RootDirectory / ".." / "LocalPackages";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj", "**/TestResults").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .SetVersion(OctoVersionInfo.FullSemVer)
                .SetInformationalVersion(OctoVersionInfo.InformationalVersion));
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetLoggers("trx")
                .SetVerbosity(DotNetVerbosity.Normal)
                .SetFilter(TestFilter)
                .EnableNoBuild()
                .EnableNoRestore());
            GlobFiles(SourceDirectory, "**/*.trx").ForEach(x => CopyFileToDirectory(x, ArtifactsDirectory));
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .DependsOn(Test)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .EnableNoBuild()
                .AddProperty("Version", OctoVersionInfo.FullSemVer)
            );
        });

    Target CopyToLocalPackages => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .TriggeredBy(Pack)
        .Executes(() =>
        {
            EnsureExistingDirectory(LocalPackagesDirectory);
            GlobFiles(ArtifactsDirectory, $"*.{OctoVersionInfo.FullSemVer}.nupkg").ForEach(x => CopyFileToDirectory(x, LocalPackagesDirectory));
        });

    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Pack);
}