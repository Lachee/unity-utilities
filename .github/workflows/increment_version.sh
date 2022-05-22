#!/bin/bash

FILE_PATH=$1

# Fetches the version from the given file
get_version() {
  echo $(cat $1 | grep version | head -1 | awk -F: '{ print $2 }' | sed 's/[", \]//g')
}

# Increments version given to it
increment_version() {
  while getopts ":Mmp" Option; do
    case $Option in
    M) major=true ;;
    m) minor=true ;;
    p) patch=true ;;
    esac
  done

  shift $(($OPTIND - 1))

  version=$1

  # Build array from version string.

  a=(${version//./ })

  # If version string is missing or has the wrong number of members, show usage message.

  if [ ${#a[@]} -ne 3 ]; then
    echo "usage: $(basename $0) [-Mmp] major.minor.patch"
    exit 1
  fi

  # Increment version numbers as requested.

  if [ ! -z $major ]; then
    ((a[0]++))
    a[1]=0
    a[2]=0
  fi

  if [ ! -z $minor ]; then
    ((a[1]++))
    a[2]=0
  fi

  if [ ! -z $patch ]; then
    ((a[2]++))
  fi

  echo "${a[0]}.${a[1]}.${a[2]}"
}

# Bump the versioning
PACKAGE_VERSION=$(get_version $FILE_PATH)
echo "Package: $PACKAGE_VERSION"

BUMP_VERSION=$(increment_version -p $PACKAGE_VERSION)
echo "Bumped: $BUMP_VERSION"

# Write it back
sed -i "s/$PACKAGE_VERSION/$BUMP_VERSION/g" "$FILE_PATH"
