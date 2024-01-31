#!/bin/bash
set -e
BASEDIR=$(dirname "$0")

PM2="$BASEDIR/node_modules/.bin/pm2"
DOTNET=dotnet
PM2_CONF_PATH="$BASEDIR/pm2.config.json"
PROJ_PATH="$BASEDIR/Grenache.Test.csproj"

$PM2 restart $PM2_CONF_PATH
$DOTNET test $PROJ_PATH
$PM2 stop grape-1 grape-2
$PM2 delete grape-1 grape-2
