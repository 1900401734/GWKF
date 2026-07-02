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
$systemInfoServerPath = Join-Path $RepoRoot 'MesDatas\DataAccess\SystemInfoServer.cs'
$designerPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.Designer.cs'

foreach ($path in @($formPath, $systemInfoPath, $systemInfoServerPath, $designerPath)) {
    if (-not (Test-Path -LiteralPath $path)) {
        throw "Required source file does not exist: $path"
    }
}

$form = [System.IO.File]::ReadAllText($formPath)
$systemInfo = [System.IO.File]::ReadAllText($systemInfoPath)
$systemInfoServer = [System.IO.File]::ReadAllText($systemInfoServerPath)
$designer = [System.IO.File]::ReadAllText($designerPath)

$ackTimeoutText = New-Text @(0x626D,0x529B,0x0041,0x0043,0x004B,0x8D85,0x65F6)
$resetAlarmModeText = New-Text @(0x8D85,0x65F6,0x6E05,0x0052,0x0065,0x0071,0x5E76,0x62A5,0x8B66)
$clearReqText = New-Text @(0x0052,0x0065,0x0071,0x5DF2,0x6E05,0x96F6)
$ignoreTransferText = New-Text @(0x5DF2,0x5FFD,0x7565,0x672C,0x6B21,0x8F6C,0x53D1)
$resetFailReasonText = New-Text @(0x626D,0x529B,0x8F6C,0x53D1,0x0041,0x0043,0x004B,0x8D85,0x65F6,0x6E05,0x96F6,0x0052,0x0065,0x0071)

Assert-Contains $systemInfo $resetAlarmModeText 'SystemInfo must default the torque ACK timeout mode to reset-Req-and-alarm.'
Assert-Contains $systemInfo 'TorqueAckTimeoutSeconds' 'SystemInfo must persist the configurable torque ACK timeout seconds.'
Assert-Contains $systemInfo 'public string TorqueAckTimeoutSeconds { get; set; } = "3";' 'Torque ACK timeout must default to the previous 3 second behavior.'

Assert-Contains $systemInfoServer 'EnsureTorqueAckTimeoutSecondsColumn' 'SystemInfoServer must add the torque ACK timeout column for old Access databases.'
Assert-Contains $systemInfoServer 'TorqueAckTimeoutSecondsColumnName' 'SystemInfoServer must name the torque ACK timeout column explicitly.'

Assert-Contains $designer 'txtTorqueAckTimeoutSeconds' 'Production config UI must expose torque ACK timeout seconds textbox.'
Assert-Contains $designer 'labelTorqueAckTimeoutSeconds' 'Production config UI must label torque ACK timeout seconds.'
Assert-Contains $designer $ackTimeoutText 'The torque ACK timeout label must be clear for operators.'
Assert-Contains $designer $resetAlarmModeText 'The ACK timeout mode UI must show reset-Req-and-alarm behavior.'

Assert-Contains $form 'TorqueAckTimeoutModeResetAndAlarm' 'Form1 must define the reset-Req-and-alarm ACK timeout mode.'
Assert-Contains $form 'DefaultTorqueAckTimeoutSeconds' 'Form1 must define a default torque ACK timeout.'
Assert-Contains $form 'MinTorqueAckTimeoutSeconds' 'Form1 must define a minimum torque ACK timeout.'
Assert-Contains $form 'MaxTorqueAckTimeoutSeconds' 'Form1 must define a maximum torque ACK timeout.'
Assert-Contains $form 'NormalizeTorqueAckTimeoutSeconds' 'Form1 must validate the configured torque ACK timeout.'
Assert-Contains $form 'GetTorqueAckTimeoutMs' 'Torque forwarding must read the configured ACK timeout in milliseconds.'
Assert-Contains $form 'txtTorqueAckTimeoutSeconds.Text = systemInfo.TorqueAckTimeoutSeconds' 'Loading product config must populate the torque ACK timeout textbox.'
Assert-Contains $form 'systemInfo.TorqueAckTimeoutSeconds = NormalizeTorqueAckTimeoutSeconds' 'Saving product config must persist the normalized torque ACK timeout.'

Assert-Contains $form 'HandleTorqueAckTimeoutAndResetRequest' 'Torque forwarding must handle ACK timeout by reporting and clearing Req.'
Assert-Contains $form 'TORQUE_FORWARD_ACK_TIMEOUT' 'Torque ACK timeout must be written to structured exception logs.'
Assert-Contains $form 'lblStatusErrorTip.ExecuteSafely' 'Torque ACK timeout must update lblStatusErrorTip.'
Assert-Contains $form 'rtbErrorLog.AppendToComponent' 'Torque ACK timeout must be written to the exception detail UI.'
Assert-Contains $form 'TryWriteInt16(context.RequestAddress, 0' 'Torque ACK timeout must clear Req directly.'
Assert-Contains $form $resetFailReasonText 'Torque ACK timeout Req reset must have a clear failure reason.'
Assert-Contains $form $clearReqText 'Timeout logs must state that Req was cleared.'
Assert-Contains $form $ignoreTransferText 'Timeout logs must state that the current transfer was ignored.'

Assert-NotContains $form 'Req={5}保持1' 'ACK timeout must not keep Req high after the configured timeout.'
Assert-NotContains $form '继续等待PLC ACK' 'ACK timeout must not keep waiting for PLC ACK after the configured timeout.'

Write-Host 'Torque forward timeout source checks passed.'
