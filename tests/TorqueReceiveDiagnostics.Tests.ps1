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

$rawTorquePacketText = '收到扭力原始报文'
$parseSuccessText = '扭力报文解析成功'
$noTorqueSummaryText = '已连接但未收到 MID 0061 扭力数据'
$shortPacketText = '收到短报文'
$unknownMidText = '收到未处理 MID'
$heartbeatText = '收到心跳'

Assert-NotContains $client 'TorqueNoDataSummaryIntervalSeconds' 'Torque diagnostics must not schedule idle no-data summaries.'
Assert-NotContains $client '_lastTorqueDataReceiveTime' 'Torque diagnostics must not track idle time since the last MID 0061.'
Assert-NotContains $client '_lastNoTorqueDataSummaryTime' 'Torque diagnostics must not retain idle-summary throttle state.'
Assert-NotContains $client 'ReportNoTorqueDataIfNeeded' 'Connected idle time must not generate recurring torque logs.'
Assert-NotContains $client $noTorqueSummaryText 'Connected idle time must not write no-MID-0061 log lines.'
Assert-Contains $client 'BuildPacketSummary' 'Torque diagnostics must retain bounded packet summaries for abnormal packets.'

Assert-Contains $client $shortPacketText 'Torque diagnostics must retain short-packet diagnostics.'
Assert-Contains $client $unknownMidText 'Torque diagnostics must retain unknown-MID diagnostics.'
Assert-Contains $client 'OnLog?.Invoke("收到扭力原始报文", false);' 'MID 0061 must use the concise raw-packet log line.'
Assert-Contains $client '扭力报文解析成功，扭力={data.Torque}，下限={data.TorqueMin}，上限={data.TorqueMax}，结果=' 'Parsed torque logs must use the requested Chinese field names.'
Assert-NotContains $client '收到扭力原始报文 MID 0061' 'Normal MID 0061 logs must not include protocol metadata.'
Assert-NotContains $client 'Torque={data.Torque}' 'Normal parsed logs must not use legacy English field names.'
Assert-NotContains $client 'Time={data.TimeStamp}' 'Normal parsed logs must not include the controller timestamp.'

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
    'OnTorqueDataReceived\s*\+=\s*\(data\)\s*=>[\s\S]*?Task\.Run\(async\s*\(\)\s*=>\s*await ForwardTorqueToPlcAsync\(ProcessName\.Scan_ASSY,\s*data\)\)' `
    'Scan_ASSY must forward parsed torque data directly to PLC.'

Assert-RegexMatch `
    $form `
    'OnTorqueDataReceived\s*\+=\s*\(data\)\s*=>[\s\S]*?Task\.Run\(async\s*\(\)\s*=>\s*await ForwardTorqueToPlcAsync\(ProcessName\.Screw_BA,\s*data\)\)' `
    'Screw_BA must forward parsed torque data directly to PLC.'

Assert-NotContains $form 'BuildTorqueForwardReadyMessage' 'Form1 must not emit the duplicate ready-to-forward log node.'
Assert-NotContains $form '程序已收到扭力数据，准备转发PLC' 'The normal torque flow must not contain the legacy duplicate forwarding line.'
Write-Host 'TorqueReceiveDiagnostics source checks passed.'
