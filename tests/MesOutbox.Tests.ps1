param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

function Assert-FileContains {
    param(
        [string]$Path,
        [string]$Text,
        [string]$Message
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "文件不存在: $Path"
    }

    $content = [System.IO.File]::ReadAllText($Path)
    if (-not $content.Contains($Text)) {
        throw $Message
    }
}

$recordPath = Join-Path $RepoRoot 'MesDatas\Models\MesOutboxRecord.cs'
$storePath = Join-Path $RepoRoot 'MesDatas\Services\MesOutboxStore.cs'
$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
$httpPath = Join-Path $RepoRoot 'MesDatas\Utility\HttpClientUtil.cs'
$projectPath = Join-Path $RepoRoot 'MesDatas\MesDatas.csproj'

$queuePathText = [string]::Concat(
    [char]0x0044, [char]0x003A, [char]0x005C,
    [char]0x004B, [char]0x0061, [char]0x0069, [char]0x0046, [char]0x0061,
    [char]0x004C, [char]0x006F, [char]0x0067, [char]0x0073, [char]0x005C,
    [char]0x004D, [char]0x0045, [char]0x0053,
    [char]0x8865, [char]0x4F20,
    [char]0x961F, [char]0x5217
)

Assert-FileContains $recordPath 'enum MesOutboxStatus' 'MesOutboxRecord must define MesOutboxStatus.'
foreach ($status in @('PendingRetry', 'ConfirmedPass', 'ConfirmedFail', 'ManualProcessing', 'OfflineBypass')) {
    Assert-FileContains $recordPath $status "MesOutboxStatus must include $status."
}

Assert-FileContains $recordPath 'Created' 'MesOutboxStatus must include Created so new records are not immediately retryable.'

foreach ($field in @('TraceId', 'ProcessName', 'Barcode', 'PayloadJson', 'Status', 'ErrorType', 'ErrorMessage', 'RetryCount')) {
    Assert-FileContains $recordPath $field "MesOutboxRecord must include $field."
}

Assert-FileContains $storePath $queuePathText 'MesOutboxStore must persist queue files under D:\KaiFaLogs\MES补传队列.'
foreach ($method in @('Save', 'LoadPendingRetry', 'MarkConfirmedPass', 'MarkConfirmedFail', 'MarkPendingRetry', 'MarkManualProcessing')) {
    Assert-FileContains $storePath $method "MesOutboxStore must expose $method."
}

foreach ($symbol in @('CreateMesOutboxRecord', 'StartMesOutboxRetryTask', 'RetryPendingMesOutboxRecords', 'MarkOutboxConfirmedPass', 'MarkOutboxPendingRetry', 'MarkOutboxManualProcessing')) {
    Assert-FileContains $formPath $symbol "Form1 must wire MES outbox symbol $symbol."
}

Assert-FileContains $formPath 'MesOutboxStatus status = MesOutboxStatus.Created' 'Upload-after-feedback outbox records must start as Created, not PendingRetry.'
Assert-FileContains $formPath 'record.Status = MesOutboxStatus.Created;' 'Saving the initial payload must keep the record out of retry scanning.'
Assert-FileContains $formPath 'MarkOutboxPendingRetry' 'Only explicit failure paths should move an outbox record to PendingRetry.'
Assert-FileContains $storePath 'item.Status == MesOutboxStatus.PendingRetry' 'Retry loop must only load records explicitly marked PendingRetry.'

Assert-FileContains $formPath 'CanPrintAfterWeightMesPass' 'Print flow must gate GETPRINTDATA on Weight MES PASS.'
Assert-FileContains $formPath 'WeightMesPassConfirmed' 'Form1 must track Weight MES PASS state for printing.'
Assert-FileContains $httpPath 'SaveResultTimeoutSeconds' 'HttpClientUtil must define a dedicated SAVERESULT timeout.'
Assert-FileContains $httpPath 'TIMEOUT' 'HttpClientUtil must classify timeout errors.'
Assert-FileContains $projectPath 'Models\MesOutboxRecord.cs' 'Project must include MesOutboxRecord.cs.'
Assert-FileContains $projectPath 'Services\MesOutboxStore.cs' 'Project must include MesOutboxStore.cs.'

Write-Host 'MES outbox source checks passed.'
