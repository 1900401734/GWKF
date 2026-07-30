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

function New-Text {
    param([int[]]$Codes)
    return [string]::Concat(($Codes | ForEach-Object { [char]$_ }))
}

$managerPath = Join-Path $RepoRoot 'MesDatas\Services\PlcConnectionManager.cs'
$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'

if (-not (Test-Path -LiteralPath $managerPath)) {
    throw 'PlcConnectionManager.cs does not exist.'
}
if (-not (Test-Path -LiteralPath $formPath)) {
    throw 'Form1.cs does not exist.'
}

$manager = [System.IO.File]::ReadAllText($managerPath)
$form = [System.IO.File]::ReadAllText($formPath)
$heartbeatTimeoutText = New-Text @(0x0050,0x004C,0x0043,0x5FC3,0x8DF3,0x8D85,0x8FC7,0x0031,0x0030,0x79D2)
$badHeartbeatDisconnectCall = 'UpdateConnectionStatus(false, "' + $heartbeatTimeoutText

Assert-NotContains $manager '_addressInfo.PcHeartBeat' 'PLC manager must not write or read the PC heartbeat address.'
Assert-NotContains $manager 'failCountMax' 'PLC heartbeat timeout must use elapsed time, not fixed failure counts.'
Assert-NotContains $manager 'lastWriteValue' 'PLC manager must not maintain PC heartbeat write values.'
Assert-NotContains $manager $badHeartbeatDisconnectCall 'PLC heartbeat timeout must not mark PLC communication disconnected.'
Assert-Contains $manager 'private const int MonitorIntervalMs = 500;' 'PLC monitor interval must stay at 500ms.'
Assert-Contains $manager 'private const int PlcHeartbeatTimeoutMs = 10000;' 'PLC heartbeat timeout must be 10 seconds.'
Assert-Contains $manager 'private const int HeartbeatReadTimeoutMs = 1000;' 'PLC heartbeat read timeout must be 1 second.'
Assert-Contains $manager 'OnHeartbeatStatusChanged' 'PLC manager must expose a separate heartbeat status event.'
Assert-Contains $manager 'TryReadPlcHeartbeatAsync' 'PLC manager must read PLC heartbeat through a dedicated helper.'
Assert-Contains $manager 'RefreshHeartbeatWatchdog' 'PLC manager must refresh the watchdog only when PLC heartbeat changes.'
Assert-Contains $manager 'BuildHeartbeatStatusMessage' 'PLC manager must include heartbeat diagnostic details in the status message.'
Assert-Contains $manager 'UpdateHeartbeatStatus(false' 'PLC manager must report heartbeat timeout through the heartbeat event.'
Assert-Contains $manager 'UpdateHeartbeatStatus(true' 'PLC manager must report heartbeat recovery through the heartbeat event.'
Assert-Contains $manager 'UpdateConnectionStatus(true);' 'PLC manager must update IsConnected when a connection attempt succeeds.'

Assert-NotContains $form 'PLC_HEARTBEAT_STATUS_CHANGED' 'Form1 must not write PLC heartbeat status to exception logs.'
Assert-NotContains $form 'rtbErrorLog.AppendToComponent(heartbeatMsg)' 'Form1 must not display PLC heartbeat status in the exception detail UI.'

Write-Host 'PLC heartbeat source checks passed.'
