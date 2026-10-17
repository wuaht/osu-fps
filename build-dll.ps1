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

# BuildAgainstClient=false, as this repository may be checked out next to an osu! client source tree.
dotnet build $project -c Release -p:BuildAgainstClient=false -nologo
if ($LASTEXITCODE -ne 0) { throw "Build failed (exit code $LASTEXITCODE)." }

$dll = Get-ChildItem (Join-Path $project 'bin/Release') -Recurse -Filter 'osu.Game.Rulesets.Fposu.dll' |
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
