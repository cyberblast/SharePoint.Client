#!/usr/bin/env bash

echo "[INFO] Download nuget"
wget -nc https://dist.nuget.org/win-x86-commandline/latest/nuget.exe;
mozroots --import --sync

echo "[INFO] Create and deploy new nuget package"
mono nuget.exe pack cyberblast.SharePoint.Client.nuspec && \
mono nuget.exe push cyberblast.SharePoint.Client.nupkg -ApiKey $NUGET_API_KEY -Verbosity detailed -Source https://www.nuget.org/api/v2/package
