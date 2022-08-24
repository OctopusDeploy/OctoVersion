[![feedz.io](https://img.shields.io/badge/endpoint.svg?url=https%3A%2F%2Ff.feedz.io%2Foctopus-deploy%2Fdependencies%2Fshield%2FNuke.OctoVersion%2Fstable&label=Nuke.OctoVersion)](https://f.feedz.io/octopus-deploy/dependencies/packages/Nuke.OctoVersion/stable/download)

# Welcome to OctoVersion

## What is OctoVersion?

OctoVersion is a dotnet tool for deriving a SemVer 2.0 version number based on a Git revision graph.

## Getting started

Install the tool into the global tool registry:

```
dotnet tool install --global Octopus.OctoVersion.Tool
```

or into the local project's tool manifest:

```
dotnet tool install Octopus.OctoVersion.Tool
```

Run it against a local Git repository:

```
cd ~/code/my-app
octoversion --CurrentBranch=main # if installed globally
dotnet tool run octoversion --CurrentBranch=main # if installed locally
```

You should see output that looks something like this:

```
[22:16:32 INF] Version is 0.0.5
```

## Integrating with build tools

### Nuke

Add a reference to `OctoVersion.Tool` in your Nuke `_build` project. This can be done using the nuke command:

```
nuke :add-package Octopus.OctoVersion.Tool --version X.X.X
```

Add a field marked with `[OctoVersion]` and it will be auto-populated:

```c#
using Nuke.Common
using Nuke.Common.Git;
using Nuke.Common.Tools.OctoVersion;
using Serilog;

class Build : NukeBuild
{
     [GitRepository]
     readonly GitRepository GitRepository;

     string Branch => GitRepository.Branch;

     // The Required Attribute will automatically throw an exception if the 
     // OctoVersionInfo parameter is not set due to an error or misconfiguration in Nuke.
     [Required]
     // 'Framework = "net6.0"' is only required for net6.0 apps.
     [OctoVersion(UpdateBuildNumber = true, 
                  BranchParameter = nameof(Branch),
                  Framework = "net6.0")]
     readonly OctoVersionInfo OctoVersionInfo;
     
     Target PrintVersion => _ => _
     .Executes(() => {
                Log.Information($"The version is {OctoVersion.MajorMinorBuild}");
     });
}
```

Take a look at the Nuke [OctoVersionAttribute](https://github.com/nuke-build/nuke/blob/master/source/Nuke.Common/Tools/OctoVersion/OctoVersionAttribute.cs) for all available attribute properties. 

### Cake

```csharp
#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#addin "nuget:?package=Cake.OctoVersion&version=0.3.63"

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
#addin "nuget:?package=Octopus.OctoVersion.Core&version=0.3.63"
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

if (!BuildSystem.IsRunningOnTeamCity) OctoVersionDiscoverLocalGitBranch(out _);
OctoVersion(out var versionInfo);

Task("Default")
    .IsDependentOn("Build");

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
    });

RunTarget(target);
```
### TeamCity

```bash
octoversion --CurrentBranch=main --OutputFormats:0=TeamCity
```

```
##teamcity[buildNumber '0.0.12']
##teamcity[setParameter name='env.OCTOVERSION_Major' value='0']
##teamcity[setParameter name='OctoVersion.Major' value='0']
##teamcity[setParameter name='env.OCTOVERSION_Minor' value='0']
##teamcity[setParameter name='OctoVersion.Minor' value='0']
##teamcity[setParameter name='env.OCTOVERSION_Patch' value='12']
##teamcity[setParameter name='OctoVersion.Patch' value='12']
##teamcity[setParameter name='env.OCTOVERSION_MajorMinorPatch' value='0.0.12']
##teamcity[setParameter name='OctoVersion.MajorMinorPatch' value='0.0.12']
##teamcity[setParameter name='env.OCTOVERSION_PreReleaseTag' value='']
##teamcity[setParameter name='OctoVersion.PreReleaseTag' value='']
##teamcity[setParameter name='env.OCTOVERSION_PreReleaseTagWithDash' value='']
##teamcity[setParameter name='OctoVersion.PreReleaseTagWithDash' value='']
##teamcity[setParameter name='env.OCTOVERSION_BuildMetadata' value='']
##teamcity[setParameter name='OctoVersion.BuildMetadata' value='']
##teamcity[setParameter name='env.OCTOVERSION_BuildMetadataWithPlus' value='']
##teamcity[setParameter name='OctoVersion.BuildMetadataWithPlus' value='']
##teamcity[setParameter name='env.OCTOVERSION_FullSemVer' value='0.0.12']
##teamcity[setParameter name='OctoVersion.FullSemVer' value='0.0.12']```
```

### Environment variables

```bash
octoversion --CurrentBranch=main --OutputFormats:0=Environment
```

```
OCTOVERSION_Major=0
OCTOVERSION_Minor=0
OCTOVERSION_Patch=5
OCTOVERSION_MajorMinorPatch=0.0.5
OCTOVERSION_PreReleaseTag=
OCTOVERSION_PreReleaseTagWithDash=
OCTOVERSION_BuildMetadata=
OCTOVERSION_BuildMetadataWithPlus=
OCTOVERSION_FullSemVer=0.0.5
```

### JSON

```bash
octoversion --CurrentBranch=main --OutputFormats:0=Json
```

```
{
  "Major": 0,
  "Minor": 0,
  "Patch": 5,
  "MajorMinorPatch": "0.0.5",
  "PreReleaseTag": "",
  "PreReleaseTagWithDash": "",
  "BuildMetadata": "",
  "BuildMetadataWithPlus": "",
  "FullSemVer": "0.0.5"
}
```

## Configuration

OctoVersion sources configuration from:

1. The first, if any, `octoversion.json` file (lower case!) it finds walking up from the current directory.
1. Environment variables prefixed with the OCTOVERSION_ prefix.
1. Command-line parameters.

with the later sources overriding the earlier ones. The configuration syntax for each is as per the standard [.NET Core standard configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/) providers:

- `octoversion.json` uses the [JSON Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#file-configuration-provider) in the same way that `appsettings.json` files do.
    - Yes, this means that you can hook in a customer Serilog sink using `octoversion.json` if you like :)
- Environment variables use the [Environment Variables Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#evcp).
- Command-line variables use the [Command-line Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#command-line).

## FAQ

### Can't you figure out the branch I'm on? Why do I have to specify it?

This way, madness lies.

It's important to remember that _branches in Git are a lie_. Branches are nothing more than pointers to a commit. It's entirely possible (and very likely) that a commit will be reachable from many branches - or no branches, if you've checked out a commit and have a detached head.

Rather than attempting to guess (and sneakily getting the answer incorrect), OctoVersion requires you to tell it which branch you're on. All good build servers will provide this as an environment variable or other build parameter, and it's trivial to pass the information to OctoVersion.

For local versioning, it's always possible to set up a `bash`/`zsh` alias along these lines:

```bash
alias ov="octoversion --CurrentBranch=`git branch --show-current`"
```
