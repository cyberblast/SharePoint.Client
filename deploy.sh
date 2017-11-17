#!/usr/bin/env bash

mono nuget.exe pack cyberblast.SharePoint.Client.nuspec && \
mono nuget.exe push cyberblast.SharePoint.Client.nupkg -ApiKey $NUGET_API_KEY -Verbosity detailed -Source https://www.nuget.org/api/v2/package
