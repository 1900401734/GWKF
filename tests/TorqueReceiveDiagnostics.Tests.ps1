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

function Assert-TextAppearsBefore {
    param(
        [string]$Content,
        [string]$AnchorText,
        [string]$FirstText,
        [string]$SecondText,
        [string]$Message
    )

    $anchorIndex = $Content.IndexOf($AnchorText)
    $firstIndex = if ($anchorIndex -ge 0) { $Content.IndexOf($FirstText, $anchorIndex) } else { -1 }
    $secondIndex = if ($anchorIndex -ge 0) { $Content.IndexOf($SecondText, $anchorIndex) } else { -1 }

    if ($anchorIndex -lt 0 -or $firstIndex -lt 0 -or $secondIndex -lt 0 -or $firstIndex -gt $secondIndex) {
        throw $Message
    }
}

$clientPath = Join-Path $RepoRoot 'MesDatas\Services\TorqueControllerClient.cs'
$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'

foreach ($path in @($clientPath, $formPath)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required source file does not exist: $path"
    }
}

$client = [System.IO.File]::ReadAllText($clientPath)
$form = [System.IO.File]::ReadAllText($formPath)

$rawTorquePacketText = New-Text @(0x6536,0x5230,0x626D,0x529B,0x539F,0x59CB,0x62A5,0x6587)
$parseSuccessText = New-Text @(0x626D,0x529B,0x62A5,0x6587,0x89E3,0x6790,0x6210,0x529F)
$noTorqueSummaryText = New-Text @(0x5DF2,0x8FDE,0x63A5,0x4F46,0x672A,0x6536,0x5230,0x0020,0x004D,0x0049,0x0044,0x0020,0x0030,0x0030,0x0036,0x0031)
$shortPacketText = New-Text @(0x6536,0x5230,0x77ED,0x62A5,0x6587)
$unknownMidText = New-Text @(0x6536,0x5230,0x672A,0x5904,0x7406,0x0020,0x004D,0x0049,0x0044)
$forwardPlcText = New-Text @(0x7A0B,0x5E8F,0x5DF2,0x6536,0x5230,0x626D,0x529B,0x6570,0x636E,0xFF0C,0x51C6,0x5907,0x8F6C,0x53D1,0x0050,0x004C,0x0043)
$heartbeatText = New-Text @(0x6536,0x5230,0x5FC3,0x8DF3)

Assert-Contains $client 'TorqueNoDataSummaryIntervalSeconds = 30' 'Torque diagnostics must use a 30 second no-data summary interval.'
Assert-Contains $client '_lastTorqueDataReceiveTime' 'Torque diagnostics must track the last MID 0061 receive time.'
Assert-Contains $client '_lastNoTorqueDataSummaryTime' 'Torque diagnostics must throttle no-data summaries.'
Assert-Contains $client 'ReportNoTorqueDataIfNeeded' 'Torque diagnostics must report connected-but-no-torque-data summaries.'
Assert-Contains $client 'BuildPacketSummary' 'Torque diagnostics must log a bounded raw packet summary.'

Assert-Contains $client $shortPacketText 'Torque diagnostics must log short packets.'
Assert-Contains $client $unknownMidText 'Torque diagnostics must log unknown MID packets.'
Assert-Contains $client $rawTorquePacketText 'Torque diagnostics must log raw MID 0061 torque packets.'
Assert-Contains $client $parseSuccessText 'Torque diagnostics must log successful torque packet parsing.'
Assert-Contains $client $noTorqueSummaryText 'Torque diagnostics must log connected-but-no-MID-0061 summaries.'

Assert-TextAppearsBefore `
    $client `
    'case "0061":' `
    $rawTorquePacketText `
    'ParseTorqueData(msg)' `
    'MID 0061 must be logged before parsing.'

Assert-RegexMatch `
    $client `
    'case\s+"9999":[\s\S]*?break;' `
    'Heartbeat MID 9999 must remain handled without falling into unknown MID logging.'

Assert-NotContains $client ('OnLog?.Invoke("' + $heartbeatText) 'Heartbeat MID 9999 must not be logged for every packet.'
Assert-NotContains $client ('OnLog?.Invoke($"' + $heartbeatText) 'Heartbeat MID 9999 must not be logged for every packet.'

Assert-RegexMatch `
    $form `
    'OnTorqueDataReceived\s*\+=\s*\(data\)\s*=>[\s\S]*?AppendLog\(ProcessName\.Scan_ASSY,[\s\S]*?Task\.Run\(async\s*\(\)\s*=>\s*await ForwardTorqueToPlcAsync\(ProcessName\.Scan_ASSY,\s*data\)\)' `
    'Scan_ASSY must log received torque data before forwarding to PLC.'

Assert-RegexMatch `
    $form `
    'OnTorqueDataReceived\s*\+=\s*\(data\)\s*=>[\s\S]*?AppendLog\(ProcessName\.Screw_BA,[\s\S]*?Task\.Run\(async\s*\(\)\s*=>\s*await ForwardTorqueToPlcAsync\(ProcessName\.Screw_BA,\s*data\)\)' `
    'Screw_BA must log received torque data before forwarding to PLC.'

Assert-Contains $form $forwardPlcText 'Form1 must log that parsed torque data is ready to forward to PLC.'

Write-Host 'TorqueReceiveDiagnostics source checks passed.'
