# Simple powershell script to easily build on Windows and verify functionality.

$ErrorActionPreference = "Stop";
$DaemonRuntime = "win-x64";
$UIRuntime = "win10-x64";
$PrevPath = $(Get-Location).Path;
$nl = [Environment]::NewLine;
$OutputDir = "./bin";

$Config = "Release";

$Options = @("--configuration", "$Config", "--verbosity=quiet", "/p:PublishSingleFile=true", "/p:DebugType=embedded",`
    "/p:SuppressNETCoreSdkPreviewMessage=true", "--self-contained=false", "-p:SourceRevisionId=$(git rev-parse --short HEAD)");

# Change dir to script root, in case people run the script outside of the folder.
Set-Location $PSScriptRoot;

# Sanity check
if (!(Test-Path "./OpenTabletDriver")) {
    Write-Error "Could not find OpenTabletDriver folder! Please put this script from the root of the OpenTabletDriver repository.";
    exit 1;
}

Write-Output "Cleaning old build dirs...";
if (Test-Path "./bin") {
    try {
        Get-ChildItem -Path "./bin" | ForEach-Object {
            if ($_.Name -ne "userdata") {
                Remove-Item -Path $_.FullName -Recurse -Force;
            }
        }
    } catch {
        Write-Error "Could not clean old build dirs. Please manually remove contents of ./bin folder.";
        exit 1;
    }
}

dotnet restore --verbosity=quiet;
dotnet clean --configuration $Config --verbosity=quiet;

Write-Output "Building OpenTabletDriver with runtime $NetRuntime...";
New-Item -ItemType Directory -Force -Path "./bin";

Write-Output "${nl}Building Daemon...$nl";
dotnet publish .\OpenTabletDriver.Daemon $Options --output "${OutputDir}" --runtime "${DaemonRuntime}";
if ($LASTEXITCODE -ne 0) { exit 1; }

# Write-Output "${nl}Building Console...$nl";
# dotnet publish .\OpenTabletDriver.Console $Options;
# if ($LASTEXITCODE -ne 0) { exit 2; }

Write-Output "${nl}Building UI...$nl";
dotnet publish .\OpenTabletDriver.UI $Options --output "${OutputDir}" --runtime "${UIRuntime}";
if ($LASTEXITCODE -ne 0) { exit 3; }

Write-Output "${nl}Build finished. Binaries created in ./bin";
Set-Location $PrevPath;
