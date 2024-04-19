#!/bin/bash
set -e

BASEDIR=$(dirname "$0")

if [[ $# -ne 3 ]]; then
  echo "Usage: ./deploy.sh <version> <git-user> <git-token>"
  echo "E.g.: ./deploy.sh 0.5.0 vigan-abd ghp_S1..."
  exit 1
fi

dotnet restore
dotnet build
cd "$BASEDIR/Grenache"
cp ./nuget.config.example ./nuget.config
sed -i "s/__USER__/$2/g" ./nuget.config
sed -i "s/__GITHUB_TOKEN__/$3/g" ./nuget.config
dotnet pack --configuration Release
dotnet nuget push bin/Release/Grenache.$1.nupkg --source github
