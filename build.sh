#!/usr/bin/env bash

bash --version 2>&1 | head -n 1

set -eo pipefail
SCRIPT_DIR=$(cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd)

###########################################################################
# CONFIGURATION
###########################################################################

BUILD_PROJECT_FILE="$SCRIPT_DIR/build/_build.csproj"
TEMP_DIRECTORY="$SCRIPT_DIR//.nuke/temp"

DOTNET_GLOBAL_FILE="$SCRIPT_DIR//global.json"
DOTNET_INSTALL_URL="https://dot.net/v1/dotnet-install.sh"
DOTNET_CHANNEL="Current"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_MULTILEVEL_LOOKUP=0

#### START CUSTOM CODE ####
# This file is an odd one. `nuke :update` wants to make edits to it, so we want to keep it similar to the default 
# as we can. But, we want to do some tricky stuff, and make sure both dotnet 6 and dotnet 7 are installed, so we
# can compile for both versions. So... We've got our "custom code" block here, which we want to keep, and the 
# default code down below so we can see what changes are made upstream, and apply them to ours if we need to.

# Download install script
DOTNET_INSTALL_FILE="$TEMP_DIRECTORY/dotnet-install.sh"
mkdir -p "$TEMP_DIRECTORY"
curl -Lsfo "$DOTNET_INSTALL_FILE" "$DOTNET_INSTALL_URL"
chmod +x "$DOTNET_INSTALL_FILE"

# check if we've got the versions we expect
has_net_6=false
has_net_7=false
for sdk_version in $(dotnet --list-sdks | awk '{print $1}'); do
    if [[ "$sdk_version" == 6.* ]]; then
        has_net_6=true
    elif [[ "$sdk_version" == 7.* ]]; then
        has_net_7=true
    fi
done

if ! $has_net_6; then
    "$DOTNET_INSTALL_FILE" --install-dir "$DOTNET_DIRECTORY" --version "6.0" --no-path
fi
if ! $has_net_7; then
    "$DOTNET_INSTALL_FILE" --install-dir "$DOTNET_DIRECTORY" --version "7.0" --no-path
fi

export DOTNET_EXE="$DOTNET_DIRECTORY/dotnet"

echo "Microsoft (R) .NET SDK version $("$DOTNET_EXE" --version)"

"$DOTNET_EXE" build "$BUILD_PROJECT_FILE" /nodeReuse:false /p:UseSharedCompilation=false -nologo -clp:NoSummary --verbosity quiet
"$DOTNET_EXE" run --project "$BUILD_PROJECT_FILE" --no-build -- "$@"

exit $?

##### END CUSTOM CODE #####


###########################################################################
# EXECUTION
###########################################################################

function FirstJsonValue {
    perl -nle 'print $1 if m{"'"$1"'": "([^"]+)",?}' <<< "${@:2}"
}

# If dotnet CLI is installed globally and it matches requested version, use for execution
if [ -x "$(command -v dotnet)" ] && dotnet --version &>/dev/null; then
    export DOTNET_EXE="$(command -v dotnet)"
else
    # Download install script
    DOTNET_INSTALL_FILE="$TEMP_DIRECTORY/dotnet-install.sh"
    mkdir -p "$TEMP_DIRECTORY"
    curl -Lsfo "$DOTNET_INSTALL_FILE" "$DOTNET_INSTALL_URL"
    chmod +x "$DOTNET_INSTALL_FILE"

    # If global.json exists, load expected version
    if [[ -f "$DOTNET_GLOBAL_FILE" ]]; then
        DOTNET_VERSION=$(FirstJsonValue "version" "$(cat "$DOTNET_GLOBAL_FILE")")
        if [[ "$DOTNET_VERSION" == ""  ]]; then
            unset DOTNET_VERSION
        fi
    fi

    # Install by channel or version
    DOTNET_DIRECTORY="$TEMP_DIRECTORY/dotnet-unix"
    if [[ -z ${DOTNET_VERSION+x} ]]; then
        "$DOTNET_INSTALL_FILE" --install-dir "$DOTNET_DIRECTORY" --channel "$DOTNET_CHANNEL" --no-path
    else
        "$DOTNET_INSTALL_FILE" --install-dir "$DOTNET_DIRECTORY" --version "$DOTNET_VERSION" --no-path
    fi
    export DOTNET_EXE="$DOTNET_DIRECTORY/dotnet"
fi

echo "Microsoft (R) .NET SDK version $("$DOTNET_EXE" --version)"

"$DOTNET_EXE" build "$BUILD_PROJECT_FILE" /nodeReuse:false /p:UseSharedCompilation=false -nologo -clp:NoSummary --verbosity quiet
"$DOTNET_EXE" run --project "$BUILD_PROJECT_FILE" --no-build -- "$@"
