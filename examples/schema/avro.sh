#!/usr/bin/env bash
#######################################################################
# Generate C# classes from Apache Avro IDL
#
# Run `./avro.sh --help` for usage instuctions.
#
# Prerequities
#
# This script depends upon the following tools being installed:
# - java (version 1.8 or higher)
# - avrogen
# - curl
#######################################################################

SCRIPT_NAME=$(basename $0)
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
BUILD_DIR="$SCRIPT_DIR/build"
GENERATED_AVRO_DIR="$BUILD_DIR/generated/avro"
GENERATED_CSHARP_DIR="$BUILD_DIR/generated/csharp"

MAVEN_CENTRAL_URL="https://repo1.maven.org/maven2"
AVRO_TOOLS_VERSION="1.10.2"
AVRO_TOOLS_URL="$MAVEN_CENTRAL_URL/org/apache/avro/avro-tools/$AVRO_TOOLS_VERSION/avro-tools-$AVRO_TOOLS_VERSION.jar"
AVRO_TOOLS_LOCAL_DIR="$HOME/.avro"
AVRO_TOOLS_JAR="$AVRO_TOOLS_LOCAL_DIR/avro-tools.jar"

function sub_install_avro_tools() {
  echo "Checking avro-tools installation"
  if [[ -f $AVRO_TOOLS_JAR ]]
  then
    echo "-> avro-tools already installed at $AVRO_TOOLS_JAR"
  else
    echo "Installing avro-tools version $AVRO_TOOLS_VERSION to $AVRO_TOOLS_JAR"
    curl --create-dirs --output $AVRO_TOOLS_JAR $AVRO_TOOLS_URL
  fi
}

function sub_generate_avsc() {
  echo "Generating schema from idl into $GENERATED_AVRO_DIR"
  mkdir -p $GENERATED_AVRO_DIR
  for f in "$SCRIPT_DIR/avro"/*.avdl
  do
    echo "-> Converting $f"
    java -jar $AVRO_TOOLS_JAR idl2schemata $f $GENERATED_AVRO_DIR
  done
}

function sub_generate_csharp() {
  echo "Generating C# from schema into $GENERATED_CSHARP_DIR"
  mkdir -p $GENERATED_CSHARP_DIR
  for f in "$GENERATED_AVRO_DIR"/*.avsc
  do
    echo "-> Converting $f"
    avrogen -s $f $GENERATED_CSHARP_DIR; \
  done
}

function sub_generate() {
  sub_install_avro_tools
  sub_generate_avsc
  sub_generate_csharp
}

function sub_clean() {
  echo "Cleaning generated files"
  rm -rf "$BUILD_DIR"
}

function sub_help() {
  echo "Usage: $SCRIPT_NAME <subcommand>"
  echo "Subcommands:"
  echo "    clean       clean up generated files"
  echo "    generate    generate C# from Avro IDL files"
  echo "    help        show this help message"
  echo ""
}

SUBCOMMAND=$1
case $SUBCOMMAND in
  "" | "-h" | "--help")
    sub_help
    ;;
  *)
    shift
    sub_${SUBCOMMAND} "$@"
    if [ $? = 127 ]; then
        echo "Error: '$SUBCOMMAND' is not a known subcommand." >&2
        echo "       Run '$SCRIPT_NAME --help' for a list of known subcommands." >&2
        exit 1
    fi
    ;;
esac
