# Builds the osu!fps ruleset DLL for official osu!lazer, like the CI does (against the official ppy.osu.Game packages).
#
# Usage:
#   ./build-dll.ps1            # build bin/osu.Game.Rulesets.Fposu.dll
#   ./build-dll.ps1 -Install   # also copy it into the rulesets folder of official osu!lazer (%AppData%\osu\rulesets)

param(
    [switch]$Install
)

$ErrorActionPreference = 'Stop'

$root = $PSScriptRoot
$project = Join-Path $root 'osu.Game.Rulesets.Fposu'
$outputDir = Join-Path $root 'bin'
# separate from the project's own bin / obj, which are used by the client build when this repository is a submodule of it.
$buildOutputDir = Join-Path $root 'bin/official/'
$intermediateDir = Join-Path $root 'obj/official/'

# BuildAgainstClient=false, as this repository may be checked out next to an osu! client source tree.
dotnet build $project -c Release -nologo -p:BuildAgainstClient=false "-p:BaseOutputPath=$buildOutputDir" "-p:BaseIntermediateOutputPath=$intermediateDir"
if ($LASTEXITCODE -ne 0) { throw "Build failed (exit code $LASTEXITCODE)." }

$dll = Get-ChildItem (Join-Path $buildOutputDir 'Release') -Recurse -Filter 'osu.Game.Rulesets.Fposu.dll' |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1
if (-not $dll) { throw 'The built DLL could not be found.' }

New-Item -ItemType Directory -Force $outputDir | Out-Null
Copy-Item $dll.FullName $outputDir -Force
Write-Host "Built $(Join-Path $outputDir $dll.Name)" -ForegroundColor Green

if ($Install)
{
    $rulesetsDir = Join-Path $env:APPDATA 'osu/rulesets'
    New-Item -ItemType Directory -Force $rulesetsDir | Out-Null
    Copy-Item $dll.FullName $rulesetsDir -Force
    Write-Host "Installed to $rulesetsDir (restart osu!lazer to load it)" -ForegroundColor Green
}
