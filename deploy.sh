#!/usr/bin/env bash

echo "[INFO] Download nuget"
wget -nc https://dist.nuget.org/win-x86-commandline/latest/nuget.exe;

echo "[INFO] Create nuget package"
mono nuget.exe pack cyberblast.SharePoint.Client.nuspec -properties version=1.0.$TRAVIS_BUILD_NUMBER && \
echo "[INFO] Deploy nuget package"
mono nuget.exe push cyberblast.SharePoint.Client.1.0.$TRAVIS_BUILD_NUMBER.nupkg -ApiKey $NUGET_API_KEY -Verbosity detailed -Source https://www.nuget.org/api/v2/package 