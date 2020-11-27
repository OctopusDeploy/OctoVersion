#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#tool "nuget:?package=TeamCity.Dotnet.Integration&version=1.0.10"
#addin "nuget:?package=Cake.OctoVersion&version=0.0.138"

// The list below is to manually resolve NuGet dependencies to work around a bug in Cake's dependency loader.
// Our intention is to remove this list again once the Cake bug is fixed.
//
// What we want:
// #addin "nuget:?package=Cake.OctoVersion&version=0.0.138&loaddependencies=true"
// (Note the loaddependencies=true parameter.)
//
// Our workaround:
#addin "nuget:?package=LibGit2Sharp&version=0.26.2"
#addin "nuget:?package=Serilog&version=2.8.0.0"
#addin "nuget:?package=Serilog.Settings.Configuration&version=3.1.0.0"
#addin "nuget:?package=Serilog.Sinks.Console&version=3.0.1.0"
#addin "nuget:?package=Serilog.Sinks.Literate&version=3.0.0.0"
#addin "nuget:?package=SerilogMetrics&version=2.1.0.0"
#addin "nuget:?package=OctoVersion.Core&version=0.0.138"
#addin "nuget:?package=Cake.OctoVersion&version=0.0.138"
#addin "nuget:?package=Microsoft.Extensions.Primitives&version=3.1.7"
#addin "nuget:?package=Microsoft.Extensions.Configuration&version=3.1.7.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.Abstractions&version=3.1.7.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.Binder&version=3.1.7.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.CommandLine&version=3.1.7.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.EnvironmentVariables&version=3.1.7.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.FileExtensions&version=3.1.0.0"
#addin "nuget:?package=Microsoft.Extensions.Configuration.Json&version=3.1.7"
#addin "nuget:?package=Microsoft.Extensions.DependencyModel&version=2.0.4.0"
#addin "nuget:?package=Microsoft.Extensions.FileProviders.Abstractions&version=3.1.0.0"
#addin "nuget:?package=Microsoft.Extensions.FileProviders.Physical&version=3.1.0.0"

using Path = System.IO.Path;
using IO = System.IO;
using Cake.Common.Tools;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var testFilter = Argument("where", "");

var localPackagesDir = "../LocalPackages";
var artifactsDir = "./artifacts";

if (!BuildSystem.IsRunningOnTeamCity) OctoVersionDiscoverLocalGitBranch(out _);
OctoVersion(out var versionInfo);

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("Test")
    .IsDependentOn("CopyToLocalPackages");

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(artifactsDir);
        CleanDirectories("./source/**/bin");
        CleanDirectories("./source/**/obj");
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetCoreRestore("./source");
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCoreBuild("./source", new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            NoRestore = true,
            ArgumentCustomization = args => args.Append($"/p:Version={versionInfo.FullSemVer} /p:InformationalVersion={versionInfo.FullSemVer}"),
            MSBuildSettings = new DotNetCoreMSBuildSettings
            {
                ArgumentCustomization = args => args.Append($"/p:Version={versionInfo.FullSemVer} /p:InformationalVersion={versionInfo.FullSemVer}")
            }
        });

        DotNetCorePublish("./source", new DotNetCorePublishSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true,
            ArgumentCustomization = args => args.Append($"/p:Version={versionInfo.FullSemVer} /p:InformationalVersion=${versionInfo.FullSemVer}")
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => {

        DotNetCoreTest("./source", new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            ArgumentCustomization = args => {
                if(!string.IsNullOrEmpty(testFilter)) {
                    args = args.Append("--where").AppendQuoted(testFilter);
                }
                return args.Append("--logger:trx")
                    .Append($"--verbosity normal");
            }
        });
	});

Task("PublishArtifacts")
    .IsDependentOn("Test")
    .Does(() => {
        CreateDirectory(artifactsDir);
        CopyFiles($"source/**/*.nupkg", artifactsDir);
        CopyFiles($"source/**/*.trx", artifactsDir);
    });

Task("CopyToLocalPackages")
    .WithCriteria(BuildSystem.IsLocalBuild)
    .IsDependentOn("PublishArtifacts")
    .Does(() =>
    {
        CreateDirectory(localPackagesDir);
        CopyFiles($"{artifactsDir}/*.nupkg", localPackagesDir);
    });

RunTarget(target);
