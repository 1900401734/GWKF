param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

$form1Path = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
if (-not (Test-Path -LiteralPath $form1Path)) {
    throw "File does not exist: $form1Path"
}

$content = [System.IO.File]::ReadAllText($form1Path)

function Assert-Contains {
    param(
        [string]$Text,
        [string]$Message
    )

    if (-not $content.Contains($Text)) {
        throw $Message
    }
}

Assert-Contains 'TorqueInterlockWriteIntervalMs = 1000' 'Interlock writes must keep a one-second target interval.'
Assert-Contains 'TorqueInterlockRetryDelayMs = 100' 'Fast failures must use a short retry delay.'
Assert-Contains 'TorqueInterlockAlarmThresholdMs = 10000' 'The interlock alarm threshold must be ten seconds.'
Assert-Contains 'await Task.WhenAll(' 'Process 1 and process 3 interlock loops must run independently.'
Assert-Contains '() => _clientScanAssy' 'Process 1 must read its current controller state on every write.'
Assert-Contains '() => _clientScrewBa' 'Process 3 must read its current controller state on every write.'
Assert-Contains 'short targetValue = client.IsConnected ? (short)1 : (short)2' 'Interlock values must remain 1=allow and 2=deny.'
Assert-Contains 'continuousFailure = Stopwatch.StartNew()' 'Continuous failure timing must start on the first failed write.'
Assert-Contains 'continuousFailure.ElapsedMilliseconds >= TorqueInterlockAlarmThresholdMs' 'Alarm evaluation must use elapsed failure time.'
Assert-Contains 'alarmReported = true' 'A continuous failure episode must latch its alarm.'
Assert-Contains 'continuousFailure = null' 'A successful write must reset continuous failure timing.'
Assert-Contains 'alarmReported = false' 'A successful write must allow a later failure episode to alarm again.'
Assert-Contains '连续10秒写入失败' 'The operator alarm must describe the ten-second continuous failure.'

$monitorStart = $content.IndexOf('private async Task MonitorTorqueInterlockAsync(', [StringComparison]::Ordinal)
$monitorEnd = $content.IndexOf('/// <summary>', $monitorStart + 1, [StringComparison]::Ordinal)
if ($monitorStart -lt 0 -or $monitorEnd -lt 0) {
    throw 'Unable to locate the interlock monitor method.'
}

$monitorMethod = $content.Substring($monitorStart, $monitorEnd - $monitorStart)
if ($monitorMethod.Contains('Task.WhenAny')) {
    throw 'The interlock monitor must not abandon an unfinished PLC write task.'
}

$monitorCallCount = ([regex]::Matches($content, 'MonitorTorqueInterlockAsync\(')).Count
if ($monitorCallCount -ne 3) {
    throw "Expected two independent monitor calls plus one method declaration, found $monitorCallCount occurrences."
}

Write-Host 'Torque interlock write tolerance source checks passed.'
