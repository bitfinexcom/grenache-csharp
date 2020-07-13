#!/bin/bash
set -e

BASEDIR=$(dirname "$0")

if [[ $# -ne 2 ]]; then
  echo "Usage: ./deploy.sh <git-user> <git-token>"
  exit 1
fi

cd "$BASEDIR/Grenache"
cp ./nuget.config.example ./nuget.config
sed -i "s/__USER__/$1/g" ./nuget.config
sed -i "s/__GITHUB_TOKEN__/$2/g" ./nuget.config
dotnet pack --configuration Release
dotnet nuget push bin/Release/Grenache.0.0.1.2.nupkg --source github
