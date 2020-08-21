#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0

using Path = System.IO.Path;
using IO = System.IO;
using Cake.Common.Tools;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var testFilter = Argument("where", "");

var localPackagesDir = "../LocalPackages";
var artifactsDir = "./artifacts";

// We have to bootstrap things somwehere...
var buildNumber = System.Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "0.0.0";

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
        DotNetCoreRestore("source", new DotNetCoreRestoreSettings
        {
            ArgumentCustomization = args => args.Append($"-p:Version={buildNumber} -p:InformationalVersion=${buildNumber}")
        });
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetCorePublish("./source", new DotNetCorePublishSettings
        {
            Configuration = configuration,
            NoRestore = true,
            ArgumentCustomization = args => args.Append($"-p:Version={buildNumber} -p:InformationalVersion=${buildNumber}")
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