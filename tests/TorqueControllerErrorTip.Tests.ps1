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

$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
if (-not (Test-Path -LiteralPath $formPath)) {
    throw "Required source file does not exist: $formPath"
}

$form = [System.IO.File]::ReadAllText($formPath)
$communicationFaultText = New-Text @(0x626D,0x529B,0x63A7,0x5236,0x5668,0x901A,0x8BAF,0x5F02,0x5E38)

Assert-Contains $form 'ReportTorqueControllerCommunicationError' 'Form1 must define a dedicated torque controller communication error reporter.'
Assert-Contains $form 'lblStatusErrorTip.ExecuteSafely' 'Torque controller communication errors must update lblStatusErrorTip.'
Assert-Contains $form 'rtbErrorLog.AppendToComponent' 'Torque controller communication errors must be written to the UI exception log.'
Assert-Contains $form 'TORQUE_CONTROLLER_COMMUNICATION_ERROR' 'Torque controller communication errors must be written to structured exception logs.'
Assert-Contains $form $communicationFaultText 'The operator-facing torque controller error message must clearly identify a communication fault.'

Assert-RegexMatch `
    $form `
    '_clientScanAssy\.OnLog\s*\+=\s*\(msg,\s*isErrorLog\)\s*=>[\s\S]*?if\s*\(isErrorLog\)[\s\S]*?ReportTorqueControllerCommunicationError\(ProcessName\.Scan_ASSY,\s*msg\)' `
    'Scan_ASSY torque controller error logs must call ReportTorqueControllerCommunicationError.'

Assert-RegexMatch `
    $form `
    '_clientScrewBa\.OnLog\s*\+=\s*\(msg,\s*isErrorLog\)\s*=>[\s\S]*?if\s*\(isErrorLog\)[\s\S]*?ReportTorqueControllerCommunicationError\(ProcessName\.Screw_BA,\s*msg\)' `
    'Screw_BA torque controller error logs must call ReportTorqueControllerCommunicationError.'

Write-Host 'TorqueControllerErrorTip source checks passed.'
