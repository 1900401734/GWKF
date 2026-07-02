param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Text,
        [string]$Message
    )

    if (-not $Content.Contains($Text)) {
        throw $Message
    }
}

function Assert-NotContains {
    param(
        [string]$Content,
        [string]$Text,
        [string]$Message
    )

    if ($Content.Contains($Text)) {
        throw $Message
    }
}

function Assert-RegexMatch {
    param(
        [string]$Content,
        [string]$Pattern,
        [string]$Message
    )

    if (-not [regex]::IsMatch($Content, $Pattern)) {
        throw $Message
    }
}

function New-Text {
    param([int[]]$Codes)
    return [string]::Concat(($Codes | ForEach-Object { [char]$_ }))
}

$clientPath = Join-Path $RepoRoot 'MesDatas\Services\TorqueControllerClient.cs'
if (-not (Test-Path -LiteralPath $clientPath)) {
    throw "Required source file does not exist: $clientPath"
}

$client = [System.IO.File]::ReadAllText($clientPath)
$nextReconnectText = New-Text @(0x4E0B,0x6B21,0x91CD,0x8FDE)

Assert-Contains $client 'InitialReconnectDelayMs = 1000' 'Torque controller reconnect backoff must start at 1 second.'
Assert-Contains $client 'MaxReconnectDelayMs = 10000' 'Torque controller reconnect backoff must be capped at 10 seconds.'
Assert-Contains $client 'Math.Min(MaxReconnectDelayMs' 'Torque controller reconnect backoff must cap the delay.'
Assert-Contains $client 'reconnectDelayMs * 2' 'Torque controller reconnect backoff must double after each failed connection attempt.'
Assert-Contains $client 'Task.Delay(reconnectDelayMs, token)' 'Torque controller reconnect loop must wait using the current backoff delay.'
Assert-Contains $client $nextReconnectText 'Reconnect logs must show the next retry delay for field diagnosis.'

Assert-RegexMatch `
    $client `
    'while\s*\(\s*!token\.IsCancellationRequested\s*&&\s*_isStarted\s*\)' `
    'Torque controller reconnect loop must continue until stopped or cancelled.'

Assert-NotContains $client 'FailCountMax' 'Torque controller reconnect loop must not use a fixed max failure count.'
Assert-NotContains $client 'failCount <= ' 'Torque controller reconnect loop must not stop after a fixed number of failures.'
Assert-NotContains $client 'Task.Delay(3000, token)' 'Torque controller reconnect loop must not use a fixed 3 second retry delay.'

Write-Host 'Torque controller reconnect backoff source checks passed.'
