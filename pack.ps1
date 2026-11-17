param([Parameter(Mandatory = $true)][string]$Version)

$feed = Join-Path $HOME "LocalNuGet"
New-Item -ItemType Directory -Force -Path $feed | Out-Null

if (-not (Test-Path "AuthKit.Tests\AuthKit.Tests.csproj")) {
    Write-Error "AuthKit.Tests project not found. Refusing to pack without tests."
    exit 1
}

dotnet test AuthKit.Tests -c Release
if ($LASTEXITCODE -ne 0) { Write-Error "Tests failed. Package was not created."; exit 1 }

dotnet pack AuthKit\AuthKit.csproj -c Release -o $feed -p:Version=$Version
if ($LASTEXITCODE -ne 0) { Write-Error "Pack failed."; exit 1 }

Write-Host "Created AuthKit $Version in $feed"
