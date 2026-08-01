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

$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
$systemInfoPath = Join-Path $RepoRoot 'MesDatas\DataAccess\SystemInfo.cs'
$addressInfoPath = Join-Path $RepoRoot 'MesDatas\DataAccess\PlcAddressInfo.cs'
$designerPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.Designer.cs'

foreach ($path in @($formPath, $systemInfoPath, $addressInfoPath, $designerPath)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required source file does not exist: $path"
    }
}

$form = [System.IO.File]::ReadAllText($formPath)
$systemInfo = [System.IO.File]::ReadAllText($systemInfoPath)
$addressInfo = [System.IO.File]::ReadAllText($addressInfoPath)
$designer = [System.IO.File]::ReadAllText($designerPath)

$resetAlarmModeText = New-Text @(0x8D85,0x65F6,0x6E05,0x0052,0x0065,0x0071,0x5E76,0x62A5,0x8B66)
$timeoutHandlingLabel = New-Text @(0x0050,0x004C,0x0043,0x63A5,0x6536,0x8D85,0x65F6,0x5904,0x7406)
$clearReqText = New-Text @(0x0052,0x0065,0x0071,0x5DF2,0x6E05,0x96F6)
$ignoreTransferText = New-Text @(0x5DF2,0x5FFD,0x7565,0x672C,0x6B21,0x8F6C,0x53D1)
$keepReqHighText = New-Text @(0x4FDD,0x6301,0x0031)
$ackRecoveredText = New-Text @(0x0050,0x004C,0x0043,0x0020,0x0041,0x0043,0x004B,0x5DF2,0x6536,0x5230)
$allowDenyText = New-Text @(0x0031,0x003D,0x5141,0x8BB8,0x6253,0x87BA,0x9489,0xFF0C,0x0032,0x003D,0x7981,0x6B62,0x6253,0x87BA,0x9489)

Assert-Contains $systemInfo 'TorqueAckTimeoutMode' 'SystemInfo must persist the torque ACK timeout handling mode.'
Assert-Contains $systemInfo $resetAlarmModeText 'Torque ACK timeout mode must default to reset-Req-and-alarm.'

Assert-Contains $designer 'cboTorqueAckTimeoutMode' 'The assembly/torque settings UI must expose the ACK timeout mode combo box.'
Assert-Contains $designer 'labelTorqueAckTimeoutMode' 'The assembly/torque settings UI must label the ACK timeout mode combo box.'
Assert-Contains $designer $timeoutHandlingLabel 'The ACK timeout mode label must explain PLC receive timeout handling.'
Assert-Contains $designer $resetAlarmModeText 'The ACK timeout mode option must match the new reset-Req-and-alarm behavior.'

Assert-Contains $form 'TorqueAckTimeoutModeResetAndAlarm' 'Form1 must define the reset-Req-and-alarm ACK timeout mode.'
Assert-Contains $form 'SemaphoreSlim' 'Torque forwarding must serialize transfers per process.'
Assert-Contains $form '_scanAssyTorqueTransferLock' 'Scan_ASSY must have its own torque transfer lock.'
Assert-Contains $form '_screwBaTorqueTransferLock' 'Screw_BA must have its own torque transfer lock.'
Assert-Contains $form 'WaitForTorqueAckAsync' 'Torque forwarding must use a dedicated ACK wait helper.'
Assert-Contains $form 'WaitForTorqueAckResetAsync' 'Torque forwarding must wait for PLC ACK to return to 0 before releasing the next transfer.'
Assert-Contains $form 'SetTorqueAckWaitingState' 'Torque forwarding must mark the process as waiting for PLC ACK.'
Assert-Contains $form 'GetTorqueAckTimeoutMs' 'Torque forwarding must read the configured ACK timeout.'
Assert-Contains $form 'Req=' 'Timeout logs must include the request address.'
Assert-Contains $form $clearReqText 'Timeout logs must state that Req is cleared after ACK timeout.'
Assert-Contains $form $ignoreTransferText 'Timeout logs must state that the timed-out transfer is ignored.'
Assert-Contains $form $ackRecoveredText 'Recovery logs must state that PLC ACK was received.'
Assert-Contains $form 'AppendLog(context.ProcessName, $"[转发请求] 扭力={val}，结果={resultText}，Req={context.RequestAddress}=1，等待Ack={context.AckAddress}");' 'Normal forwarding requests must use the concise configured-address format.'
Assert-Contains $form 'AppendLog(context.ProcessName, $"PLC ACK已收到，{context.AckAddress}={ackResult.LastAckValue}，等待耗时={ackResult.ElapsedMs}ms");' 'Normal ACK logs must use the concise configured-address format.'
Assert-Contains $form 'AppendLog(context.ProcessName, $"[转发成功] {context.RequestAddress}已复位0，{context.AckAddress}已回0，恢复扭力转发");' 'Normal forwarding completion logs must use the concise reset format.'
Assert-NotContains $form '[转发请求] TransferId=' 'Normal forwarding requests must not include TransferId.'
Assert-NotContains $form 'PLC ACK已收到，TransferId=' 'Normal ACK logs must not include TransferId.'
Assert-NotContains $form '[转发成功] TransferId=' 'Normal forwarding completion logs must not include TransferId.'

Assert-NotContains $form 'while (!isSuccess)' 'Torque forwarding must not use an infinite retry loop.'
Assert-NotContains $form $keepReqHighText 'Torque timeout must not keep Req high after the configured timeout.'
Assert-NotContains $form '重试前复位Req=0' 'Torque timeout must not reset Req before PLC ACK arrives.'
Assert-NotContains $form '程序挂起并持续重试' 'Torque timeout must not continue automatic retry after ACK timeout.'

Assert-Contains $addressInfo $allowDenyText 'Torque interlock comments must match field protocol: 1=allow, 2=deny.'
Assert-NotContains $addressInfo '0=禁止打螺钉' 'Torque interlock comments must not claim 0 means deny.'

Write-Host 'Torque forwarding source checks passed.'
