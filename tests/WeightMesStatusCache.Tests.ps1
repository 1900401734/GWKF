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
        throw "File does not exist: $Path"
    }

    $content = [System.IO.File]::ReadAllText($Path)
    if (-not $content.Contains($Text)) {
        throw $Message
    }
}

function Assert-FileNotContains {
    param(
        [string]$Path,
        [string]$Text,
        [string]$Message
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "File does not exist: $Path"
    }

    $content = [System.IO.File]::ReadAllText($Path)
    if ($content.Contains($Text)) {
        throw $Message
    }
}

$recordPath = Join-Path $RepoRoot 'MesDatas\Models\WeightMesStatusRecord.cs'
$storePath = Join-Path $RepoRoot 'MesDatas\Services\WeightMesStatusStore.cs'
$formPath = Join-Path $RepoRoot 'MesDatas\Views\Form1.cs'
$projectPath = Join-Path $RepoRoot 'MesDatas\MesDatas.csproj'

$cachePathText = [string]::Concat(
    [char]0x0044, [char]0x003A, [char]0x005C,
    [char]0x004B, [char]0x0061, [char]0x0069, [char]0x0046, [char]0x0061,
    [char]0x004C, [char]0x006F, [char]0x0067, [char]0x0073, [char]0x005C,
    [char]0x0057, [char]0x0065, [char]0x0069, [char]0x0067, [char]0x0068, [char]0x0074,
    [char]0x004D, [char]0x0045, [char]0x0053,
    [char]0x72B6, [char]0x6001, [char]0x7F13, [char]0x5B58
)

$outboxPathText = [string]::Concat(
    [char]0x0044, [char]0x003A, [char]0x005C,
    [char]0x004B, [char]0x0061, [char]0x0069, [char]0x0046, [char]0x0061,
    [char]0x004C, [char]0x006F, [char]0x0067, [char]0x0073, [char]0x005C,
    [char]0x004D, [char]0x0045, [char]0x0053,
    [char]0x8865, [char]0x4F20, [char]0x961F, [char]0x5217
)

$missingCacheReasonText = [string]::Concat(
    [char]0x672A, [char]0x627E, [char]0x5230,
    [char]0x672C, [char]0x5730,
    [char]0x0057, [char]0x0065, [char]0x0069, [char]0x0067, [char]0x0068, [char]0x0074,
    [char]0x0020, [char]0x004D, [char]0x0045, [char]0x0053,
    [char]0x786E, [char]0x8BA4, [char]0x8BB0, [char]0x5F55
)

Assert-FileContains $recordPath 'class WeightMesStatusRecord' 'Weight MES status record model must exist.'
Assert-FileContains $storePath 'class WeightMesStatusStore' 'Weight MES status store must exist.'
Assert-FileContains $storePath $cachePathText 'Weight status cache must persist under D:\KaiFaLogs\WeightMES状态缓存.'
Assert-FileNotContains $storePath $outboxPathText 'Weight status cache must not use the MES outbox queue folder.'

foreach ($field in @('Barcode', 'ProcessName', 'Status', 'FailureSource', 'ErrorMessage', 'UpdatedAt')) {
    Assert-FileContains $recordPath $field "WeightMesStatusRecord must include $field."
}

foreach ($forbidden in @('PayloadJson', 'RetryCount', 'LoadPendingRetry', 'MarkPendingRetry')) {
    Assert-FileNotContains $recordPath $forbidden "WeightMesStatusRecord must not contain outbox field $forbidden."
    Assert-FileNotContains $storePath $forbidden "WeightMesStatusStore must not contain outbox behavior $forbidden."
}

Assert-FileContains $formPath 'private readonly WeightMesStatusStore _weightMesStatusStore = new WeightMesStatusStore();' 'Form1 must hold the lightweight Weight MES status store.'
Assert-FileContains $formPath 'WeightMesStatusCacheLoadDays = 7' 'Form1 must load 7 days of Weight MES status cache.'
Assert-FileContains $formPath 'WeightMesStatusCacheRetentionDays = 30' 'Form1 must retain Weight MES status cache for 30 days.'
Assert-FileContains $formPath 'LoadRecentWeightMesStatusCache();' 'Form1 must restore Weight MES status cache during startup.'
Assert-FileContains $formPath 'SaveWeightMesStatusRecords(recordsToSave);' 'UpdateWeightMesStatus must persist Weight status to the lightweight cache.'
Assert-FileContains $formPath 'FindWeightMesStatusFromLightweightCache' 'Print gating must query the lightweight Weight status cache.'
Assert-FileContains $formPath $missingCacheReasonText 'Missing Weight status message must mention the local lightweight cache clearly.'
Assert-FileContains $projectPath 'Models\WeightMesStatusRecord.cs' 'Project must include WeightMesStatusRecord.cs.'
Assert-FileContains $projectPath 'Services\WeightMesStatusStore.cs' 'Project must include WeightMesStatusStore.cs.'

$formContent = [System.IO.File]::ReadAllText($formPath)
if (-not [regex]::IsMatch($formContent, '_weightMesStatus\.TryGetValue[\s\S]{0,700}FindWeightMesStatusFromLightweightCache[\s\S]{0,700}_mesOutboxStore\.FindLatestByBarcodeAndProcess')) {
    throw 'Print gating lookup order must be memory cache, lightweight status cache, then legacy outbox compatibility.'
}

Write-Host 'Weight MES status cache source checks passed.'
