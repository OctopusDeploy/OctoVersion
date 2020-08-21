# Welcome to OctoVersion

## Getting started

Install the tool into the global tool registry:

```
dotnet tool install --global OctoVersion.Tool
```

Run it against a local Git repository:

```
cd ~/code/my-app
octoversion CurrentBranch=main
```

You should see output that looks something like this:

```
[22:16:32 INF] Version is 0.0.5
```

## Integrating with build tools

### TeamCity

```bash
octoversion CurrentBranch=main OutputFormat:0=TeamCity
```

```
##teamcity[buildNumber '0.0.5']
##teamcity[setParameter name='env.OCTOVERSION_Major' value='0']
##teamcity[setParameter name='env.OCTOVERSION_Minor' value='0']
##teamcity[setParameter name='env.OCTOVERSION_Patch' value='5']
##teamcity[setParameter name='env.OCTOVERSION_MajorMinorPatch' value='0.0.5']
##teamcity[setParameter name='env.OCTOVERSION_PreReleaseTag' value='']
##teamcity[setParameter name='env.OCTOVERSION_PreReleaseTagWithDash' value='']
##teamcity[setParameter name='env.OCTOVERSION_BuildMetadata' value='']
##teamcity[setParameter name='env.OCTOVERSION_BuildMetadataWithPlus' value='']
##teamcity[setParameter name='env.OCTOVERSION_FullSemVer' value='0.0.5']
```

### Environment variables

```bash
octoversion CurrentBranch=main OutputFormat:0=Environment
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
octoversion CurrentBranch=main OutputFormat:0=Json
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

with the later sources overriding the earlier ones.

## FAQ

### Can't you figure out the branch I'm on? Why do I have to specify it?

This way, madness lies.

It's important to remember that _branches in Git are a lie_. Branches are nothing more than pointers to a commit. It's entirely possible (and very likely) that a commit will be reachable from many branches - or no branches, if you've checked out a commit and have a detached head.

Rather than attempting to guess (and sneakily getting the answer incorrect), OctoVersion requires you to tell it which branch you're on. All good build servers will provide this as an environment variable or other build parameter, and it's trivial to pass the information to OctoVersion.

For local versioning, it's always possible to set up a `bash`/`zsh` alias along these lines:

```bash
alias ov="octoversion CurrentBranch=`git branch --show-current` OutputFormat:0=Console"
```
