#Requires -Version 7.2
<#
.SYNOPSIS
  Activate or return a Unity Personal license in GitHub Actions.
.DESCRIPTION
  Uses Unity.Licensing.Client --activate-all --include-personal, matching
  the activation tested on GitHub-hosted Ubuntu runners.

  Required environment variables:
    UNITY_EMAIL, UNITY_PASSWORD

  Optional environment variable:
    UNITY_LICENSING_CLIENT (absolute path to Unity.Licensing.Client[.exe])

  The activation marker is stored under RUNNER_TEMP so the cleanup step can
  attempt to return the license even if activation or compilation fails.

  Use on trusted workflow runs only. The Licensing Client accepts credentials
  as command-line arguments; never enable shell debug tracing or share raw
  process lists/logs containing these arguments.
.EXAMPLE
  pwsh -File .github/scripts/unity-license.ps1 -Action activate
.EXAMPLE
  pwsh -File .github/scripts/unity-license.ps1 -Action return
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('activate', 'return')]
    [string] $Action
)

$ErrorActionPreference = 'Stop'
Set-PSDebug -Off

$workingDirectory = if ($env:RUNNER_TEMP) { $env:RUNNER_TEMP } else { [IO.Path]::GetTempPath() }
$marker = Join-Path $workingDirectory 'unity-personal-license-attempted.marker'

function Get-LicensingClient {
    if ($env:UNITY_LICENSING_CLIENT) {
        if (-not (Test-Path -LiteralPath $env:UNITY_LICENSING_CLIENT -PathType Leaf)) {
            throw 'UNITY_LICENSING_CLIENT does not point to a file.'
        }
        return $env:UNITY_LICENSING_CLIENT
    }

    $filename = if ($IsWindows) { 'Unity.Licensing.Client.exe' } else { 'Unity.Licensing.Client' }
    $candidateDirs = [System.Collections.Generic.List[string]]::new()
    $searchRoots = [System.Collections.Generic.List[string]]::new()

    # Unity CLI-installed client: this is the path proven to work on Linux.
    $candidateDirs.Add((Join-Path $HOME '.config/unityhub/external-modules/licensingClient'))

    if ($IsWindows) {
        foreach ($base in @($env:APPDATA, $env:LOCALAPPDATA)) {
            if ($base) {
                $candidateDirs.Add((Join-Path $base 'UnityHub/external-modules/licensingClient'))
                $candidateDirs.Add((Join-Path $base 'unityhub/external-modules/licensingClient'))
            }
        }
        foreach ($base in @($env:PROGRAMFILES, ${env:ProgramFiles(x86)}, $HOME)) {
            if ($base) {
                $searchRoots.Add((Join-Path $base 'Unity/Hub/Editor'))
            }
        }
    }
    elseif ($IsMacOS) {
        $candidateDirs.Add((Join-Path $HOME 'Library/Application Support/UnityHub/external-modules/licensingClient'))
        $candidateDirs.Add((Join-Path $HOME 'Library/Application Support/unityhub/external-modules/licensingClient'))
        $searchRoots.Add('/Applications/Unity/Hub/Editor')
        $searchRoots.Add((Join-Path $HOME 'Unity/Hub/Editor'))
        $searchRoots.Add((Join-Path $HOME 'Library/Unity/Hub/Editor'))
    }
    else {
        $searchRoots.Add((Join-Path $HOME 'Unity/Hub/Editor'))
        $searchRoots.Add('/opt/unity/Editor')
    }

    foreach ($dir in $candidateDirs) {
        $candidate = Join-Path $dir $filename
        if (Test-Path -LiteralPath $candidate -PathType Leaf) {
            return $candidate
        }
    }

    # Fallback for installations where the Licensing Client lives inside
    # the Unity Editor rather than Unity CLI's external modules directory.
    # Restrict recursive scanning to Unity Editor installation roots.
    foreach ($root in $searchRoots) {
        if (-not (Test-Path -LiteralPath $root -PathType Container)) {
            continue
        }
        $result = Get-ChildItem -LiteralPath $root -File -Recurse -Filter $filename `
            -ErrorAction SilentlyContinue | Select-Object -First 1
        if ($result) {
            return $result.FullName
        }
    }

    throw 'Unity.Licensing.Client not found. Run "unity license status" to initialize it, or provide UNITY_LICENSING_CLIENT.'
}

function Assert-Credentials {
    if ([string]::IsNullOrWhiteSpace($env:UNITY_EMAIL) -or
        [string]::IsNullOrWhiteSpace($env:UNITY_PASSWORD)) {
        throw 'Missing UNITY_EMAIL or UNITY_PASSWORD GitHub Actions secret.'
    }
}

function Read-LicenseStatus {
    # Unity CLI returns exit code 4 if no license is present; capture output
    # and return the exit code without suppressing actual licensing failures.
    $output = & unity license status 2>&1
    $code = $LASTEXITCODE
    return [pscustomobject]@{
        Code = $code
        Text = ($output -join "`n")
    }
}

if ($Action -eq 'return' -and -not (Test-Path -LiteralPath $marker)) {
    Write-Host '[Unity CI] No activation attempted during this job; nothing to return.'
    exit 0
}

Assert-Credentials
$client = Get-LicensingClient

if ($Action -eq 'activate') {
    # Mark BEFORE invoking the server so the `if: always()` cleanup can run
    # even if activation succeeds remotely but the process exits unexpectedly.
    New-Item -Path $marker -ItemType File -Force | Out-Null
    Write-Host '[Unity CI] Activating Unity Personal license...'

    & $client `
        --activate-all `
        --include-personal `
        --username $env:UNITY_EMAIL `
        --password $env:UNITY_PASSWORD

    if ($LASTEXITCODE -ne 0) {
        throw 'Licensing Client returned an error during activation.'
    }

    $status = Read-LicenseStatus
    if ($status.Code -ne 0 -or
        $status.Text -notmatch '(?i)Unity Personal\s*\(Assigned\)') {
        throw 'Unity CLI did not confirm an active Unity Personal (Assigned) license.'
    }

    Write-Host '[Unity CI] Unity Personal (Assigned) license is active.'
    exit 0
}

Write-Host '[Unity CI] Returning Unity Personal license...'

& $client `
    --deactivate-all `
    --username $env:UNITY_EMAIL `
    --password $env:UNITY_PASSWORD

if ($LASTEXITCODE -ne 0) {
    throw 'Licensing Client returned an error during deactivation; the seat may still be assigned.'
}

$status = Read-LicenseStatus
if ($status.Code -ne 4 -or
    $status.Text -notmatch '(?i)License:\s*none active') {
    throw 'Unity CLI did not confirm that the license is inactive after deactivation.'
}

Remove-Item -LiteralPath $marker -Force
Write-Host '[Unity CI] License returned successfully.'
