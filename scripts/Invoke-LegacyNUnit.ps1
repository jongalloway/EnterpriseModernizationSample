[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$AssemblyPath,

    [string]$AdapterVersion = "4.5.0"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$resolvedAssemblyPath = (Resolve-Path -Path $AssemblyPath).Path

$candidateAdapterPaths = @(
    (Join-Path $repoRoot "src\before\packages\NUnit3TestAdapter.$AdapterVersion\build\net462"),
    (Join-Path $env:USERPROFILE ".nuget\packages\nunit3testadapter\$AdapterVersion\build\net462")
)

$adapterPath = $candidateAdapterPaths | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $adapterPath) {
    throw "NUnit3TestAdapter $AdapterVersion was not restored. Run 'nuget restore src\before\Fabrikam.EnterprisePizza.Legacy.sln' first."
}

Write-Host "Running NUnit tests from $resolvedAssemblyPath"
Write-Host "Using adapter from $adapterPath"

dotnet vstest $resolvedAssemblyPath "--TestAdapterPath:$adapterPath"
exit $LASTEXITCODE
